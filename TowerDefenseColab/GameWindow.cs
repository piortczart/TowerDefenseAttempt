using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GameObjects.Enemies;
using TowerDefenseColab.GamePhases;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.GamePhases.GameLevels.LevelLayouts;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab
{
    public partial class GameWindow : Form
    {
        private readonly GamePhaseManager _phaseManager;
        private readonly StartScreen _startScreen;
        private bool _isAlive = true;
        private readonly GameLevelFactory _gameLevelFactory;
        private readonly InputManager _inputManager;
        private readonly GameBus _bus;
        private readonly ApplicationLogger _applicationLogger;
        private BufferedGraphics _backBuffer;
        private bool _isMouseDown;
        private readonly FontsAndColors _fontsAndColors;
        private readonly LogsOverlay _logsOverlay;
        private readonly LevelLayoutLoader _layoutLoader;

        public GameWindow(GamePhaseManager phaseManager, StartScreen startScreen, GameLevelFactory gameLevelFactory,
            InputManager inputManager, GameBus bus, ApplicationLogger applicationLogger, FontsAndColors fontsAndColors,
            LogsOverlay logsOverlay, LevelLayoutLoader layoutLoader)
        {
            _phaseManager = phaseManager;
            _startScreen = startScreen;
            _gameLevelFactory = gameLevelFactory;
            _bus = bus;
            _applicationLogger = applicationLogger;
            _fontsAndColors = fontsAndColors;
            _logsOverlay = logsOverlay;
            _layoutLoader = layoutLoader;
            _inputManager = inputManager;
            _inputManager.SetMousePointFunction(() => PointToClient(Cursor.Position));

            _bus.Subscribe<MessageWindowResized>(a => { InitBackBuffer(a.DisplayRectangle); });

            InitializeComponent();
            Show();
        }

        /// <summary>
        /// Initializes the back buffer. Needs to be called when resizing the window.
        /// </summary>
        private void InitBackBuffer(Rectangle displayRectangle)
        {
            _backBuffer?.Dispose();
            BufferedGraphicsContext myContext = BufferedGraphicsManager.Current;
            _backBuffer = myContext.Allocate(CreateGraphics(), displayRectangle);
        }

        private void InitGame()
        {
            _applicationLogger.LogInfo("Initializing...");

            // Create the pahses.
            // TODO: should it be even done here or by the PhageManager class itself?
            _phaseManager.Add(GamePhaseEnum.StartScreen, _startScreen);

            _phaseManager.Add(GamePhaseEnum.Level001,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = new[] { EnemyTypeEnum.BlueVan },
                    SpawnFrequency = TimeSpan.FromSeconds(1),
                    LevelNumber = 1,
                    StartingResources = 10,
                    Waypoints = new List<Point> { new Point(1, -1), new Point(1, 3), new Point(5, 3), new Point(5, 5) },
                    Map = new LevelMap { Layout = _layoutLoader.LoadLevelLayout("01") }
                }));
            _phaseManager.Add(GamePhaseEnum.Level002,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = Enumerable.Range(0, 20).Select(i => EnemyTypeEnum.BlueVan),
                    SpawnFrequency = TimeSpan.FromSeconds(1.5),
                    LevelNumber = 2,
                    StartingResources = 20,
                    Waypoints = new List<Point> { new Point(2, 0), new Point(2, 5), new Point(5, 5), new Point(5, 9), new Point(6, 9) },
                    Map = new LevelMap { Layout = _layoutLoader.LoadLevelLayout("02") }
                }));

            _phaseManager.ChangeActiveGamePhase(GamePhaseEnum.StartScreen);

            _bus.Publish(new MessageWindowResized(DisplayRectangle));
        }

        public void GameLoop()
        {
            var stopWatch = Stopwatch.StartNew();
            TimeSpan last = stopWatch.Elapsed;

            InitGame();
            while (_isAlive)
            {
                // update
                var currentTimeSpan = stopWatch.Elapsed;
                _phaseManager.Update(currentTimeSpan - last);

                // render game phase
                _phaseManager.Render(_backBuffer);

                // render logs (if needed)
                _logsOverlay.Render(_backBuffer);

                _backBuffer.Render();
                _backBuffer.Render(CreateGraphics());

                Application.DoEvents();

                last = currentTimeSpan;
            }

            _backBuffer.Dispose();
        }


        private void GameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isAlive = false;
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            _inputManager.KeyPressed(e.KeyCode);
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            _inputManager.KeyReleased(e.KeyCode);
        }

        private void GameWindow_MouseClick(object sender, MouseEventArgs e)
        {
            _bus.Publish(new MouseClicked(e));
        }

        private void GameWindow_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
        }

        private void GameWindow_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            _inputManager.MouseRelease(e);
        }

        private void GameWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _inputManager.MouseDrag(e);
            }
        }

        private void GameWindow_ResizeEnd(object sender, EventArgs e)
        {
            _bus.Publish(new MessageWindowResized(DisplayRectangle));
        }

        private void GameWindow_Resize(object sender, EventArgs e)
        {
            _bus.Publish(new MessageWindowResized(DisplayRectangle));
        }
    }
}