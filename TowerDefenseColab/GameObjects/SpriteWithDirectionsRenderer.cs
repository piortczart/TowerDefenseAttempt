using System.Drawing;
using System.Numerics;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GameObjects
{
    public class SpriteWithDirectionsRenderer
    {
        public SpriteDetails Sprite => _sprite.Sprites[_direction];
        private readonly SpriteWithDirections _sprite;
        private SpriteDirectionEnum _direction = SpriteDirectionEnum.BottomLeft;
        /// <summary>
        /// If this is true, then this entity should be rendered relatively to the map. Like a placed tower.
        /// If it's false, it should be relative to the window (like a tower being placed).
        /// </summary>
        public bool IsRelativeToMap { private get; set; } = true;

        public Size Size => Sprite.Location.Size;

        public SpriteWithDirectionsRenderer(SpriteWithDirections sprite)
        {
            _sprite = sprite;
        }

        public void ChangeDirection(Vector2 vector)
        {

            if (vector.X < 0 && vector.Y < 0)
            {
                _direction = SpriteDirectionEnum.TopLeft;
            }
            else if (vector.X > 0 && vector.Y > 0)
            {
                _direction = SpriteDirectionEnum.BottomRight;
            }
            else if (vector.X > 0 && vector.Y < 0)
            {
                _direction = SpriteDirectionEnum.TopRight;
            }
            else
            {
                _direction = SpriteDirectionEnum.BottomLeft;
            }
        }

        public PointF Render(BufferedGraphics graphics, GraphicsTracker graphicsTracker, PointF locationCenter)
        {
            PointF point = GetWindowLocation(graphicsTracker, locationCenter);
            graphics.Graphics.DrawImage(Sprite.Bitmap, point);
            return point;
        }

        public PointF GetWindowLocation(GraphicsTracker graphicsTracker, PointF locationCenter)
        {
            Size offset = IsRelativeToMap ? graphicsTracker.MapOffset.ToSize() : Size.Empty;
            return PointF.Add(locationCenter, offset);
        }
    }
}