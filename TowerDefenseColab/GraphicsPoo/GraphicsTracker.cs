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
        public Point MapOffset => _mapOffset;

        /// <summary>
        /// The offset is the relative location of the map in relation to the game window.
        /// </summary>
        Point _mapOffset = new Point(200, 200);

        public GraphicsTracker(GameBus bus)
        {
            bus.Subscribe<MessageWindowResized>(m => DisplayRectangle = m.DisplayRectangle);
            bus.Subscribe<MessageMouseDragged>(m =>
            {
                _mapOffset.X += m.ChangeX;
                _mapOffset.Y += m.ChangeY;
            });
        }

        public PointF ConvertWindowCoordsToMapCoords(PointF windowCoords)
        {
            return PointF.Subtract(windowCoords, MapOffset.ToSize());
        }
    }
}