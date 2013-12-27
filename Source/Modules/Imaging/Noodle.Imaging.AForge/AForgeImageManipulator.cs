using System.Drawing;
using AForge.Imaging.Filters;

namespace Noodle.Imaging.AForge
{
    /// <summary>
    /// An aforge implementation of an image manipulator
    /// </summary>
    public class AForgeImageManipulator : NoodleImageManipulator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AForgeImageManipulator"/> class.
        /// </summary>
        public AForgeImageManipulator(IImageLayoutBuilder imageLayoutBuilder)
            :base(imageLayoutBuilder)
        {
        }

        /// <summary>
        /// Manipulate (and resize) an image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Bitmap Manipulate(object source, ImageManipulationSettings parameters)
        {
            var image = Resize(source, parameters);

            var filters = new FiltersSequence();

            if (parameters.Gamma.HasValue)
                filters.Add(new GammaCorrection(parameters.Gamma.Value));

            if (parameters.Sharpen.HasValue)
                filters.Add(new Sharpen { Threshold = (int)parameters.Sharpen.Value });

            if (parameters.Hue.HasValue)
                filters.Add(new HueModifier((int)parameters.Hue.Value));

            if (parameters.Saturation.HasValue)
                filters.Add(new SaturationCorrection((float)parameters.Saturation.Value));

            if (parameters.Brightness.HasValue)
                filters.Add(new BrightnessCorrection((int)parameters.Brightness.Value));

            if (parameters.Contrast.HasValue)
                filters.Add(new ContrastCorrection((int)parameters.Contrast.Value));

            return filters.Count == 0 
                ? image 
                : filters.Apply(image);
        }
    }
}
