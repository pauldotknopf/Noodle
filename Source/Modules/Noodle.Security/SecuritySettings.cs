using System.Collections.Generic;

namespace Noodle.Security
{
    public class SecuritySettings : ISettings
    {
        public SecuritySettings()
        {
            EncryptionKey = "3d2a6853542b242d4c317b446d";
        }

        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of allowed IP addresses
        /// </summary>
        public List<string> AllowedIpAddresses { get; set; }
    }
}
