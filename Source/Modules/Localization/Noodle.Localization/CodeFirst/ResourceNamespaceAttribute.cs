namespace Noodle.Localization.CodeFirst
{
    public class ResourceNamespaceAttribute : System.Attribute
    {
        public string ResourceNamespace { get; private set; }

        public ResourceNamespaceAttribute(string resourceNamespace)
        {
            ResourceNamespace = resourceNamespace;
        }
    }
}
