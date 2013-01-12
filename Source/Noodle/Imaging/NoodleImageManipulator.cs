using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Image manipulator (TODO)
    /// </summary>
    public class NoodleImageManipulator : IImageManipulator
    {
        public bool Resize(System.IO.FileInfo image, ImageResizeParameters parameters, System.IO.FileInfo resizedImage)
        {
            throw new NotImplementedException("Not done yet!");
        }

        public bool Resize(System.IO.Stream inputStream, ImageResizeParameters parameters, System.IO.Stream outputStream)
        {
            throw new NotImplementedException("Not done yet!");
        }

        public void Resize(System.Drawing.Bitmap original, ImageResizeParameters parameters, System.IO.Stream output)
        {
            throw new NotImplementedException("Not done yet!");
        }

        public void Rotate(System.Drawing.Bitmap original, System.Drawing.RotateFlipType rotateFlipType, System.IO.Stream output)
        {
            throw new NotImplementedException("Not done yet!");
        }

        public System.Drawing.Bitmap Rotate(System.Drawing.Bitmap original, System.Drawing.RotateFlipType rotateFlipType)
        {
            throw new NotImplementedException("Not done yet!");
        }
    }
}
