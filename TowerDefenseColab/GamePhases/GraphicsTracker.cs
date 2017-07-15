using System.Drawing;
using TowerDefenseColab.GameBusHere;

namespace TowerDefenseColab
{
    public class GraphicsTracker
    {
        public Rectangle DisplayRectangle { get; set; }
        public Point DisplayOffset { get { return _displayOffset; } }

        Point _displayOffset = new Point(200, 200);

        public GraphicsTracker(GameBus bus)
        {
            bus.Subscribe<MessageWindowResized>(m => DisplayRectangle = m.DisplayRectangle);
            bus.Subscribe<MessageMouseDragged>(m =>
            {
                _displayOffset.X += m.ChangeX;
                _displayOffset.Y += m.ChangeY;
            });
        }
    }
}