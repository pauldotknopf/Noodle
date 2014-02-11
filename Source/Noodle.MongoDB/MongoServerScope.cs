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

        public MongoServerScope(int port) : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetRandomFileName()), port, DefaultMongoDbLocation())
        {
            
        }

        public MongoServerScope(string directory, int port, string mongoLocation)
        {
            _directory = directory;

            Monitor.Enter(_lockObject);

            Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);

            try
            {
                _directory = directory;

                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
                Directory.CreateDirectory(directory);

                var logPath = Path.Combine(directory, "dblog.log");
                var command = new Noodle.Process.ProcessCommand(mongoLocation, "--dbpath \"" + directory + "\" --port \"" + port + "\" --logpath \"" + logPath + "\" --logappend");
                _process = command.Run();
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
                if(!_process.HasExited)
                    _process.Kill();
                Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                Directory.Delete(_directory, true);
            }
            finally
            {
                Monitor.Exit(_lockObject);
            }
        }

        private static string DefaultMongoDbLocation()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mongod.exe");
        }
    }
}
