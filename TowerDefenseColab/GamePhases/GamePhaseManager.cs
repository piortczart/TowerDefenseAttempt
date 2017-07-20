using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GameObjects.Enemies;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.GamePhases.GameLevels.LevelLayouts;
using TowerDefenseColab.GamePhases.GameLevels.MapGeneration;

namespace TowerDefenseColab.GamePhases
{
    public class GamePhaseManager : GameLoopMethods
    {
        private readonly Dictionary<GamePhaseEnum, GamePhase> _gamePhases =
            new Dictionary<GamePhaseEnum, GamePhase>();

        private GamePhase _activeGamePhase;
        private readonly MapGenerator _mapGenerator;
        private readonly GameLevelFactory _gameLevelFactory;
        private readonly LevelLayoutLoader _layoutLoader;

        public GamePhaseManager(MapGenerator mapGenerator, GameLevelFactory gameLevelFactory,
            LevelLayoutLoader layoutLoader, GameBus bus)
        {
            _mapGenerator = mapGenerator;
            _gameLevelFactory = gameLevelFactory;
            _layoutLoader = layoutLoader;
            bus.Subscribe<GameStateChange>(change =>
            {
                switch (change.GameState)
                {
                    case GameState.Lost:
                        LevelEndedPlayerLost();
                        break;
                    case GameState.Won:
                        LevelEndedPlayerWon();
                        break;
                }
            });
        }

        private void Add(GamePhaseEnum phaseType, GamePhase gamePhase)
        {
            _gamePhases.Add(phaseType, gamePhase);
        }

        public override void Init()
        {
            GeneratedMap generatedMap = _mapGenerator.GenerateMap();

            Add(GamePhaseEnum.Level001,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = new[] { EnemyTypeEnum.BlueVan },
                    SpawnFrequency = TimeSpan.FromSeconds(1),
                    LevelNumber = 1,
                    StartingResources = 10,
                    Waypoints = generatedMap.Path,
                    Map = new LevelMap { Layout = generatedMap.Map }
                }));

            Add(GamePhaseEnum.Level002,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = Enumerable.Range(0, 20).Select(i => EnemyTypeEnum.BlueVan),
                    SpawnFrequency = TimeSpan.FromSeconds(1.5),
                    LevelNumber = 2,
                    StartingResources = 20,
                    Waypoints =
                        new List<Point>
                        {
                            new Point(2, 0),
                            new Point(2, 5),
                            new Point(5, 5),
                            new Point(5, 9),
                            new Point(6, 9)
                        },
                    Map = new LevelMap { Layout = _layoutLoader.LoadLevelLayout("02") }
                }));

            ChangeActiveGamePhase(_gamePhases.First().Key);

            _activeGamePhase.Init();
        }

        public override void Update(TimeSpan timeDelta)
        {
            _activeGamePhase.Update(timeDelta);
        }

        public override void Render(BufferedGraphics gr)
        {
            _activeGamePhase.Render(gr);
        }

        private void ChangeActiveGamePhase(GamePhaseEnum gamePhase)
        {
            if (_activeGamePhase != null)
            {
                _activeGamePhase.IsVisible = false;
            }
            _activeGamePhase = _gamePhases[gamePhase];
            _activeGamePhase.Init();
            _activeGamePhase.IsVisible = true;
        }

        /// <summary>
        /// Called by the level itself when it finishes.
        /// </summary>
        private void LevelEndedPlayerWon()
        {
            // Fugly way of selecting the next level.
            // Assumes levels are at the end of the _gamePhases dictionary.
            int nextIndex = _gamePhases.Values.ToList().IndexOf(_activeGamePhase) + 1;
            GamePhaseEnum nextLevel = nextIndex >= _gamePhases.Count
                ? GamePhaseEnum.StartScreen
                : _gamePhases.Keys.ToList()[nextIndex];

            ChangeActiveGamePhase(nextLevel);
        }

        private void LevelEndedPlayerLost()
        {
            // Again a fugly way of doing this.
            // Re-initialize current game pahse.
            int currentIndex = _gamePhases.Values.ToList().IndexOf(_activeGamePhase);
            ChangeActiveGamePhase(_gamePhases.Keys.ToList()[currentIndex]);
        }
    }
}