using System.Drawing;
using System.IO;

namespace Noodle.Imaging
{
    /// <summary>
    /// Tool for manipulating images
    /// </summary>
    public interface IImageManipulator
    {
        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Bitmap Resize(object source, ResizeSettings parameters);

        /// <summary>
        /// Manipulate (and resize) an image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Bitmap Manipulate(object source, ImageManipulationSettings parameters);
    }
}
