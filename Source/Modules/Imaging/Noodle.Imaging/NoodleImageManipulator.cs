using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Encoder = System.Text.Encoder;

namespace Noodle.Imaging
{
    /// <summary>
    /// Handles resizing of the images. Manipulator is provided via other libraries (Noodle.Images.AForge, etc).
    /// </summary>
    public class NoodleImageManipulator : IImageManipulator
    {
        private readonly IImageLayoutBuilder _imageLayoutBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoodleImageManipulator"/> class.
        /// </summary>
        /// <param name="imageLayoutBuilder">The image layout builder.</param>
        public NoodleImageManipulator(IImageLayoutBuilder imageLayoutBuilder)
        {
            _imageLayoutBuilder = imageLayoutBuilder;
        }

        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual Bitmap Resize(object source, ResizeSettings parameters)
        {
            using (var job = new ImageJob(source))
            {
                var layout = _imageLayoutBuilder.BuildLayout(job.Bitmap.Size, parameters);
                var bmp = new Bitmap((int)layout.CanvasSize.Width, (int)layout.CanvasSize.Height, PixelFormat.Format24bppRgb);

                var gfx = Graphics.FromImage(bmp);
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.CompositingMode = CompositingMode.SourceOver;
                // white background
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, layout.CanvasSize.Width, layout.CanvasSize.Height);
                // draw the image
                gfx.DrawImage(job.Bitmap, layout.Image, new RectangleF(0,0, job.Bitmap.Width, job.Bitmap.Height), GraphicsUnit.Pixel);
                // commit
                gfx.Flush(FlushIntention.Flush);
                gfx.Dispose();

                return bmp;
            }
        }

        /// <summary>
        /// Manipulate (and resize) an image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">You can only resize an image</exception>
        public virtual Bitmap Manipulate(object source, ImageManipulationSettings parameters)
        {
            throw new NotSupportedException("You can only resize an image");
        }
    }
}
