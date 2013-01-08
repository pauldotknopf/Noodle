using System;
using System.IO;
using System.Linq;
using Noodle.Data.SqlPackage;
using Noodle.Process;
using Noodle.Serialization;

namespace Noodle.Data
{
    /// <summary>
    /// Classes inheriting this abstract class definie embedded resource files that can be delpoyed using SqlPackage.exe (as opposed to VSCBCMD.exe)
    /// </summary>
    public abstract class SqlPackageEmbeddedSchemaProvider : AbstraceEmbeddedSchemaProvider
    {
        /// <summary>
        /// The dacpac file that is in the temporary directory that contains everything we need to delpoy using sqlpackage.exe
        /// </summary>
        /// <returns></returns>
        public FileInfo GetDACPACFile(DirectoryInfo tempDirectoryOverride = null)
        {
            GetTempDirectory();
            var file = DefaultFiles.FirstOrDefault(x => x.EndsWith("dacpac", StringComparison.InvariantCultureIgnoreCase));
            if (file == null)
            {
                throw new InvalidOperationException("Couldn't find dacpac file.");
            }
            return new FileInfo(file);
        }

        public override void PublishTo(string targetConnectionString, string targetDatabase)
        {
            var connectionString = targetConnectionString + ";Initial Catalog=" + targetDatabase + ";";
            var dacpacFile = GetDACPACFile().FullName;

            var arguements = "/a:Publish /q:True /tcs:\"{0}\" /sf:{1}"
                .F(connectionString,
                Path.GetFileName(dacpacFile));

            var command = GetCommand(arguements, dacpacFile);
            command.Invoke();
        }

        /// <summary>
        /// Returns the xml location of the report
        /// </summary>
        /// <param name="targetConnectionString"></param>
        /// <param name="targetDatabase"></param>
        /// <returns></returns>
        public DeploymentReport DeployReport(string targetConnectionString, string targetDatabase)
        {
            var connectionString = targetConnectionString + ";Initial Catalog=" + targetDatabase + ";";
            var dacpacFile = GetDACPACFile().FullName;
            var xmlPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".xml");

            var arguements = "/a:DeployReport  /q:True /tcs:\"{0}\" /sf:{1} /OutputPath:\"{2}\" /p:TreatVerificationErrorsAsWarnings=true"
                .F(connectionString,
                Path.GetFileName(dacpacFile),
                xmlPath);

            var command = GetCommand(arguements, dacpacFile);
            command.Invoke();

            if (File.Exists(xmlPath))
            {
                var result = new XmlSerializer().Deserialize<DeploymentReport>(File.ReadAllText(xmlPath));
                File.Delete(xmlPath);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Build a sql delpoy script for a specified target connection
        /// </summary>
        /// <param name="targetConnectionString"></param>
        /// <param name="targetDatabase"></param>
        /// <returns></returns>
        public string DeployScript(string targetConnectionString, string targetDatabase)
        {
            var connectionString = targetConnectionString + ";Initial Catalog=" + targetDatabase + ";";
            var dacpacFile = GetDACPACFile().FullName;

            var sqlPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".sql");

            var arguements = "/a:Script  /q:True /tcs:\"{0}\" /sf:{1} /OutputPath:\"{2}\" /p:TreatVerificationErrorsAsWarnings=true"
                .F(connectionString,
                Path.GetFileName(dacpacFile),
                sqlPath);

            var command = GetCommand(arguements, dacpacFile);
            command.Invoke();

            if (File.Exists(sqlPath))
            {
                var result = File.ReadAllText(sqlPath);
                File.Delete(sqlPath);
                return result;
            }
            return null;
        }

        #region Helpers

        /// <summary>
        /// The location of the sqlpackage.exe
        /// </summary>
        /// <returns></returns>
        public string GetSqlPackage()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var vsdbcmd = Path.Combine(programFiles, "Microsoft SQL Server\\110\\DAC\\bin\\sqlpackage.exe");
            if (!File.Exists(vsdbcmd))
            {
                throw new InvalidOperationException("sqlpackage.exe could not be found. make sure you have sql server 2012 data tools installed.");
            }
            return vsdbcmd;
        }

        private ProcessCommand GetCommand(string arguements, string dacpacFile)
        {
            var process = new ProcessCommand(GetSqlPackage(), arguements)
            {
                ProcessStartModifier = (p) =>
                {
                    p.WorkingDirectory = Path.GetDirectoryName(dacpacFile);
                }
            };
            process.OnError += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (e.Data.StartsWith("*** This deployment may encounter errors during execution because"))
                    {
                        if(OnWarning != null)
                            OnWarning(e.Data);
                    }
                    else
                    {
                        if(OnError != null)
                            OnError(e.Data);
                    }
                }
            };
            process.OnOutput += (sender, e) =>
            {
                if (OnOutput != null && !string.IsNullOrEmpty(e.Data))
                    OnOutput(e.Data);
            };
            process.Timeout = 0;

            return process;
        }

        public Action<string> OnError { get; set; }
        public Action<string> OnWarning { get; set; }
        public Action<string> OnOutput { get; set; }

        #endregion
    }
}
