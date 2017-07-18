using System.Drawing;
using System.Numerics;

namespace TowerDefenseColab.GraphicsPoo
{
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Converts a Point to a Size.
        /// </summary>
        public static Size ToSize(this Point point)
        {
            return new Size(point);
        }

        public static Size Invert(this Size size)
        {
            return new Size(-size.Width, -size.Height);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}