using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// How to resolve aspect ratio differences between the requested size and the original image's size.
    /// </summary>
    public enum FitMode
    {
        /// <summary>
        /// Fit mode will be determined by other settings, such as carve=true, stretch=fill, and crop=auto. If none are specified and width/height are specified , mode=pad will be used. If maxwidth/maxheight are used, mode=max will be used.
        /// </summary>
        None,
        /// <summary>
        /// Width and height are considered maximum values. The resulting image may be smaller to maintain its aspect ratio. The image may also be smaller if the source image is smaller
        /// </summary>
        Max,
        /// <summary>
        /// Width and height are considered exact values - padding is used if there is an aspect ratio difference. Use anchor to override the MiddleCenter default.
        /// </summary>
        Pad,
        /// <summary>
        /// Width and height are considered exact values - cropping is used if there is an aspect ratio difference. Use anchor to override the MiddleCenter default.
        /// </summary>
        Crop,
        /// <summary>
        /// Width and height are considered exact values - if there is an aspect ratio difference, the image is stretched.
        /// </summary>
        Stretch,
    }
}
