using System.Collections.Generic;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    /// <summary>
    /// A set of sprites which can be facing multiple directions.
    /// </summary>
    public class SpriteWithDirections
    {
        public Dictionary<SpriteDirectionEnum, SpriteDetails> Sprites { get; set; }
    }
}