using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.Assets
{
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

    public class AssetsFactory
    {
        private const string AssetsBase = "Assets/";
        private const string SpritesheetsBase = AssetsBase + "Spritesheets/";

        public SpriteSheet GetSpriteSheet(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextureAtlas));
            TextureAtlas atlas;
            using (var streamReader = new StreamReader($"{SpritesheetsBase}{name}.xml"))
            {
                atlas = (TextureAtlas)serializer.Deserialize(streamReader);
            }
            Image image = Image.FromFile($"{SpritesheetsBase}{name}.png");
            return new SpriteSheet(name, new Bitmap(image), atlas);
        }
    }
}
