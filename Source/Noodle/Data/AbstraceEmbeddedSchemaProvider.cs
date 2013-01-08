using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Noodle.Data
{
    /// <summary>
    /// Classes implemented extending this clalss can delpoyed a schema file (dbschema, dpac) to a target database
    /// </summary>
    public abstract class AbstraceEmbeddedSchemaProvider
    {
        /// <summary>
        /// The cached location of the temp directory
        /// </summary>
        private DirectoryInfo _tempDirectory;

        /// <summary>
        /// Since providers may bring in references files to this directory,
        /// we need to keep track which ones belong to this particular provider
        /// </summary>
        protected List<string> DefaultFiles = new List<string>();

        /// <summary>
        /// The assembly containing the emebedded fiels
        /// </summary>
        /// <returns></returns>
        public abstract Assembly GetContainingAssembly();

        /// <summary>
        /// The resource name prefix.
        /// For example, "Noodle.Data.Settings" is the prefix for all the embedded resources files.
        /// The embedded resources wil have the prefix removed to determine that actual file name.
        /// </summary>
        public abstract string ResourceNamePrefix
        {
            get;
        }

        /// <summary>
        /// Publish the schema to the target connection string and database
        /// </summary>
        /// <param name="targetConnectionString"></param>
        /// <param name="targetDatabase"></param>
        public abstract void PublishTo(string targetConnectionString, string targetDatabase);

        /// <summary>
        /// Get a list of schemas that must be executed before this 
        /// </summary>
        /// <returns></returns>
        public virtual List<AbstraceEmbeddedSchemaProvider> GetDependentSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>();
        }

        /// <summary>
        /// The temporary directory that will store all the embedded deploy files
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo GetTempDirectory()
        {
            if (_tempDirectory == null)
            {
                _tempDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(),
                                                               GetType().Name + "-Schema-" + Path.GetFileNameWithoutExtension(Path.GetTempFileName())));
                SaveFilesToDirectory(_tempDirectory);
            }

            return _tempDirectory;
        }

        public void SaveFilesToDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                directory.Create();

            var resourceNamePrefix = ResourceNamePrefix;
            if (!resourceNamePrefix.EndsWith("."))
                resourceNamePrefix += ".";

            foreach (var resource in GetContainingAssembly().GetManifestResourceNames().Where(resource => resource.StartsWith(resourceNamePrefix)))
            {
                var destination = Path.Combine(directory.FullName, resource.Replace(resourceNamePrefix, ""));

                if (File.Exists(destination))
                    File.Delete(destination);

                using (Stream file = File.OpenWrite(destination))
                {
                    CommonHelper.CopyStream(GetContainingAssembly().GetManifestResourceStream(resource), file);
                }

                DefaultFiles.Add(destination);
            }

            foreach (var dependencies in GetDependentSchemaProviders().OfType<SqlPackageEmbeddedSchemaProvider>())
            {
                dependencies.SaveFilesToDirectory(directory);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is AbstraceEmbeddedSchemaProvider))
                return false;

            return obj.GetType().FullName.Equals(GetType().FullName);
        }

        public override int GetHashCode()
        {
            return GetType().FullName.GetHashCode();
        }
    }
}
