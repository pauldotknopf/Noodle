using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Noodle.MongoDB
{
    /// <summary>
    /// This creates (and then kills) a mongo db server instance for testing.
    /// </summary>
    public class MongoServerScope : IDisposable
    {
        private static object _lockObject = new object();
        private readonly string _directory;
        private readonly System.Diagnostics.Process _process = null;

        public MongoServerScope() :this(8989){ }

        public MongoServerScope(int port) : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetRandomFileName()), port)
        {
            
        }

        public MongoServerScope(string directory, int port)
        {
            _directory = directory;
            Trace.WriteLine(directory);

            Monitor.Enter(_lockObject);
            try
            {
                _directory = directory;
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
                Directory.CreateDirectory(directory);
                var mongoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mongod.exe");
                Trace.WriteLine(mongoPath);
                _process = System.Diagnostics.Process.Start(mongoPath, "--dbpath \"" + directory + "\" --port \"" + port + "\"");
            }
            catch (Exception)
            {
                Monitor.Exit(_lockObject);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _process.Kill();
                System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                Directory.Delete(_directory, true);
            }
            finally
            {
                Monitor.Exit(_lockObject);
            }
        }
    }
}
