using System.Drawing;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    /// <summary>
    /// Details of a sprite - it's actual image and location within the original spritesheet.
    /// </summary>
    public class SpriteDetails
    {
        public Rectangle Location { get; set; }
        public Bitmap Bitmap { get; set; }
        public Bitmap Spritesheet { get; set; }
    }
}