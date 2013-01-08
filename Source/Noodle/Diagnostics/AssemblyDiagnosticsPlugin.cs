using System.Text;
using Noodle.Engine;

namespace Noodle.Diagnostics
{
    [DiagnosticsPlugin]
    public class AssemblyDiagnosticsPlugin : DiagnosticsPlugin
    {
        private readonly ITypeFinder _typeFinder;

        public AssemblyDiagnosticsPlugin(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override string DisplayName
        {
            get { return "Assemblies"; }
        }

        public override string BuildHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<pre>");
            foreach (var assembly in _typeFinder.GetAssemblies())
            {
                sb.AppendLine(assembly.FullName);
            }
            sb.AppendLine("</pre>");
            return sb.ToString();
        }
    }
}
