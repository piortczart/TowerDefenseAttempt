using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GamePhases;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab
{
    public partial class GameWindow : Form
    {
        private readonly GamePhaseManager _phaseManager;
        private bool _isAlive = true;
        private readonly InputManager _inputManager;
        private readonly GameBus _bus;
        private readonly ApplicationLogger _applicationLogger;
        private BufferedGraphics _backBuffer;
        private bool _isMouseDown;
        private readonly LogsOverlay _logsOverlay;

        public GameWindow(GamePhaseManager phaseManager, InputManager inputManager, GameBus bus,
            ApplicationLogger applicationLogger, LogsOverlay logsOverlay)
        {
            _phaseManager = phaseManager;
            _bus = bus;
            _applicationLogger = applicationLogger;
            _logsOverlay = logsOverlay;
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
            _phaseManager.Init();
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