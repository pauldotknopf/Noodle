using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Noodle.Settings
{
    /// <summary>
    /// Some extension methods for FileInfo
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Is the file available for writing?
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileAccess"></param>
        /// <returns></returns>
        public static bool IsFileAvailable(this FileInfo file, FileAccess fileAccess = FileAccess.ReadWrite)
        {
            try
            {
                if (!file.Exists) return false;
                using (var fs = File.Open(file.FullName, FileMode.Open, fileAccess, FileShare.None)) { };
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Wait for a file to become available with a given timeout
        /// </summary>
        /// <param name="file"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool WaitForFile(this FileInfo file, double milliseconds)
        {
            var timer = new Timer(milliseconds);
            var isTimedOut = false;

            timer.Start();
            timer.Elapsed += (sender, e) =>
            {
                isTimedOut = true;
            };

            while (!IsFileAvailable(file))
            {
                if (isTimedOut)
                    return false;
                Thread.Sleep(100);
            }

            return true;
        }
    }
}
