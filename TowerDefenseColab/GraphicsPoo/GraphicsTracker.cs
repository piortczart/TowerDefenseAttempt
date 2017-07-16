using System.Drawing;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;

namespace TowerDefenseColab.GraphicsPoo
{
    /// <summary>
    /// Keeps track of some display settings.
    /// </summary>
    public class GraphicsTracker
    {
        public Rectangle DisplayRectangle { get; private set; }
        public Point DisplayOffset => _displayOffset;

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