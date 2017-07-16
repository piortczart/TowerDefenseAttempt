using System.Collections.Generic;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    public enum SpriteDirectionEnum
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class SpriteWithDirections
    {
        public Dictionary<SpriteDirectionEnum, SpriteDetails> Sprites { get; set; }
    }
}