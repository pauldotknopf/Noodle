using System.Collections.Generic;

namespace Noodle.Extensions.Documentation
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
