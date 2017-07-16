using System.Drawing;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class MessageWindowResized : IGameMessage
    {
        public Rectangle DisplayRectangle { get; }

        public MessageWindowResized(Rectangle displayRectangle)
        {
            DisplayRectangle = displayRectangle;
        }
    }
}
