using System.Drawing;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class LevelMap
    {
        public Size Size { get { return new Size(Layout.GetLength(0), Layout.GetLength(1)); } }
        public SpriteEnum[,] Layout { get; set; }
    }
}
