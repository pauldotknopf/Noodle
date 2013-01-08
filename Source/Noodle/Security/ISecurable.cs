namespace Noodle.Security
{
    /// <summary>
    /// Plugins implementing this interface can have define security requirements
    /// </summary>
    public interface ISecurable : ISecurableBase
    {
        string[] RequiredRoles { get; set; }
    }
}
