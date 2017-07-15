using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TowerDefenseColab.Assets;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameMechanisms;
using TowerDefenseColab.GameObjects;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class MouseDragControl
    {
        private GameBus _bus;
        private Point _lastMouseLocationDrag = new Point(0, 0);
        public MouseDragControl(GameBus bus)
        {
            _bus = bus;
        }

        public void InputManagerOnMouseRelease(MouseEventArgs e)
        {
            /// When the mouse is released, we want to reset it's last location.
            _lastMouseLocationDrag = new Point(0, 0);
        }

        public void InputManagerOnMouseDrag(MouseEventArgs e)
        {
            // When the mouse is dragged, we want to adjust the offset to move the rendered map/stuff.
            if (_lastMouseLocationDrag.X != 0)
            {
                int dx = e.Location.X - _lastMouseLocationDrag.X;
                int dy = e.Location.Y - _lastMouseLocationDrag.Y;

                _bus.Publish(new MessageMouseDragged
                {
                    ChangeX = dx,
                    ChangeY = dy
                });
            }
            _lastMouseLocationDrag = e.Location;
        }
    }

    public class GameLevel : GamePhase
    {
        private Image _background;
        public readonly List<EnemyBase> CurrentMonsters = new List<EnemyBase>();
        private readonly GameLevelSettings _settings;
        private readonly EnemyFactory _enemyFactory;
        private TimeSpan _lastSpawn = TimeSpan.MinValue;
        private readonly GamePhaseManager _gamePhaseManager;
        private readonly InputManager _inputManager;
        private readonly TowerFactory _towerFactory;
        private readonly GameLevelTime _time = new GameLevelTime();
        private Queue<EnemyTypeEnum> _monstersLeftToSpawn;
        private GameState _gameState = GameState.Paused;
        private readonly List<TowerBase> _towers = new List<TowerBase>();
        private Resources _resources;
        private readonly AssetsFactory _assetsFactory;
        private MouseDragControl _mouseDragControl;
        private GraphicsTracker _graphicsTracker;
        private StringFormat _stringFormatCenter;

        public GameLevel(GameLevelSettings settings, EnemyFactory enemyFactory, GamePhaseManager gamePhaseManager,
            InputManager inputManager, TowerFactory towerFactory, AssetsFactory assetsFactory, GraphicsTracker graphicsTracker, MouseDragControl mouseDragControl)
        {
            _settings = settings;
            _enemyFactory = enemyFactory;
            _gamePhaseManager = gamePhaseManager;
            _inputManager = inputManager;
            _towerFactory = towerFactory;
            _assetsFactory = assetsFactory;
            _mouseDragControl = mouseDragControl;
            _graphicsTracker = graphicsTracker;
            _stringFormatCenter = new StringFormat();
            _stringFormatCenter.LineAlignment = StringAlignment.Center;
            _stringFormatCenter.Alignment = StringAlignment.Center;

            inputManager.OnKeyReleased += OnKeyRelease;
            inputManager.OnClick += OnMouseClick;
            inputManager.OnMouseDragged += _mouseDragControl.InputManagerOnMouseDrag;
            inputManager.OnMouseReleased += _mouseDragControl.InputManagerOnMouseRelease;
        }

        private void OnMouseClick(MouseEventArgs mouseEventArgs)
        {
            if (IsVisible)
            {
                TowerBase placing = _towers.SingleOrDefault(t => t.TowerStateEnum == TowerStateEnum.Setup);
                if (placing != null)
                {
                    placing.TowerStateEnum = TowerStateEnum.Active;
                }
            }
        }

        private void OnKeyRelease(Keys key)
        {
            if (IsVisible)
            {
                if (_gameState == GameState.Won)
                {
                    _gamePhaseManager.LevelEndedPlayerWon(this);
                    CurrentMonsters.Clear();
                    return;
                }
                else if (_gameState == GameState.Lost)
                {
                    _gamePhaseManager.LevelEndedPlayerLost(this);
                    CurrentMonsters.Clear();
                    return;
                }

                switch (key)
                {
                    case Keys.Space:
                        TogglePause();
                        break;
                    case Keys.D1:
                        StartPlacingTower();
                        break;
                }
            }
        }

        private void StartPlacingTower()
        {
            _towers.RemoveAll(t => t.TowerStateEnum == TowerStateEnum.Setup);
            TowerBase newTower = _towerFactory.GetTower(_time, this, new TowerSettings
            {
                Powah = 1,
                RangePixels = 100,
                ShootFrequency = TimeSpan.FromSeconds(1),
                CostBase = 10
            });
            newTower.Init();

            if (_resources.TryTake(newTower.Settings.CostBase))
            {
                _towers.Add(newTower);
            }
        }

        private void TogglePause()
        {
            if (_gameState == GameState.Rolling)
            {
                _gameState = GameState.Paused;
                _time.Stop();
            }
            else if (_gameState == GameState.Paused)
            {
                _gameState = GameState.Rolling;
                _time.Start();
            }
        }

        public override void Init()
        {
            _time.Reset();
            _towers.Clear();
            _gameState = GameState.Paused;
            _monstersLeftToSpawn = new Queue<EnemyTypeEnum>(_settings.EnemyTypesToSpawn);
            _background = Image.FromFile($@"Assets\bglvl{_settings.LevelNumber}Path.png");
            _resources = new Resources(_settings.StartingResources);
        }

        public static Point ConvertMapToReal(int x, int y, Point offset)
        {
            var blaX = 64;
            var blaY = 32;

            int rx = x * blaX + y * blaX;
            int ry = -x * blaY + y * blaY;
            rx += offset.X;
            ry += offset.Y;

            return new Point(rx, ry);
        }


        public override void Render(BufferedGraphics g)
        {
            // Clearing the screen.
            g.Graphics.DrawImage(_background, 0, 0);

            // Drawing the map using tiles.
            for (int y = 0; y < _settings.Map.Layout.GetLength(0); y++)
            {
                for (int x = _settings.Map.Layout.GetLength(1) - 1; x >= 0; x--)
                {
                    Bitmap aTile = _assetsFactory.GetBackground(_settings.Map.Layout[y, x]);
                    if (aTile != null)
                    {
                        Point p = ConvertMapToReal(x, y, _graphicsTracker.DisplayOffset);
                        g.Graphics.DrawString(y + "," + x, new Font(FontFamily.GenericMonospace, 12), Brushes.Black, p);
                        g.Graphics.DrawImage(aTile, p);
                    }
                }
            }

            // Draw all monsters.
            foreach (EnemyBase monster in CurrentMonsters)
            {
                monster.Render(g);
            }

            // Draw all towers.
            foreach (TowerBase tower in _towers)
            {
                tower.Render(g);
            }

            // Some extra info depending on the game state.
            if (_gameState == GameState.Won)
            {
                g.Graphics.DrawString("Wow. You won.", new Font("monospace", 20), new SolidBrush(Color.Blue), _graphicsTracker.DisplayRectangle, _stringFormatCenter);
                return;
            }
            else if (_gameState == GameState.Lost)
            {
                g.Graphics.DrawString("You noob. You lost. Again.", new Font("monospace", 20), new SolidBrush(Color.Blue), _graphicsTracker.DisplayRectangle, _stringFormatCenter);
                return;
            }

            // Show pause info.
            if (_gameState == GameState.Paused)
            {
                g.Graphics.DrawString("! PAUSED !", new Font("monospace", 20),
                    new SolidBrush(Color.Blue), _graphicsTracker.DisplayRectangle, _stringFormatCenter);

                g.Graphics.DrawString($"space - pause{Environment.NewLine}1 - new tower (click to place)",
                    new Font("monospace", 10), new SolidBrush(Color.Blue), 370, 500);
            }

            // Draw time and resources.
            g.Graphics.DrawString($"{_time.GetCurrent()}",
                new Font("monospace", 10), new SolidBrush(Color.Blue), 10, 0);
            g.Graphics.DrawString($"${_resources.Amount}",
                new Font("monospace", 10), new SolidBrush(Color.Blue), _graphicsTracker.DisplayRectangle.Width - 100, 20);
        }

        public override void Update(TimeSpan timeDelta)
        {
            // Update the location of the tower being currently placed.
            TowerBase placing = _towers.SingleOrDefault(t => t.TowerStateEnum == TowerStateEnum.Setup);
            placing?.SetLocationCenter(_inputManager.GetMousePosition());

            if (_gameState == GameState.Paused)
            {
                return;
            }

            // Create a new enemy if appropriate.
            SpawnSomething();

            if (CurrentMonsters.Count == 0 && _monstersLeftToSpawn.Count == 0)
            {
                _gameState = GameState.Won;
                return;
            }

            foreach (TowerBase tower in _towers)
            {
                tower.Update(timeDelta);
            }

            foreach (EnemyBase monster in CurrentMonsters.ToList())
            {
                if (monster.FoundPointG)
                {
                    _gameState = GameState.Lost;
                    break;
                }

                // "Despawn" if dead...
                if (!monster.IsVisible)
                {
                    CurrentMonsters.Remove(monster);
                }
                monster.Update(timeDelta);
            }
        }

        private void SpawnSomething()
        {
            TimeSpan nao = _time.GetCurrent();
            bool isFirstSpawn = _lastSpawn == TimeSpan.MinValue;
            // Spawn enemy if no enemy was spawned yet or if the time since last spawn is long enough.
            bool shouldSpawnEnemy = (_lastSpawn + _settings.SpawnFrequency <= nao) || isFirstSpawn;
            if (_monstersLeftToSpawn.Count > 0 && shouldSpawnEnemy)
            {
                EnemyTypeEnum enemyType = _monstersLeftToSpawn.Dequeue();
                EnemyBase enemy = _enemyFactory.GetEnemy(enemyType);
                enemy.Init();
                enemy.SetLocation(_settings.SpawnPoint);
                enemy.Waypoints = _settings.Waypoints;
                enemy.OnDeathAction = OnEnemyDeath;
                CurrentMonsters.Add(enemy);
                _lastSpawn = nao;
            }
        }

        /// <summary>
        /// This will be called when an enemy dies.
        /// </summary>
        private void OnEnemyDeath(EnemyBase enemy)
        {
            // Git some moneyz!
            _resources.AddForKilling(enemy);
        }
    }
}