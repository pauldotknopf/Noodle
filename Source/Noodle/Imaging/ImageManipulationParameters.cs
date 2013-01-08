namespace Noodle.Imaging
{
    public class ImageManipulationParameters
    {
        public ImageManipulationParameters()
        {
            //Brightness = 1;
            //Contrast = 1;
            //Gamma = 1;

            //Hue = 0;
            //Saturation = 0;

            //Sharpen = 0;
        }

        public double? Brightness { get; set; }
        public double? Contrast { get; set; }
        public double? Gamma { get; set; }

        public double? Hue { get; set; }
        public double? Saturation { get; set; }

        public double? Sharpen { get; set; }
    }
}
