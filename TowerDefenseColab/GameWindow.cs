using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameObjects;
using TowerDefenseColab.GamePhases;
using TowerDefenseColab.GamePhases.GameLevels;

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
        BufferedGraphics backBuffer;
        private GraphicsTracker _graphicsTracker;

        public GameWindow(GamePhaseManager phaseManager, StartScreen startScreen, GameLevelFactory gameLevelFactory,
            InputManager inputManager, GraphicsTracker graphicsTracker, GameBus bus)
        {
            _graphicsTracker = graphicsTracker;
            _phaseManager = phaseManager;
            _startScreen = startScreen;
            _gameLevelFactory = gameLevelFactory;
            _bus = bus;
            _inputManager = inputManager;
            _inputManager.SetMousePointFunction(() => PointToClient(Cursor.Position));

            _bus.Subscribe<MessageWindowResized>(a =>
            {
                InitBackBuffer(a.DisplayRectangle);
            });

            InitializeComponent();
            Show();
        }

        public void InitBackBuffer(Rectangle displayRectangle)
        {
            if (backBuffer != null)
            {
                backBuffer.Dispose();
            }
            BufferedGraphicsContext myContext = BufferedGraphicsManager.Current;
            backBuffer = myContext.Allocate(CreateGraphics(), displayRectangle);
        }

        private void InitGame()
        {
            //var waypoints1 = new List<PointF>() { new PointF(260, 270), new PointF(260, 120), new PointF(575, 120), new PointF(575, 270), new PointF(800, 270) };
            //var waypoints2 = new List<PointF>() { new PointF(260, 270), new PointF(260, 120), new PointF(575, 120), new PointF(575, 270), new PointF(365, 270), new PointF(365, 510), new PointF(680, 510), new PointF(680, 160), new PointF(800, 160) };

            // Create the pahses.
            // TODO: should it be even done here or by the PhageManager class itself?
            _phaseManager.Add(GamePhaseEnum.StartScreen, _startScreen);

            var map = new LevelMap
            {
                Layout = new int[10, 10]
                {
                    { 13,32,13,13,13,13,13,13,13,13 },
                    { 13,32,13,13,13,13,13,13,13,13 },
                    { 13,32,13,13,13,13,13,13,13,13 },
                    { 13,07,29,29,29,38,13,13,13,13 },
                    { 13,13,13,13,13,07,29,29,29,29 },
                    { 13,13,13,13,13,13,13,13,13,13 },
                    { 13,13,13,13,-1,13,13,13,13,13 },
                    { 13,13,13,13,13,13,13,13,13,13 },
                    { 13,13,13,13,13,13,13,13,13,13 },
                    { 13,13,13,13,13,13,13,13,13,13 }
                }

            };

            _phaseManager.Add(GamePhaseEnum.Level001,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = new[] { EnemyTypeEnum.CircleOfDeath },
                    SpawnFrequency = TimeSpan.FromSeconds(1),
                    LevelNumber = 1,
                    StartingResources = 10,
                    Waypoints = new List<Point> { new Point(1, 0), new Point(1, 3) },
                    Map = map
                }));
            _phaseManager.Add(GamePhaseEnum.Level002,
                _gameLevelFactory.CreateLevel(new GameLevelSettings
                {
                    EnemyTypesToSpawn = Enumerable.Range(0, 20).Select(i => EnemyTypeEnum.CircleOfDeath),
                    SpawnFrequency = TimeSpan.FromSeconds(1.5),
                    LevelNumber = 2,
                    StartingResources = 20,
                    Waypoints = new List<Point> { new Point(1, 0), new Point(1, 3) },
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

                // render
                _phaseManager.Render(backBuffer);

                backBuffer.Render();
                backBuffer.Render(CreateGraphics());

                Application.DoEvents();

                last = currentTimeSpan;
            }

            backBuffer.Dispose();
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
            _inputManager.MouseClicked(e);
        }

        private bool isMouseDown = false;

        private void GameWindow_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
        }

        private void GameWindow_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            _inputManager.MouseRelease(e);
        }

        private void GameWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
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