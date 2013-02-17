using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Represents a scope of a source bitmap
    /// </summary>
    public class ImageJob : IDisposable
    {
        private Bitmap _bitmap = null;

        /// <summary>
        /// The source image to be manipulated
        /// </summary>
        public object Source { get; protected set; }

        public ImageJob(object source)
        {
            if(source == null)
                throw new ArgumentNullException("source");

            Source = source;

            LoadBitmap();
        }

        /// <summary>
        /// The bitmap represented by the source object
        /// </summary>
        public Bitmap Bitmap { get { return _bitmap; } }

        public void Dispose()
        {
        }

        private void LoadBitmap()
        {
            // if the source it a bitmap, lets just use it
            if (Source is Bitmap)
            {
                _bitmap = Source as Bitmap;
                return;
            }
            // if the source is an image, lets just use it
            if (Source is Image)
            {
                _bitmap = new Bitmap((Image)Source);
                return;
            }

            // if the user passed to us a stream, don't dispose it as they probally intend on doing so at a later point in time
            var disposeStream = !(Source is Stream);

            string path = null;
            Stream s = null;

            try
            {
                s = GetStreamFromSource(Source, out path);
                if (s == null) throw new ArgumentException("Source may only be an instance of string, VirtualFile, Bitmap, Image, or Stream.", "Source");
                _bitmap = DecodeStream(s, path);
                if (_bitmap == null) throw new NoodleException("Failed to decode image.");
            }
            finally
            {
                // now, we can't dispose the stream if Bitmap is still using it. 
                if (_bitmap != null && _bitmap.Tag != null 
                    && _bitmap.Tag is BitmapTag && ((BitmapTag)_bitmap.Tag).Source == s)
                {
                    // and, it slooks like Bitmap is still using it.
                    s = null;
                }
                // dispose the stream if we opened it. If someone passed it to us, they're responsible.
                if (s != null && disposeStream)
                {
                    s.Dispose(); 
                    s = null;
                }

                // make sure the bitmap is tagged with its path.
                if (_bitmap != null && _bitmap.Tag == null && path != null) 
                    _bitmap.Tag = new BitmapTag(path);
            }
        }

        private Stream GetStreamFromSource(object source, out string path)
        {
            // app-relative path - converted to virtual path
            if (source is string)
            {
                path = source as string;
                // convert app-relative paths to VirtualFile instances
                if (path.StartsWith("~", StringComparison.OrdinalIgnoreCase))
                {
                    source = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(path);
                    if (source == null) throw new FileNotFoundException("The specified virtual file could not be found.", path);
                }
            }

            path = null;
            Stream s = null;

            // stream
            if (source is Stream)
            {
                s = (Stream)source;
            }
            
            // virtualFile
            else if (source is System.Web.Hosting.VirtualFile)
            {
                path = ((System.Web.Hosting.VirtualFile)source).VirtualPath;
                s = ((System.Web.Hosting.VirtualFile)source).Open();
            }
            else if (source is string)
            {
                path = (string)source;
                s = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else if (source is byte[])
            {
                s = new MemoryStream((byte[])source, 0, ((byte[])source).Length, false, true);
            }

            if (s != null && s.Length <= s.Position && s.Position > 0)
                throw new NoodleException("The source stream is at the end (have you already read it?). You must call stream.Seek(0, SeekOrigin.Begin); before re-using a stream, or use ImageJob with ResetSourceStream=true the first time the stream is read.");

            if (s != null && s.Length == 0)
                throw new NoodleException("Source stream is empty; it has a length of 0. No bytes, no data. We can't work with this.");

            return s;
        }

        private Bitmap DecodeStream(Stream stream, string optionalpath)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            var bitmap = new Bitmap(ms, true);
            bitmap.Tag = new BitmapTag(optionalpath, ms);
            return bitmap;
        }
    }
}
