using System.Drawing;

namespace TowerDefenseColab.Assets
{
    public class AssetsFactory
    {

        public Bitmap GetLandscapeSpritesheet()
        {
            Image image = Image.FromFile(@"Assets/Spritesheets/landscape_sheet.png");
            return new Bitmap(image);
        }
        public Bitmap GetCarsSpritesheet()
        {
            Image image = Image.FromFile(@"Assets/Spritesheets/cars_sheet.png");
            return new Bitmap(image);
        }

        public Bitmap GetTowersSpritesheet()
        {
            Image image = Image.FromFile(@"Assets/Spritesheets/towers_grey_sheet.png");
            return new Bitmap(image);
        }
    }
}
