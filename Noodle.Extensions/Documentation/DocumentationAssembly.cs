using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Documentation
{
    public class DocumentationAssembly
    {
        public DocumentationAssembly()
        {
            Members = new List<DocumentationMember>();    
        }

        /// <summary>
        /// The assembly name the documentation file represents
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// A list of all the members in the assembly
        /// </summary>
        public List<DocumentationMember> Members { get; set; } 
    }
}
