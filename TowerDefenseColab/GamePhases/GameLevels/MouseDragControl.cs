using System.Drawing;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class MouseDragControl
    {
        private readonly GameBus _bus;
        private Point _lastMouseLocationDrag = new Point(0, 0);

        public MouseDragControl(GameBus bus)
        {
            _bus = bus;
        }

        public void InputManagerOnMouseRelease(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // When the mouse is released, we want to reset it's last location.
                _lastMouseLocationDrag = new Point(0, 0);
            }
        }

        public void InputManagerOnMouseDrag(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
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
    }
}