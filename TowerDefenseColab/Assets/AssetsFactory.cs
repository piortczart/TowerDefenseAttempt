using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseColab.Assets
{
    public class AssetsFactory
    {
        Dictionary<int, Bitmap> _cache = new Dictionary<int, Bitmap>();

        public const int TileSize = 100;

        public Bitmap GetBackground(int number)
        {
            if (number == -1)
            {
                return null;
            }

            if (!_cache.ContainsKey(number))
            {
                var image = Image.FromFile(@"Assets/Landscape/landscape_" + number.ToString("00") + ".png");
                //_cache[number] = GraphicsHelper.ResizeImage(image, TileSize, TileSize);
                _cache[number] = new Bitmap(image);
            }
            return _cache[number];
        }
    }
}
