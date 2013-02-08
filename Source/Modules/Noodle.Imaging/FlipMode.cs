using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Horizontal and vertical flipping. Convertible to System.Drawing.RotateFlipType by casting.
    /// </summary>
    public enum FlipMode
    {
        /// <summary>
        /// No flipping
        /// </summary>
        None = 0,
        /// <summary>
        /// Flip horizontally
        /// </summary>
        X = 4,
        /// <summary>
        /// Flip vertically (identical to 180 degree rotation)
        /// </summary>
        Y = 6,
        /// <summary>
        /// Flip horizontally and vertically
        /// </summary>
        XY = 2
    }
}
