using System.Collections.Generic;
using System.Drawing;
using TowerDefenseColab.Assets;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    /// <summary>
    /// Gives access to all sprite sheets.
    /// </summary>
    public class SpriteSheets
    {
        private readonly Dictionary<SpriteEnum, SpriteDetails> _spriteDetails;

        public SpriteSheets(AssetsFactory assetsFactory)
        {
            Bitmap sheetTowers = assetsFactory.GetSpriteSheet("towers_grey_sheet").Image;
            SpriteSheet landscape = assetsFactory.GetSpriteSheet("landscape_sheet");
            SpriteSheet cars = assetsFactory.GetSpriteSheet("cars_sheet");
            SpriteSheet buldings = assetsFactory.GetSpriteSheet("buildings_sheet");

            _spriteDetails = new Dictionary<SpriteEnum, SpriteDetails>
            {
                // LANDSCAPE
                {
                    SpriteEnum.LandscapeGrass,
                    landscape.GetByName("landscape_28.png")
                },
                {
                    SpriteEnum.LandscapeRoadDown,
                    landscape.GetByName("landscape_32.png")
                },
                {
                    SpriteEnum.LandscapeMinerals,
                    landscape.GetByName("rocks_1.png")
                },
                {
                    SpriteEnum.LandscapeRoadUp,
                    landscape.GetByName("landscape_29.png")
                },
                {
                    SpriteEnum.LandscapeTurnTopLeftTopRight,
                    landscape.GetByName("landscape_07.png")

                },
                {
                    SpriteEnum.LandscapeTurnBottomLeftBottomRight,
                    landscape.GetByName("landscape_38.png")
                },
                {
                    SpriteEnum.LandscapeTurnBottomLeftTopLeft,
                    landscape.GetByName("landscape_03.png")
                },
                {
                    SpriteEnum.LandscapeTurnBottomRightTopRight,
                    landscape.GetByName("landscape_02.png")
                },
                // CARS
                {
                    SpriteEnum.VehicleVanBottomRight,
                    cars.GetByName("carBlue6_011.png")
                },
                {
                    SpriteEnum.VehicleVanBottomLeft,
                    cars.GetByName("carBlue6_010.png")
                },
                {
                    SpriteEnum.VehicleVanTopRight,
                    cars.GetByName("carBlue6_006.png")
                },
                {
                    SpriteEnum.VehicleVanTopLeft,
                    cars.GetByName("carBlue6_004.png")
                },
                // TOWER
                {
                    SpriteEnum.Tower,
                    new SpriteDetails
                    {
                        Spritesheet = sheetTowers,
                        Location = new Rectangle(0, 308, 93, 99)
                    }
                },
                // BUILDING
                {
                    SpriteEnum.Building,
                    buldings.GetByName("buildingTiles_123.png")
                }
            };
        }

        public SpriteDetails GetSprite(SpriteEnum spriteEnum)
        {
            SpriteDetails details = _spriteDetails[spriteEnum];
            if (details.Bitmap == null)
            {
                details.Bitmap = details.Spritesheet.Clone(details.Location, details.Spritesheet.PixelFormat);
            }
            return details;
        }
    }
}