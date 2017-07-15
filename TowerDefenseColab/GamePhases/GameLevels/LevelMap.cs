using System.Drawing;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class LevelMap
    {
        public Size Size { get { return new Size(Layout.GetLength(0), Layout.GetLength(1)); } }
        public int[,] Layout { get; set; }
    }
}
