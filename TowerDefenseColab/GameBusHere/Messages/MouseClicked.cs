using System.Windows.Forms;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class MouseClicked : IGameMessage
    {
        public MouseEventArgs EventArgs { get; }

        public MouseClicked(MouseEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
    }
}