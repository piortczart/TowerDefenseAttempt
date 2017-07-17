using System.Windows.Forms;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class KeyReleased : IGameMessage
    {
        public Keys Key { get; }

        public KeyReleased(Keys key)
        {
            Key = key;
        }
    }
}