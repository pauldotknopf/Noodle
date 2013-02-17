using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Tag associated to bitmaps for tracking the streams used by the bitmap
    /// </summary>
    public class BitmapTag
    {
        public BitmapTag(object tag)
        {
            Source = null;
            Path = null;
            if(tag == null)
                throw new ArgumentNullException("tag");

            var s = tag as string;
            if (s != null) Path = s;
            var bitmapTag = tag as BitmapTag;
            if (bitmapTag == null) return;
            Path = bitmapTag.Path;
            Source = bitmapTag.Source;
        }

        public BitmapTag(string path, Stream source)
        {
            Path = path;
            Source = source;
        }

        public string Path { get; set; }
        public Stream Source { get; set; }
    }
}
