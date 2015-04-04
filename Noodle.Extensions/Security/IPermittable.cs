namespace Noodle.Extensions.Security
{
    /// <summary>
    /// When implemented by a plugin this attribute controls who may use it.
    /// </summary>
    public interface IPermittable : ISecurableBase
    {
        /// <summary>The permissions required.</summary>
        string[] RequiredPermissions { get; set; }
    }
}
