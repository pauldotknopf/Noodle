using Noodle.Security.Users;

namespace Noodle.Security
{
    public interface ISecurityContext
    {
        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        User CurrentUser { get; set; }
    }
}
