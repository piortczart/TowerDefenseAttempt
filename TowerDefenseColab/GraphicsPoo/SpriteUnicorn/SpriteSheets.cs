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

            _spriteDetails = new Dictionary<SpriteEnum, SpriteDetails>
            {
                // LANDSCAPE
                {
                    SpriteEnum.LandscapeGrass,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(1193, 0, 132, 99),
                        Anchor = new Point(0, 0)
                    }
                },
                {
                    SpriteEnum.LandscapeRoadDown,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(928, 230, 132, 99),
                        Anchor = new Point(0, 0)
                    }
                },
                {
                    SpriteEnum.LandscapeMinerals,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(0, 0, 134, 100),
                        Anchor = new Point(0, 0)
                    }
                },
                {
                    SpriteEnum.LandscapeRoadUp,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(1325, 0, 132, 99),
                        Anchor = new Point(0, 0)
                    }
                },
                {
                    SpriteEnum.LandscapeTurnTopLeftTopRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetLandscape,
                        Location = new Rectangle(928, 329, 132, 99),
                        Anchor = new Point(0, 0)
                    }
                },
                // CARS
                {
                    SpriteEnum.VehicleVanBottomRight,
                    new SpriteDetails
                    {
                        Spritesheet = sheetCars,
                        Location = new Rectangle(271, 64, 33, 31),
                        Anchor = new Point(0, 0)
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