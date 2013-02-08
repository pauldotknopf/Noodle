using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// A service responsible for building a layout used for image resizing
    /// </summary>
    public interface IImageLayoutBuilder
    {
        /// <summary>
        /// Build a layout for the source image given the resize settings
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="resizeSettings"></param>
        ImageLayout BuildLayout(Size sourceSize, ResizeSettings resizeSettings);
    }
}
