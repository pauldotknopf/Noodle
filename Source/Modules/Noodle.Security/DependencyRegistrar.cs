using MongoDB.Driver;
using Noodle.Engine;
using Noodle.MongoDB;
using Noodle.Security.Activity;
using Noodle.Security.Permissions;
using Noodle.Security.Users;
using Noodle.Web;
namespace Noodle.Security
{
    /// <summary>
    /// Dependency register for security services
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register your services with the container. You are given a type finder to help you find anything you need.
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Register(TinyIoCContainer container)
        {
            container.Register<IPermissionService, PermissionService>();
            container.Register<IUserService, UserService>();
            container.Register<IUserActivityService, UserActivityService>();
            container.Register<ISecurityContext, SecurityContext>().AsPerRequestSingleton();
            container.Register<IAuthenticationService, FormsAuthenticationService>().AsPerRequestSingleton();
            // Noodle.dll has a limited functionality security manager, lets rebind it with a more advanced one (db-backed).
            container.Register<ISecurityManager, SecurityManager>();

            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<ActivityLog>("ActivityLogs"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<ActivityLogType>("ActivityLogTypes"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<ActivityLogTypeProviderInstalled>("ActivityLogTypeProvidersInstalled"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<PermissionRecord>("PermissionRecords"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<RolePermissionMap>("RolePermissionMaps"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<PermissionInstalled>("PermissionProvidersInstalled"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<User>("Users"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<UserRole>("UserRoles"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<UserUserRoleMap>("UserRoleMaps"));
        }

        public int Importance
        {
            get { return 0; }
        }

        public MongoDatabase GetLocalizationDatabase(TinyIoCContainer container)
        {
            return container.Resolve<IMongoService>().GetDatabase();
        }
    }
}
