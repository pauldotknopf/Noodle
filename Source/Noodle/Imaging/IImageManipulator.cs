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
        /// Resize a file system image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="parameters"></param>
        /// <param name="resizedImage"></param>
        /// <returns></returns>
        bool Resize(FileInfo image, ImageResizeParameters parameters, FileInfo resizedImage);
        
        /// <summary>
        /// Resize an image in memory
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="parameters"></param>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        bool Resize(Stream inputStream, ImageResizeParameters parameters, Stream outputStream);
        
        /// <summary>
        /// Resize an image in memory with the Bitmap object
        /// </summary>
        /// <param name="original"></param>
        /// <param name="parameters"></param>
        /// <param name="output"></param>
        void Resize(Bitmap original, ImageResizeParameters parameters, Stream output);

        /// <summary>
        /// Rotate an image
        /// </summary>
        /// <param name="original"></param>
        /// <param name="rotateFlipType"></param>
        /// <param name="output"></param>
        void Rotate(Bitmap original, RotateFlipType rotateFlipType, Stream output);

        /// <summary>
        /// Rotate an image
        /// </summary>
        /// <param name="original"></param>
        /// <param name="rotateFlipType"></param>
        /// <returns></returns>
        Bitmap Rotate(Bitmap original, RotateFlipType rotateFlipType);
    }
}
