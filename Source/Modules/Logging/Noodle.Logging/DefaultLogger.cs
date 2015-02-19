using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace Noodle.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public class DefaultLogger : ILogger
    {
        #region Fields

        private readonly TinyIoCContainer _container;
        private readonly MongoCollection<Log> _logCollection;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="logCollection">The log collection</param>
        public DefaultLogger(TinyIoCContainer container,
            MongoCollection<Log> logCollection)
        {
            _container = container;
            _logCollection = logCollection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="logId">Log id to delete</param>
        public virtual void DeleteLog(ObjectId logId)
        {
            if(logId == ObjectId.Empty)
                return;

            _logCollection.Remove(Query.EQ("_id", logId));
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public virtual void ClearLog()
        {
            _logCollection.RemoveAll();
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item collection</returns>
        public virtual IPagedList<Log> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, LogLevel? logLevel = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var filters = new List<IMongoQuery>();

            if(fromUtc.HasValue)
                filters.Add(Query.GTE("CreatedOnUtc", fromUtc.Value));

            if(toUtc.HasValue)
                filters.Add(Query.LTE("CreatedOnUtc", toUtc.Value));

            if(logLevel.HasValue)
                filters.Add(Query.EQ("LogLevel", logLevel.Value));

            if(!string.IsNullOrEmpty(message))
            {
                var regex = new Regex(message, RegexOptions.IgnoreCase);
                filters.Add(Query.Or(Query.EQ("ShortMessage", new BsonRegularExpression(regex)),
                    Query.EQ("FullMessage", new BsonRegularExpression(regex))));
            }
            
            var query = filters.Any() ? Query.And(filters) : null;

            var total = query != null ? _logCollection.Count(query) : _logCollection.Count();

            var logs = (query != null ? _logCollection.Find(query) : _logCollection.FindAll())
                .SetSkip(pageIndex * pageSize)
                .SetLimit(pageSize)
                .ToList();

            return new PagedList<Log>(logs, pageIndex, pageSize, (int)total);
        }

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual Log GetLogById(ObjectId logId)
        {
            if (logId == ObjectId.Empty)
                return null;

            return _logCollection.FindOne(Query.EQ("_id", logId));
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="exception">The error associated with this log</param>
        /// <param name="user">The user to associate log record with</param>
        /// <returns>A log item</returns>
        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Exception exception = null, string user = null)
        {
            var log = new Log()
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage ?? string.Empty,
                FullMessage = fullMessage ?? string.Empty,
                User = user,
                CreatedOnUtc = CommonHelper.CurrentTime()
            };

            var ex = exception;
            if (ex != null)
            {
                if (string.IsNullOrEmpty(shortMessage))
                {
                    log.ShortMessage = ex.Message;
                }
                else
                {
                    log.ShortMessage += Environment.NewLine + ex.Message;
                }
                
                log.FullMessage += Environment.NewLine + "Full Trace:" + Environment.NewLine + ex.StackTrace;
            }

            // recursively add all the inner exceptions to the log
            ExceptionInfo currentExceptionInfo = null;
            while (ex != null)
            {
                var exceptionInfo = new ExceptionInfo();
                exceptionInfo.Message = ex.Message;
                exceptionInfo.ExceptionType = ex.GetType().FullName;
                exceptionInfo.StackTrace = ex.StackTrace;

                if (currentExceptionInfo == null)
                {
                    log.ExceptionInfo = exceptionInfo;
                }
                else
                {
                    currentExceptionInfo.InnerException = exceptionInfo;
                }

                currentExceptionInfo = exceptionInfo;
                ex = ex.InnerException;
            }

            foreach (var customData in LogStore.GetCustomData(ex ?? new LogException(logLevel, shortMessage, fullMessage), _container))
            {
                log.CustomData.Add(customData.Key, customData.Value);
            }

            _logCollection.Insert(log);

            return log;
        }

        #endregion
    }
}
