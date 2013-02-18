using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Noodle.Imaging.AForge.Tests
{
    [TestFixture]
    public class ImagingTests : Noodle.Tests.ImagingTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _ImageManipulator = new AForgeImageManipulator(_imageLayoutBuilder);
        }

        protected override void AssertLayout(string sourceSize, string resizeSettings, Action<System.Drawing.RectangleF> assertImage, Action<System.Drawing.SizeF> assertCanvas)
        {
            // build the source image
            var sourceSizeSettings = new ResizeSettings(sourceSize);
            var sourceBitmap = CreateSourceBitmap(new Size(sourceSizeSettings.Width, sourceSizeSettings.Height));

            // resize
            var result = _ImageManipulator.Manipulate(sourceBitmap, new ImageManipulationSettings(resizeSettings));

            // save the result
            var fileName = sourceSize + "--" + resizeSettings + ".bmp";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            result.Save(filePath, result.RawFormat);

            Trace.WriteLine("Source:        " + sourceSize);
            Trace.WriteLine("Destination:   " + resizeSettings);
            Trace.WriteLine("   Result:     " + filePath);
        }

        protected override void ResizeResource(ResizeSettings resizeSettings, string source, string destination)
        {
            if (File.Exists(destination))
                File.Delete(destination);

            var resized = _ImageManipulator.Manipulate(source, new ImageManipulationSettings(resizeSettings)
                                                                   {
                                                                       Brightness=10,
                                                                       Contrast = 10,
                                                                       Gamma = 10,
                                                                       Hue = 180,
                                                                       Saturation = 10,
                                                                       Sharpen = 10
                                                                   });
            resized.Save(destination);
            resized.Dispose();
        }

        private Bitmap CreateSourceBitmap(Size size)
        {
            // create a bitmap for visualizing
            var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            using (var gfx = Graphics.FromImage(bitmap))
            {
                // set the background
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, size.Width, size.Height);

                // add a border
                gfx.DrawRectangle(new Pen(Color.Red, 5), 0, 0, size.Width, size.Height);

                gfx.DrawString(size.Width + "x" + size.Height, new Font("Thaoma", 8), Brushes.Black, new RectangleF(0, 0, bitmap.Width, bitmap.Height), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                gfx.Flush(FlushIntention.Flush);
            }
            return bitmap;
        }

        private ImageCodecInfo GetEncoderInfo(Guid formatID)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].FormatID == formatID)
                    return codecs[i];
            return null;
        }
    }
}
