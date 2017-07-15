using System.Collections.Generic;
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
    }
}
