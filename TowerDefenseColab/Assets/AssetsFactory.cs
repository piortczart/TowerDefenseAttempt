using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.Assets
{
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
