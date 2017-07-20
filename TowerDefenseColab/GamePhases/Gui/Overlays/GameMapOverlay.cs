using System;
using System.Drawing;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GamePhases.Gui.Overlays
{
    public class GameMapOverlay : GameLoopMethods
    {
        private LevelMap _map;
        private readonly SpriteSheets _spriteSheets;
        private readonly GraphicsTracker _graphicsTracker;
        private readonly FontsAndColors _fontsAndColors;

        public GameMapOverlay(SpriteSheets spriteSheets, GraphicsTracker graphicsTracker, FontsAndColors fontsAndColors)
        {
            _spriteSheets = spriteSheets;
            _graphicsTracker = graphicsTracker;
            _fontsAndColors = fontsAndColors;
        }

        public void SetMap(LevelMap map)
        {
            _map = map;
        }

        public override void Init()
        {
        }

        public override void Update(TimeSpan timeDelta)
        {
        }

        public override void Render(BufferedGraphics graphics)
        {
            for (int y = 0; y < _map.Layout.GetLength(0); y++)
            {
                for (int x = _map.Layout.GetLength(1) - 1; x >= 0; x--)
                {
                    SpriteEnum sprinteEnum = _map.Layout[y, x];
                    SpriteDetails sprite = _spriteSheets.GetSprite(sprinteEnum);
                    if (sprite != null)
                    {
                        // This is the point where the sprite should be painted.
                        Point pointOnScreen = GraphicsHelper.ConvertMapToReal(x, y, _graphicsTracker.MapOffset);

                        // Calculate the anchor location of the sprite.
                        int xx = sprite.Anchor.X - sprite.Location.X;
                        int yy = sprite.Anchor.Y - sprite.Location.Y;

                        Point realPoint = new Point(pointOnScreen.X - xx, pointOnScreen.Y - yy);

                        graphics.Graphics.DrawImage(sprite.Bitmap, realPoint);
                        graphics.Graphics.DrawString(y + "," + x, _fontsAndColors.MonospaceFontSmaller,
                            _fontsAndColors.BlackBrush, pointOnScreen);
                    }
                }
            }
        }
    }
}