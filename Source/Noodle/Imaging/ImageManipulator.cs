using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Noodle.Imaging
{
    /// <summary>
    /// Manipulates an image.
    /// </summary>
    public class ImageManipulator : IImageManipulator
    {
        public virtual bool Resize(FileInfo image, ImageResizeParameters parameters, FileInfo resizedImage)
        {
            using (var s = image.OpenRead())
            {
                var imageBytes = new byte[s.Length];
                s.Read(imageBytes, 0, imageBytes.Length);
            }
            using (var sourceStream = image.OpenRead())
            {
                return Resize(sourceStream, parameters, resizedImage.Open(FileMode.OpenOrCreate));
            }
        }

        public virtual bool Resize(Stream inputStream, ImageResizeParameters parameters, Stream outputStream)
        {
            using (var original = new Bitmap(inputStream))
            {
                Resize(original, parameters, outputStream);
                return true;
            }
        }

        public virtual byte[] GetResizedBytes(Stream imageStream, ImageResizeParameters parameters)
        {
            using (var original = new Bitmap(imageStream))
            {
                var ms = new MemoryStream();
                Resize(original, parameters, ms);
                return ms.GetBuffer();
            }
        }

        public void Resize(Bitmap original, ImageResizeParameters parameters, Stream output)
        {
            Bitmap resized;
            var innerOriginal = (Bitmap)original.Clone();

            var mode = parameters.Mode;
            var maxWidth = parameters.MaxWidth;
            var maxHeight = parameters.MaxHeight;
            var quality = parameters.Quality;

            if (mode == ImageResizeMode.Fit)
            {
                var resizeRation = GetResizeRatio(innerOriginal, maxWidth, maxHeight);
                var newWidth = (int)Math.Round(innerOriginal.Width * resizeRation);
                var newHeight = (int)Math.Round(innerOriginal.Height * resizeRation);
                resized = new Bitmap(newWidth, newHeight, innerOriginal.PixelFormat);
            }
            else
                resized = new Bitmap((int)maxWidth, (int)maxHeight, innerOriginal.PixelFormat);

            resized.SetResolution(innerOriginal.HorizontalResolution, innerOriginal.VerticalResolution);

            if (parameters.Gamma.HasValue)
            {
                innerOriginal = AdjustGamma(innerOriginal, parameters.Gamma.Value);
            }

            if (parameters.Sharpen.HasValue)
            {
                Sharpen(innerOriginal, parameters.Sharpen.Value);
            }

            if (parameters.Contrast.HasValue)
            {
                innerOriginal = AdjustContrast(innerOriginal, parameters.Contrast.Value);
            }

            using (var g = Graphics.FromImage(resized))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;

                using (var attr = new ImageAttributes())
                {
                    attr.SetWrapMode(WrapMode.TileFlipXY);

                    var qm = new QColorMatrix();

                    if (parameters.Hue.HasValue)
                        qm.RotateHue((float)parameters.Hue.Value);

                    if (parameters.Saturation.HasValue)
                        qm.SetSaturation2((float)(parameters.Saturation.Value / 100.0f));

                    if (parameters.Brightness.HasValue)
                        qm.SetBrightness((float)(parameters.Brightness / 100.0f));

                    attr.ClearColorMatrix();
                    attr.SetColorMatrix(qm.ToColorMatrix());

                    var destinationFrame = mode == ImageResizeMode.Fill
                        ? GetFillDestinationRectangle(innerOriginal.Size, resized.Size)
                        : new Rectangle(Point.Empty, resized.Size);
                    g.DrawImage(innerOriginal, destinationFrame, 0, 0, innerOriginal.Width, innerOriginal.Height, GraphicsUnit.Pixel, attr);
                }

                // Use higher quality compression if the original image is jpg. Default is 75L.
                ImageCodecInfo codec = GetEncoderInfo(innerOriginal.RawFormat.Guid);

                if (codec != null && codec.MimeType.Equals("image/jpeg"))
                {
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    resized.Save(output, codec, encoderParams);
                }
                else
                {
                    resized.Save(output, innerOriginal.RawFormat);
                }
            }
        }

        public void Rotate(Bitmap original, RotateFlipType rotateFlipType, Stream output)
        {
            var returnedBitmap = (Bitmap)original.Clone();

            returnedBitmap.RotateFlip(rotateFlipType);

            returnedBitmap.Save(output, original.RawFormat);
        }

        public Bitmap Rotate(Bitmap original, RotateFlipType rotateFlipType)
        {
            using (var ms = new MemoryStream())
            {
                Rotate(original, rotateFlipType, ms);
                return new Bitmap(ms);
            }
        }

        private ImageCodecInfo GetEncoderInfo(Guid formatId)
        {
            // Get image codecs for all image formats
            var codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            return codecs.FirstOrDefault(t => t.FormatID == formatId);
        }

        private void Sharpen(Bitmap image, double amount = 0)
        {
            int filterWidth = 3;
            int filterHeight = 3;
            int width = image.Width;
            int height = image.Height;

            // Create sharpening filter.
            double[,] filter = new double[filterWidth, filterHeight];
            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0 + amount;
            double bias = 0;

            Color[,] result = new Color[image.Width, image.Height];

            // Lock image bits for read/write.
            BitmapData pbits = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Declare an array to hold the bytes of the bitmap.
            int bytes = pbits.Stride * height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

            int rgb;
            // Fill the color array with the new sharpened color values.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + width) % width;
                            int imageY = (y - filterHeight / 2 + filterY + height) % height;

                            rgb = imageY * pbits.Stride + 3 * imageX;

                            red += rgbValues[rgb + 2] * filter[filterX, filterY];
                            green += rgbValues[rgb + 1] * filter[filterX, filterY];
                            blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }

            // Update the image with the sharpened pixels.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rgb = y * pbits.Stride + 3 * x;

                    rgbValues[rgb + 2] = result[x, y].R;
                    rgbValues[rgb + 1] = result[x, y].G;
                    rgbValues[rgb + 0] = result[x, y].B;
                }
            }

            // Copy the RGB values back to the bitmap.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
            // Release image bits.
            image.UnlockBits(pbits);
        }

        private Bitmap AdjustContrast(Bitmap image, double contrast)
        {
            Bitmap adjustedImage = (Bitmap)image.Clone();

            float brightness = 1.0f; // no change in brightness
            float gamma = 1.0f; // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
                    new float[] {(float)contrast, 0, 0, 0, 0}, // scale red
                    new float[] {0, (float)contrast, 0, 0, 0}, // scale green
                    new float[] {0, 0, (float)contrast, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            var imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(image, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, imageAttributes);

            return adjustedImage;
        }

        private Bitmap AdjustGamma(Bitmap image, double gamma)
        {
            if (gamma == 1.0F)
                return image;

            var adjustedBitmap = (Bitmap)image.Clone();

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetGamma((float)gamma, ColorAdjustType.Bitmap);

            using (var g = Graphics.FromImage(adjustedBitmap))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, adjustedBitmap.Width, adjustedBitmap.Height)
                            , 0, 0, adjustedBitmap.Width, adjustedBitmap.Height,
                            GraphicsUnit.Pixel, imageAttributes);
            }

            return adjustedBitmap;
        }

        private Graphics CreateGraphics(Bitmap original, ref Bitmap resized)
        {
            ImageCodecInfo info = GetEncoderInfo(original.RawFormat.Guid);

            switch (info.MimeType)
            {
                case "image/jpeg":
                    if (resized.PixelFormat == PixelFormat.Format1bppIndexed || resized.PixelFormat == PixelFormat.Format4bppIndexed || resized.PixelFormat == PixelFormat.Format8bppIndexed)
                        return GetResizedBitmap(ref resized, PixelFormat.Format24bppRgb);
                    return Graphics.FromImage(resized);
                case "image/gif":
                    return GetResizedBitmap(ref resized, PixelFormat.Format24bppRgb);
                case "image/png":
                default:
                    return GetResizedBitmap(ref resized, original.PixelFormat);
            }
        }

        private Graphics GetResizedBitmap(ref Bitmap resized, PixelFormat format)
        {
            var temp = new Bitmap(resized.Width, resized.Height, format);
            var g = Graphics.FromImage(temp);
            g.DrawImage(resized, new Rectangle(0, 0, temp.Width, temp.Height), 0, 0, resized.Width, resized.Height,
                            GraphicsUnit.Pixel);
            return g;
        }

        public static Rectangle GetFillDestinationRectangle(Size original, Size resized)
        {
            var resizeWidth = resized.Width / (double)original.Width;
            var resizeHeight = resized.Height / (double)original.Height;
            var resizeMax = Math.Max(resizeWidth, resizeHeight);

            var size = new Size((int)(original.Width * resizeMax), (int)(original.Height * resizeMax));

            bool isBeeingMadeNarrower = resizeWidth < resizeHeight;

            var relocated = isBeeingMadeNarrower
                ? new Point((resized.Width - size.Width) / 2, 0)
                : new Point(0, (resized.Height - size.Height) / 2);

            return new Rectangle(relocated, size);
        }

        double GetResizeRatio(Bitmap original, double width, double height)
        {
            double ratioY = height / original.Height;
            double ratioX = width / original.Width;

            double ratio = Math.Min(ratioX, ratioY);
            if (ratio == 0)
                ratio = Math.Max(ratioX, ratioY);
            return ratio;
        }

        protected virtual void TransferBetweenStreams(Stream inputStream, Stream outputStream)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;

                outputStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}
