using System;
using System.Drawing;
using System.Linq;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    /// <summary>
    /// A sprite sheet containing a group of images. A single sprite can be created.
    /// </summary>
    public class SpriteSheet
    {
        private readonly string _sheetName;
        public Bitmap Image { get; }
        private readonly TextureAtlas _atlas;

        public SpriteSheet(string sheetName, Bitmap image, TextureAtlas atlas)
        {
            _sheetName = sheetName;
            Image = image;
            _atlas = atlas;
        }

        public SpriteDetails GetByName(string name)
        {
            TextureAtlasSubTexture x = _atlas.SubTexture.FirstOrDefault(t => t.name == name);
            if (x == null)
            {
                throw new Exception($"Invalid sprite name '{name}' in the sheet '{_sheetName}'");
            }
            var location = new Rectangle(x.x, x.y, x.width, x.height);
            Bitmap image = Image.Clone(location, Image.PixelFormat);
            return new SpriteDetails
            {
                Bitmap = image,
                Location = location
            };
        }
    }
}