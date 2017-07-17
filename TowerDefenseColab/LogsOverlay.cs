using System.Drawing;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab
{
    public class LogsOverlay
    {
        private readonly ApplicationLogger _applicationLogger;
        private readonly FontsAndColors _fontsAndColors;
        private bool _showLogs;
        private Point _logsWindowOffset = new Point(20, 50);
        private int _logHeight = 20;

        public LogsOverlay(ApplicationLogger applicationLogger, FontsAndColors fontsAndColors, GameBus bus)
        {
            _applicationLogger = applicationLogger;
            _fontsAndColors = fontsAndColors;

            bus.Subscribe<KeyReleased>(message =>
            {
                switch (message.Key)
                {
                    case Keys.L:
                        _showLogs = !_showLogs;
                        break;
                }
            });
        }

        public void Render(BufferedGraphics backBuffer)
        {
            if (_showLogs)
            {

                int logNumber = 0;
                foreach (string log in _applicationLogger.Logs)
                {
                    backBuffer.Graphics.DrawString(log, _fontsAndColors.MonospaceFontSmaller, _fontsAndColors.BlackBrush,
                        _logsWindowOffset.X, _logsWindowOffset.Y + logNumber++ * _logHeight);
                }
            }
        }
    }
}