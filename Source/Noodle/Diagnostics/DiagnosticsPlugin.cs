namespace Noodle.Diagnostics
{
    public abstract class DiagnosticsPlugin
    {
        public abstract string DisplayName { get; }
        public abstract string BuildHtml();
    }
}
