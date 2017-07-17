using System.Collections.Generic;
using System.Drawing;
using TowerDefenseColab.Assets;

namespace TowerDefenseColab.GraphicsPoo.SpriteUnicorn
{
    public class SpriteSheets
    {
        private readonly Dictionary<SpriteEnum, SpriteDetails> _spriteDetails;

        public SpriteSheets(AssetsFactory assetsFactory)
        {
            Bitmap sheetLandscape = assetsFactory.GetLandscapeSpritesheet();
            Bitmap sheetCars = assetsFactory.GetCarsSpritesheet();
            Bitmap sheetTowers = assetsFactory.GetTowersSpritesheet();

            _spriteDetails = new Dictionary<SpriteEnum, SpriteDetails>
            {
                // LANDSCAPE
                {
                    SpriteEnum.LandscapeGrass,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(1193, 0, 132, 99),
                        Anchor = new Point(1258, 66)
                    }
                },
                {
                    SpriteEnum.LandscapeRoadDown,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(928, 230, 132, 99),
                        Anchor = new Point(994, 296)
                    }
                },
                {
                    SpriteEnum.LandscapeMinerals,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(0, 0, 134, 100),
                        Anchor = new Point(66, 66)
                    }
                },
                {
                    SpriteEnum.LandscapeRoadUp,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(1325, 0, 132, 99),
                        Anchor = new Point(1391, 66)
                    }
                },
                {
                    SpriteEnum.LandscapeTurnTopLeftTopRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(928, 329, 132, 99),
                        Anchor = new Point(994, 395)
                    }
                },
                {
                    SpriteEnum.LandscapeTurnBottomLeftBottomRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(1720, 100, 133, 98),
                        Anchor = new Point(1786, 165)
                    }
                },
                // CARS
                {
                    SpriteEnum.VehicleVanBottomRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetCars,
                        Location = new Rectangle(271, 64, 33, 31)
                    }
                },
                {
                    SpriteEnum.VehicleVanBottomLeft,
                    new SpriteDetails
                    {
                        Spritesheet = sheetCars,
                        Location = new Rectangle(137, 431, 33, 31)
                    }
                },
                {
                    SpriteEnum.VehicleVanTopRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetCars,
                        Location = new Rectangle(526, 345, 33, 31)
                    }
                },
                {
                    SpriteEnum.VehicleVanTopLeft,
                    new SpriteDetails
                    {
                        Spritesheet = sheetCars,
                        Location = new Rectangle(137, 258, 33, 31)
                    }
                },
                // TOWER
                {
                    SpriteEnum.Tower,
                    new SpriteDetails
                    {
                        Spritesheet = sheetTowers,
                        Location = new Rectangle(0, 308, 93, 99)
                    }
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