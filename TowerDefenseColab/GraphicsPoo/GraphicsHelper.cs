﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TowerDefenseColab.GraphicsPoo
{
    public class GraphicsHelper
    {
        /// <summary>
        /// Converts the map coordiates to real graphics coords.
        /// </summary>
        public static Point ConvertMapCoordsToWindowCoords(int mapX, int mapY, Point offset)
        {
            int tileAdjustmentX = 64;
            int tileAdjustmentY = 32;

            int rx = mapX * tileAdjustmentX + mapY * tileAdjustmentX;
            int ry = -mapX * tileAdjustmentY + mapY * tileAdjustmentY;
            rx += offset.X;
            ry += offset.Y;

            return new Point(rx, ry);
        }

        //public static Point ConvertWindowCoordsToMapCoords(Point windowCoords, Point offset)
        //{
        //    int tileAdjustmentX = 64;
        //    int tileAdjustmentY = 32;

        //    int rx = x * tileAdjustmentX + y * tileAdjustmentX;
        //    int ry = -x * tileAdjustmentY + y * tileAdjustmentY;
        //    rx += offset.X;
        //    ry += offset.Y;

        //    return new Point(rx, ry);
        //}

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
