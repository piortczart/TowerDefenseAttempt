using System.Windows.Forms;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class MouseClicked : IGameMessage
    {
        private MouseEventArgs _eventArgs;

        public MouseClicked(MouseEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }
    }
}