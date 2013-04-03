using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Caching;
using Noodle.Engine;
using Noodle.Security.Users;

namespace Noodle.Security.Activity
{
    /// <summary>
    /// User activity service
    /// </summary>
    /// <remarks></remarks>
    public class UserActivityService : IUserActivityService
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypeFinder _typeFinder;
        private readonly TinyIoCContainer _container;
        private readonly MongoCollection<ActivityLogType> _activityLogTypeCollection;
        private readonly MongoCollection<ActivityLog> _activityLogCollection;
        private readonly MongoCollection<User> _userCollection;
        private readonly MongoCollection<ActivityLogTypeProviderInstalled> _installedActivityLogProviderCollection;

        #region Constants

        private const string ActivityTypePatternKey = "Noodle.activitytype.";

        #endregion

        #region Fields


        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="typeFinder">The type finder.</param>
        /// <param name="container">The container.</param>
        /// <param name="activityLogTypeCollection">The activity log type collection.</param>
        /// <param name="activityLogCollection">The activity log collection.</param>
        /// <param name="userCollection">The user collection.</param>
        /// <param name="installedActivityLogProviderCollection">The installed activity log provider collection.</param>
        public UserActivityService(ICacheManager cacheManager,
            ITypeFinder typeFinder,
            TinyIoCContainer container,
            MongoCollection<ActivityLogType> activityLogTypeCollection,
            MongoCollection<ActivityLog> activityLogCollection,
            MongoCollection<User> userCollection,
            MongoCollection<ActivityLogTypeProviderInstalled> installedActivityLogProviderCollection)
        {
            _cacheManager = cacheManager;
            _typeFinder = typeFinder;
            _container = container;
            _activityLogTypeCollection = activityLogTypeCollection;
            _activityLogCollection = activityLogCollection;
            _userCollection = userCollection;
            _installedActivityLogProviderCollection = installedActivityLogProviderCollection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type collection</returns>
        /// <remarks></remarks>
        public virtual IList<ActivityLogType> GetAllActivityTypes()
        {
            return _activityLogTypeCollection.FindAll().SetSortOrder(SortBy<ActivityLogType>.Ascending(x => x.Name)).ToList();
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="userId">The user to associate the activity with. If none is used, it will be gotten from security context if available</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        /// <remarks></remarks>
        public virtual void InsertActivity(string systemKeyword, string comment, ObjectId? userId = null, params object[] commentParams)
        {
            if (!userId.HasValue)
            {
                // try to get the user from the current context
                ISecurityContext securityContext;
                try
                {
                    securityContext = _container.Resolve<ISecurityContext>();
                }catch(Exception ex)
                {
                    throw new Exception(
                        "InsertActivity was called without a user specified. An attempt was made to get the current user from the ISecurityContext but the resolution failed. If you do not specify a user, make sure the security context is resolvable.",
                        ex);
                }
                var currentUser = securityContext.CurrentUser;
                userId = currentUser != null ? currentUser.Id : (ObjectId?)null;
            }

            if (!userId.HasValue || userId.Value == ObjectId.Empty)
                return;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            // ensure utc time
            var time = DateTime.SpecifyKind(CommonHelper.CurrentTime(), DateTimeKind.Utc);

            // get the log type
            var activityLogType = _activityLogTypeCollection.FindOne(Query.And(Query<ActivityLogType>.EQ(x => x.SystemKeyword, systemKeyword), Query<ActivityLogType>.EQ(x => x.Enabled, true)));
            if(activityLogType == null)
                return; // ignore this call if it doesn't exist or isn't enabled

            _activityLogCollection.Insert(new ActivityLog
            {
                ActivityLogTypeId = activityLogType.Id,
                Comment = comment,
                CreatedOnUtc = time,
                UserId = userId
            });
        }

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log id</param>
        /// <remarks></remarks>
        public virtual void DeleteActivity(ObjectId activityLogId)
        {
            _activityLogCollection.Remove(Query<ActivityLog>.EQ(x => x.Id, activityLogId));
        }

        /// <summary>
        /// Disable an activity log type
        /// </summary>
        /// <param name="systemKeyword">The system keyword of the activity log type.</param>
        public virtual void DisableActivityLogType(string systemKeyword)
        {
            _activityLogTypeCollection.Update(Query<ActivityLogType>.EQ(x => x.SystemKeyword, systemKeyword), Update<ActivityLogType>.Set(x => x.Enabled, false));
        }

        /// <summary>
        /// Enabled an activity log type
        /// </summary>
        /// <param name="systemKeyword">The system keyword of the activity log type.</param>
        public virtual void EnabledActivityLogType(string systemKeyword)
        {
            _activityLogTypeCollection.Update(Query<ActivityLogType>.EQ(x => x.SystemKeyword, systemKeyword), Update<ActivityLogType>.Set(x => x.Enabled, true));
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFromUtc">Log item creation from; null to load all users</param>
        /// <param name="createdOnToUtc">Log item creation to; null to load all users</param>
        /// <param name="email">User Email</param>
        /// <param name="username">User username</param>
        /// <param name="activityLogType">Activity log type system keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Activity log collection</returns>
        /// <remarks></remarks>
        public virtual PagedList<ActivityLog> GetAllActivities(DateTime? createdOnFromUtc = null,
            DateTime? createdOnToUtc = null, string email = "", string username = "", string activityLogType = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var filters = new List<IMongoQuery>();

            var userIds = new List<ObjectId>();

            if(!string.IsNullOrEmpty(username))
            {
                var usernameRegex = new Regex(username, RegexOptions.IgnoreCase);
                userIds.AddRange(_userCollection.Find(Query<User>.EQ(x => x.Username, BsonRegularExpression.Create(usernameRegex))).SetFields(Fields<User>.Include(x => x.Id)).Select(x => x.Id));
            }
            if(!string.IsNullOrEmpty(email))
            {
                var emailRegex = new Regex(email, RegexOptions.IgnoreCase);
                userIds.AddRange(_userCollection.Find(Query<User>.EQ(x => x.Email, BsonRegularExpression.Create(emailRegex))).SetFields(Fields<User>.Include(x => x.Id)).Select(x => x.Id));
            }

            if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email))
            {
                // we are searching for certain user ids
                filters.Add(Query<ActivityLog>.In(x => x.UserId, userIds.Select(x => (ObjectId?)x)));
            }

            if(!string.IsNullOrEmpty(activityLogType))
            {
                var activity = _activityLogTypeCollection.Find(Query<ActivityLogType>.EQ(x => x.SystemKeyword, activityLogType)).SetFields(Fields<ActivityLogType>.Include(x => x.Id)).FirstOrDefault();
                if(activity != null)
                    filters.Add(Query<ActivityLog>.EQ(x => x.ActivityLogTypeId, activity.Id));
            }

            if(createdOnFromUtc.HasValue)
                filters.Add(Query<ActivityLog>.GTE(x => x.CreatedOnUtc, createdOnFromUtc.Value));

            if(createdOnToUtc.HasValue)
                filters.Add(Query<ActivityLog>.LTE( x => x.CreatedOnUtc, createdOnToUtc.Value));

            var query = filters.Any() ? Query.And(filters) : null;

            var total = query != null ? _activityLogCollection.Count(query) : _activityLogCollection.Count();

            var logs = (query != null ? _activityLogCollection.Find(query) : _activityLogCollection.FindAll())
                .SetSkip(pageIndex * pageSize)
                .SetLimit(pageSize)
                .ToList();

            return new PagedList<ActivityLog>(logs, pageIndex, pageSize, (int)total);
        }

        /// <summary>
        /// Install all activity types specified by the activity log type provider
        /// </summary>
        /// <param name="activityLogTypeProvider">The activity log type provider.</param>
        /// <param name="reInstall">Should the provider be reinstalled (add new types or update existing)</param>
        /// <remarks></remarks>
        public void InstallActivityLogTypes(IActivityLogTypeProvider activityLogTypeProvider, bool reInstall = false)
        {
            var providerName = activityLogTypeProvider.GetType().Name;

            if (reInstall)
                _installedActivityLogProviderCollection.Remove(Query<ActivityLogTypeProviderInstalled>.EQ(x => x.Name, providerName));

            if(_installedActivityLogProviderCollection.Count(Query<ActivityLogTypeProviderInstalled>.EQ(x => x.Name, providerName)) > 0)
                return;

            _installedActivityLogProviderCollection.Insert(new ActivityLogTypeProviderInstalled { Name = providerName });

            foreach (var activityLogType in activityLogTypeProvider.GetActivityLogTypes())
            {
                var existing = _activityLogTypeCollection.FindOne(Query.And(Query<ActivityLogType>.EQ(x => x.SystemKeyword, activityLogType.SystemKeyword)));
                
                if(existing == null)
                    existing = new ActivityLogType();

                existing.SystemKeyword = activityLogType.SystemKeyword;
                existing.Name = activityLogType.Name;
                existing.Enabled = activityLogType.Enabled;

                if(existing.Id == ObjectId.Empty)
                    _activityLogTypeCollection.Insert(existing);
                else
                    _activityLogTypeCollection.Update(Query<ActivityLogType>.EQ(x => x.Id, existing.Id), Update<ActivityLogType>.Replace(existing));
            }
        }

        /// <summary>
        /// Uninstall the activity types specified by the activity log type provider
        /// </summary>
        /// <param name="activityLogTypeProvider">The activity log type provider.</param>
        /// <remarks></remarks>
        public void UninstallActivityLogTypes(IActivityLogTypeProvider activityLogTypeProvider)
        {
            throw new NotImplementedException("TODO");
        }

        /// <summary>
        /// Install all the activity log type providers found in the app domain
        /// </summary>
        /// <remarks></remarks>
        public void InstallFoundActivityLogTypeProviders()
        {
            foreach (var activityLogTypeProvider in _typeFinder.Find<IActivityLogTypeProvider>())
            {
                IActivityLogTypeProvider provider;
                try
                {
                    provider = Activator.CreateInstance(activityLogTypeProvider) as IActivityLogTypeProvider;
                }
                catch
                {
                    throw new Exception("Type {0} must have a empty constructer.".F(activityLogTypeProvider.FullName));
                }
                InstallActivityLogTypes(provider);
            }
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearAllActivities()
        {
            _activityLogCollection.RemoveAll();
        }

        #endregion
    }
}
