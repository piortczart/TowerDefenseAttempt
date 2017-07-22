using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GameObjects.Enemies;
using TowerDefenseColab.GameObjects.Towers;
using TowerDefenseColab.GamePhases.Gui;
using TowerDefenseColab.GamePhases.Gui.Overlays;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    [DebuggerDisplay("State: {" + nameof(_gameState) + "}, visible: {" + nameof(IsVisible) + "}")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameLevel : GamePhase
    {
        public readonly List<Enemy> CurrentEnemiesNew = new List<Enemy>();
        private readonly GameLevelSettings _settings;
        private readonly TowerFactory _towerFactory;
        private readonly GameLevelTime _time = new GameLevelTime();
        private Queue<EnemyTypeEnum> _monstersLeftToSpawn;
        private GameState _gameState = GameState.Paused;
        private readonly List<TowerBase> _towers = new List<TowerBase>();
        private TowerBase _towerBeingPlaced;
        private Resources _resources;
        private readonly GraphicsTracker _graphicsTracker;
        private readonly FontsAndColors _fontsAndColors;
        private readonly GameMapOverlay _gameMapOverlay;
        private readonly StringFormat _stringFormatCenter;
        private readonly EnemySpawner _enemySpawner;
        private readonly ApplicationLogger _logger;
        private readonly EnemyFactory _enemyFactory;

        public GameLevel(
            GameLevelSettings settings,
            InputManager inputManager,
            TowerFactory towerFactory,
            GraphicsTracker graphicsTracker,
            MouseDragControl mouseDragControl,
            FontsAndColors fontsAndColors,
            GameBus bus,
            GameMapOverlay gameMapOverlay,
            ApplicationLogger logger,
            EnemyFactory enemyFactory)
        {
            _settings = settings;
            _towerFactory = towerFactory;
            _graphicsTracker = graphicsTracker;
            _fontsAndColors = fontsAndColors;
            _gameMapOverlay = gameMapOverlay;
            _logger = logger;
            _enemyFactory = enemyFactory;
            _stringFormatCenter = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            _gameMapOverlay.SetMap(settings.Map);

            inputManager.OnMouseDragged += mouseDragControl.InputManagerOnMouseDrag;
            inputManager.OnMouseReleased += mouseDragControl.InputManagerOnMouseRelease;
            _enemySpawner = new EnemySpawner(_time, settings.SpawnFrequency);

            bus.Subscribe<EnemyReachedGoal>(message =>
            {
                if (IsVisible)
                {
                    _gameState = GameState.Lost;
                }
            });

            bus.Subscribe<GameStateChange>(message =>
            {
                if (IsVisible && message.GameLevel == this)
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
                        _gameState = GameState.Won;
                    }
                }
            });

            bus.Subscribe<MouseClicked>(message =>
            {
                if (IsVisible && message.EventArgs.Button == MouseButtons.Left)
                {
                    if (_towerBeingPlaced != null)
                    {
                        // There is a tower being placed and mouse was clicked => place it.
                        PlaceTower(_towerBeingPlaced);
                    }
                }
            });

            bus.Subscribe<KeyReleased>(message =>
            {
                if (IsVisible)
                {
                    if (_gameState == GameState.Won)
                    {
                        bus.Publish(new GameStateChange(GameState.Won, this));
                        CurrentEnemiesNew.Clear();
                        return;
                    }
                    if (_gameState == GameState.Lost)
                    {
                        bus.Publish(new GameStateChange(GameState.Lost, this));
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
            _towerBeingPlaced = null;
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

            if (_resources.DoesAfford(newTower.Settings.CostBase))
            {
                // Add the tower (has "Setup" state).
                _towerBeingPlaced = newTower;
            }
        }

        private void PlaceTower(TowerBase towerBeingPlaced)
        {
            // TODO: Make some map checks...
            if (_resources.TryTake(towerBeingPlaced.Settings.CostBase))
            {
                towerBeingPlaced.Place();
                _towers.Add(towerBeingPlaced);
            }
            else
            {
                _towerBeingPlaced = null;
            }
        }

        private void TogglePause()
        {
            switch (_gameState)
            {
                case GameState.Rolling:
                    _gameState = GameState.Paused;
                    _time.Stop();
                    break;
                case GameState.Paused:
                    _gameState = GameState.Rolling;
                    _time.Start();
                    break;
            }
        }

        public override void Init()
        {
            _logger.LogInfo("Game level init: " + _settings.PhaseEnum);

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
            _gameMapOverlay.Render(g);

            foreach (Enemy enemyNew in CurrentEnemiesNew)
            {
                enemyNew.Render(g);
            }

            // Draw all towers.
            foreach (TowerBase tower in _towers)
            {
                tower.Render(g);
            }
            _towerBeingPlaced?.Render(g);

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

        public override void Update(TimeSpan timeDelta)
        {
            // Update the location of the tower being currently placed.
            _towerBeingPlaced?.Update(timeDelta);

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
                // TODO: Create stuff based on type
                EnemyTypeEnum typeToSpawn = _monstersLeftToSpawn.Dequeue();
                Enemy newEnemy = _enemyFactory.CreateDefault(typeToSpawn, _settings.Waypoints);
                CurrentEnemiesNew.Add(newEnemy);
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