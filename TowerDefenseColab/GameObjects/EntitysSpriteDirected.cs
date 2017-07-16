using System.Drawing;
using System.Numerics;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GameObjects
{
    public class EntitysSpriteDirected
    {
        public SpriteDetails Sprite => _sprite.Sprites[_direction];
        private readonly SpriteWithDirections _sprite;
        private SpriteDirectionEnum _direction = SpriteDirectionEnum.BottomLeft;
        public bool IsRelativeToMap { get; set; } = true;

        public EntitysSpriteDirected(SpriteWithDirections sprite)
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
            Size offset = IsRelativeToMap
                ? new Size(graphicsTracker.DisplayOffset.X, graphicsTracker.DisplayOffset.Y)
                : Size.Empty;
            PointF point = PointF.Add(locationCenter, offset);
            graphics.Graphics.DrawImage(Sprite.Bitmap, point);
            return point;
        }
    }
}