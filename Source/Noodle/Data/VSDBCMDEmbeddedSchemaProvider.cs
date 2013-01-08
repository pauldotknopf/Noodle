using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Noodle.Process;

namespace Noodle.Data
{
    /// <summary>
    /// Classes implementing this can define embedded dbschemas that can be deployed easily.
    /// </summary>
    public abstract class VSDBCMDEmbeddedSchemaProvider : AbstraceEmbeddedSchemaProvider
    {
        /// <summary>
        /// The manifest file that is in the temporary directory that contains connection string and database name
        /// </summary>
        /// <returns></returns>
        public FileInfo GetManifest()
        {
            var file = GetTempDirectory().GetFiles("*.deploymanifest").FirstOrDefault();
            if(file == null)
            {
                throw new InvalidOperationException("Couldn't find delpoyment manifest.");
            }
            return file;
        }

        /// <summary>
        /// Get the dbschema file assoicated with this schema
        /// </summary>
        /// <returns></returns>
        public FileInfo GetSchema()
        {
            var manifestFile = GetManifest().FullName;
            var schema = GetTempDirectory().GetFiles(Path.GetFileNameWithoutExtension(manifestFile) + ".dbschema").FirstOrDefault();
            if (schema == null)
            {
                throw new InvalidOperationException("Couldn't find dbschema file.");
            }
            return schema;
        }

        /// <summary>
        /// Publish the dbschema to the target connection string and database
        /// </summary>
        /// <param name="targetConnectionString"></param>
        /// <param name="targetDatabase"></param>
        public override void PublishTo(string targetConnectionString, string targetDatabase)
        {
            var manifestFile = GetManifest().FullName;
            var arguements = string.Format("/a:Deploy /dd+ /ManifestFile:{0} /cs:\"{2}\" /p:TargetDatabase=\"{1}\"",
                              Path.GetFileName(manifestFile), targetDatabase, targetConnectionString);
            var process = new ProcessCommand(GetVSDBCMD(), arguements);
            process.ProcessStartModifier = (p) =>
                                               {
                                                   p.WorkingDirectory = Path.GetDirectoryName(manifestFile);
                                               };
            process.OnError += (sender, e) => Debug.WriteLine("vsdbcms Error: " + e.Data);
            process.OnOutput += (sender, e) => Debug.WriteLine("vsdbcms Output: " + e.Data);
            process.Timeout = 0;
            Trace.WriteLine(process.Invoke().Output);
        }

        

        /// <summary>
        /// The location of the visaul studio database command exe
        /// </summary>
        /// <returns></returns>
        public string GetVSDBCMD()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var vsdbcmd = Path.Combine(programFiles, "Microsoft Visual Studio 10.0\\vstsdb\\deploy\\vsdbcmd.exe");
            if(!File.Exists(vsdbcmd))
            {
                throw new InvalidOperationException("VSDBCMD couldn't be found.");
            }
            return vsdbcmd;
        }
    }
}
