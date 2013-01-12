

namespace Noodle.Localization
{
    public class LocalizationSettings : ISettings
    {
        public LocalizationSettings()
        {
            LogResourcesNotFound = false;
        }

        /// <summary>
        /// Default admin area language identifier
        /// </summary>
        public int DefaultLanguageId { get; set; }

        /// <summary>
        /// A value indicating whether to load all records on application startup
        /// </summary>
        public bool LoadAllLocaleRecordsOnStartup { get; set; }

        /// <summary>
        /// Indicating if resources should be logged when not found.
        /// </summary>
        public bool LogResourcesNotFound { get; set; }
    }
}
