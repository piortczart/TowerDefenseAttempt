using System;
using System.Drawing;

namespace TowerDefenseColab.GameObjects
{
    public abstract class GameObjectBase : GameLoopMethods
    {
        public PointF LocationCenter { get; set; }

        private PointF LocationTopLeft
            => new PointF(LocationCenter.X - (float)Sprite.Width / 2, LocationCenter.Y - (float)Sprite.Height / 2);

        public Image Sprite { get; set; }

        public void SetLocation(Point location)
        {
            LocationCenter = location;
        }

        public void SetLocationCenter(Point location)
        {
            LocationCenter = new PointF(location.X - Sprite.Width / 2, location.Y - Sprite.Height / 2);
        }

        public override void Render(BufferedGraphics g)
        {
            g.Graphics.DrawImage(Sprite, LocationTopLeft);
        }

        public float GetDistance(GameObjectBase other)
        {
            float pow = (LocationCenter.X - other.LocationCenter.X) * (LocationCenter.X - other.LocationCenter.X) +
                        (LocationCenter.Y - other.LocationCenter.Y) * (LocationCenter.Y - other.LocationCenter.Y);
            return (float)Math.Sqrt(pow);
        }
    }
}