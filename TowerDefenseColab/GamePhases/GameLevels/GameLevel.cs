using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GameMechanisms;
using TowerDefenseColab.GameObjects.Enemies;
using TowerDefenseColab.GameObjects.Towers;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameLevel : GamePhase
    {
        public readonly List<Enemy> CurrentEnemiesNew = new List<Enemy>();
        private readonly GameLevelSettings _settings;
        private readonly GamePhaseManager _gamePhaseManager;
        private readonly InputManager _inputManager;
        private readonly TowerFactory _towerFactory;
        private readonly GameLevelTime _time = new GameLevelTime();
        private Queue<EnemyTypeEnum> _monstersLeftToSpawn;
        private GameState _gameState = GameState.Paused;
        private readonly List<TowerBase> _towers = new List<TowerBase>();
        private Resources _resources;
        private readonly GraphicsTracker _graphicsTracker;
        private readonly FontsAndColors _fontsAndColors;
        private readonly SpriteSheets _spriteSheets;
        private readonly ApplicationLogger _logger;
        private readonly GameBus _bus;
        private readonly StringFormat _stringFormatCenter;
        private readonly EnemySpawner _enemySpawner;

        public GameLevel(
            GameLevelSettings settings,
            GamePhaseManager gamePhaseManager,
            InputManager inputManager,
            TowerFactory towerFactory,
            GraphicsTracker graphicsTracker,
            MouseDragControl mouseDragControl,
            FontsAndColors fontsAndColors,
            SpriteSheets spriteSheets,
            ApplicationLogger logger,
            GameBus bus)
        {
            _settings = settings;
            _gamePhaseManager = gamePhaseManager;
            _inputManager = inputManager;
            _towerFactory = towerFactory;
            _graphicsTracker = graphicsTracker;
            _fontsAndColors = fontsAndColors;
            _spriteSheets = spriteSheets;
            _logger = logger;
            _bus = bus;
            _stringFormatCenter = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            inputManager.OnMouseDragged += mouseDragControl.InputManagerOnMouseDrag;
            inputManager.OnMouseReleased += mouseDragControl.InputManagerOnMouseRelease;
            _enemySpawner = new EnemySpawner(_time, settings.SpawnFrequency);

            bus.Subscribe<EnemyReachedGoal>(message =>
            {
                if (IsVisible)
                {
                    bus.Publish(new GameStateChange(GameState.Lost));
                }
            });

            bus.Subscribe<GameStateChange>(message =>
            {
                if (IsVisible)
                {
                    _gameState = message.GameState;
                }
            });

            bus.Subscribe<EnemyDespawned>(message =>
            {
                if (IsVisible)
                {
                    CurrentEnemiesNew.Remove(message.Enemy);
                    if (CurrentEnemiesNew.Count == 0 && _monstersLeftToSpawn.Count == 0)
                    {
                        _bus.Publish(new GameStateChange(GameState.Won));
                    }
                }
            });

            bus.Subscribe<MouseClicked>(message =>
            {
                if (IsVisible && message.EventArgs.Button == MouseButtons.Left)
                {
                    TowerBase towerBeingPlaced = _towers.SingleOrDefault(t => t.TowerStateEnum == TowerStateEnum.Setup);
                    // There is a tower being placed and mouse was clicked => place it.
                    // TODO: Make some map checks...
                    towerBeingPlaced?.Place();
                }
            });

            bus.Subscribe<KeyReleased>(message =>
            {
                if (IsVisible)
                {
                    if (_gameState == GameState.Won)
                    {
                        _bus.Publish(new GameStateChange(GameState.Won));
                        _gamePhaseManager.LevelEndedPlayerWon(this);
                        CurrentEnemiesNew.Clear();
                        return;
                    }
                    if (_gameState == GameState.Lost)
                    {
                        _bus.Publish(new GameStateChange(GameState.Lost));
                        _gamePhaseManager.LevelEndedPlayerLost(this);
                        CurrentEnemiesNew.Clear();
                        return;
                    }

                    switch (message.Key)
                    {
                        case Keys.Space:
                            TogglePause();
                            break;
                        case Keys.D1:
                            StartPlacingTower();
                            break;
                    }
                }
            });
        }

        private void StartPlacingTower()
        {
            _towers.RemoveAll(t => t.TowerStateEnum == TowerStateEnum.Setup);
            TowerBase newTower = _towerFactory.GetTower(_time, this,
                new TowerSettings
                {
                    Powah = 1,
                    RangePixels = 100,
                    ShootFrequency = TimeSpan.FromSeconds(1),
                    CostBase = 10
                },
                _graphicsTracker);
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
            _resources = new Resources(_settings.StartingResources);
        }

        public override void Render(BufferedGraphics g)
        {
            // Clearing the screen.
            g.Graphics.FillRectangle(_fontsAndColors.BackgroundBrush, _graphicsTracker.DisplayRectangle);

            // Drawing the map using tiles.
            RenderMap(g);

            foreach (Enemy enemyNew in CurrentEnemiesNew)
            {
                enemyNew.Render(g);
            }

            // Draw all towers.
            foreach (TowerBase tower in _towers)
            {
                tower.Render(g);
            }

            // Some extra info depending on the game state.
            switch (_gameState)
            {
                case GameState.Won:
                    g.Graphics.DrawString("Wow. You won.", _fontsAndColors.MonospaceFont, _fontsAndColors.BlueBrush,
                        _graphicsTracker.DisplayRectangle, _stringFormatCenter);
                    return;
                case GameState.Lost:
                    g.Graphics.DrawString("You noob. You lost. Again.", _fontsAndColors.MonospaceFont,
                        _fontsAndColors.BlueBrush, _graphicsTracker.DisplayRectangle, _stringFormatCenter);
                    return;
                case GameState.Paused:
                    // Show pause info.
                    g.Graphics.DrawString("! PAUSED !", _fontsAndColors.MonospaceFont, _fontsAndColors.BlueBrush,
                        _graphicsTracker.DisplayRectangle, _stringFormatCenter);
                    g.Graphics.DrawString($"space - pause{Environment.NewLine}1 - new tower (click to place)",
                        _fontsAndColors.MonospaceFontSmaller, _fontsAndColors.BlueBrush, 370, 500);
                    break;
            }

            // Draw time and resources.
            g.Graphics.DrawString($"{_time.GetCurrent()}",
                _fontsAndColors.MonospaceFontSmaller, _fontsAndColors.BlueBrush, 10, 0);
            g.Graphics.DrawString($"${_resources.Amount}",
                _fontsAndColors.MonospaceFontSmaller, _fontsAndColors.BlueBrush,
                _graphicsTracker.DisplayRectangle.Width - 100, 20);
        }

        private void RenderMap(BufferedGraphics graphics)
        {
            for (int y = 0; y < _settings.Map.Layout.GetLength(0); y++)
            {
                for (int x = _settings.Map.Layout.GetLength(1) - 1; x >= 0; x--)
                {
                    SpriteEnum sprinteEnum = _settings.Map.Layout[y, x];
                    SpriteDetails sprite = _spriteSheets.GetSprite(sprinteEnum);
                    if (sprite != null)
                    {
                        // This is the point where the sprite should be painted.
                        Point pointOnScreen = GraphicsHelper.ConvertMapToReal(x, y, _graphicsTracker.MapOffset);

                        // Calculate the anchor location of the sprite.
                        var xx = sprite.Anchor.X - sprite.Location.X;
                        var yy = sprite.Anchor.Y - sprite.Location.Y;

                        Point realPoint = new Point(pointOnScreen.X - xx, pointOnScreen.Y - yy);

                        graphics.Graphics.DrawImage(sprite.Bitmap, realPoint);
                        graphics.Graphics.DrawString(y + "," + x, _fontsAndColors.MonospaceFontSmaller,
                            _fontsAndColors.BlackBrush, pointOnScreen);
                    }
                }
            }
        }

        public override void Update(TimeSpan timeDelta)
        {
            // Update the location of the tower being currently placed.
            TowerBase towerBeingPlaced = _towers.SingleOrDefault(t => t.TowerStateEnum == TowerStateEnum.Setup);
            towerBeingPlaced?.Update(timeDelta);

            if (_gameState == GameState.Paused || _gameState == GameState.Lost || _gameState == GameState.Won)
            {
                return;
            }

            // Create a new enemy if appropriate.
            SpawnSomething();

            foreach (TowerBase tower in _towers)
            {
                tower.Update(timeDelta);
            }

            foreach (Enemy enemyNew in CurrentEnemiesNew.ToList())
            {
                enemyNew.Update(timeDelta);
            }

            foreach (Enemy monster in CurrentEnemiesNew.ToList())
            {
                // "Despawn" if dead...
                if (!monster.IsVisible)
                {
                    CurrentEnemiesNew.Remove(monster);
                }
                monster.Update(timeDelta);
            }
        }

        private void SpawnSomething()
        {
            if (_enemySpawner.SpawnEnemy() && _monstersLeftToSpawn.Count > 0)
            {
                // TODO: actually spawn that type.
                var typeToSpawn = _monstersLeftToSpawn.Dequeue();

                var spr = new SpriteWithDirections
                {
                    Sprites = new Dictionary<SpriteDirectionEnum, SpriteDetails>
                    {
                        {SpriteDirectionEnum.BottomLeft, _spriteSheets.GetSprite(SpriteEnum.VehicleVanBottomLeft)},
                        {SpriteDirectionEnum.BottomRight, _spriteSheets.GetSprite(SpriteEnum.VehicleVanBottomRight)},
                        {SpriteDirectionEnum.TopLeft, _spriteSheets.GetSprite(SpriteEnum.VehicleVanTopLeft)},
                        {SpriteDirectionEnum.TopRight, _spriteSheets.GetSprite(SpriteEnum.VehicleVanTopRight)}
                    }
                };

                CurrentEnemiesNew.Add(new Enemy(spr, _graphicsTracker, _settings.Waypoints, _logger, _bus, _fontsAndColors));
            }
        }

        ///// <summary>
        ///// This will be called when an enemy dies.
        ///// </summary>
        //private void OnEnemyDeath(EnemyBase enemy)
        //{
        //    // Git some moneyz!
        //    _resources.AddForKilling(enemy);
        //}
    }
}