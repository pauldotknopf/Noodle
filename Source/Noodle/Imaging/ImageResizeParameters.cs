namespace Noodle.Imaging
{
    /// <summary>
    /// Parameters for the ImageResizer
    /// </summary>
    public class ImageResizeParameters : ImageManipulationParameters
    {
        public ImageResizeParameters(double maxWidth, double maxHeight)
            : this(maxWidth, maxHeight, ImageResizeMode.Fit)
        {
        }

        public ImageResizeParameters(double maxWidth, double maxHeight, ImageResizeMode mode)
        {
            Mode = mode;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Quality = 90;
        }

        public ImageResizeMode Mode { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public int Quality { get; set; }
    }
}
