using System.Drawing;

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
    }
}