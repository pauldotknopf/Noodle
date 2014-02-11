using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Controls whether the image is allowed to upscale, downscale, both, or if only the canvas gets to be upscaled.
    /// </summary>
    public enum ScaleMode
    {
        /// <summary>
        /// The default. Only downsamples images - never enlarges. If an image is smaller than 'width' and 'height', the image coordinates are used instead.
        /// </summary>
        [EnumString("down")]
        DownscaleOnly,
        /// <summary>
        /// Only upscales (zooms) images - never downsamples. If an image is larger than 'width' and 'height', the image coordinates are used instead.
        /// </summary>
        [EnumString("up")]
        UpscaleOnly,
        /// <summary>
        /// Upscales and downscales images according to 'width' and 'height'.
        /// </summary>
        Both,
        /// <summary>
        /// When the image is smaller than the requested size, padding is added instead of stretching the image.
        /// </summary>
        [EnumString("canvas")]
        UpscaleCanvas
    }
}
