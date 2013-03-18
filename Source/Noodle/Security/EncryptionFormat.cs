using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Security
{
    /// <summary>
    /// The type of encrpytion
    /// </summary>
    public enum EncryptionFormat
    {
        /// <summary>
        /// No encrpytion
        /// </summary>
        Clear,
        /// <summary>
        /// SH1
        /// </summary>
        SHA1,
        /// <summary>
        /// MD5
        /// </summary>
        MD5
    }
}
