using System;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Web;
using SimpleInjector;

namespace Noodle.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public class DefaultLogger : ILogger
    {
        #region Fields

        private readonly Container _container;
        private readonly MongoCollection<Log> _logCollection;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="logCollection">The log collection</param>
        public DefaultLogger(Container container,
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
            //using (var response = _logRepository
            //    .ExecuteMethodQuery<LogDataContext, IMultipleResults>(context
            //        => context.GetAllLogs(fromUtc, toUtc, message, (int?)logLevel, pageIndex, pageSize)))
            //{
            //    var logs = response.GetResult<Log>().ToList();
            //    var total = response.GetResult<Single<int>>().First().Value;

            //    return new PagedList<Log>(logs, pageIndex, pageSize, total);
            //}

            //CREATE PROCEDURE [dbo].[Log_GetAllLogs]
            //    @fromUtc	DATETIME = NULL,
            //    @toUtc		DATETIME = NULL,
            //    @message	NVARCHAR(MAX) = NULL,
            //    @logLevelId INT = NULL,
            //    @pageIndex	INT = 0,
            //    @pageSize	INT
            //AS

            //-- ensure valid page index
            //IF(@pageIndex = NULL OR @pageIndex < 0)
            //BEGIN
            //    SET @pageIndex = 0
            //END

            //SET @message = LTRIM(RTRIM(@message))

            //DECLARE @tmpTable TABLE (
            //    PK INT NOT NULL PRIMARY KEY,
            //    RowNum INT NOT NULL
            //)

            //INSERT INTO @tmpTable
            //SELECT l.Id, ROW_NUMBER() OVER(ORDER BY l.CreatedOnUtc DESC) AS RowNum
            //    FROM [dbo].[Log] AS l
            //    WHERE (@fromUtc IS NULL OR l.[CreatedOnUtc] >= @fromUtc)
            //        AND (@toUtc IS NULL OR l.[CreatedOnUtc] <= @toUtc)
            //        AND (@logLevelId IS NULL OR l.[LogLevelId] = @logLevelId)
            //        AND (@message IS NULL OR @message = '' OR l.[FullMessage] LIKE '%' + @message + '%' OR l.[ShortMessage] LIKE '%' + @message + '%')
            //    ORDER BY l.CreatedOnUtc

            //SELECT *
            //    FROM @tmpTable AS temp
            //    INNER JOIN [dbo].[Log] AS qe ON qe.Id = temp.PK
            //    WHERE RowNum > (@pageIndex * @pageSize) AND RowNum <= ((@pageIndex + 1) * @pageSize)
            //    ORDER BY RowNum

            //SELECT COUNT(*) as TotalCount FROM @tmpTable


            return null;
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
        /// <param name="requestContext">The request context. If this is supplied, additional info will be logged, like ip, post/server variables, etc</param>
        /// <returns>A log item</returns>
        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Exception exception = null, string user = null, IRequestContext requestContext = null)
        {
            Exception ex = null;

            if (exception != null)
            {
                if (IsBuiltInException(exception))
                {
                    ex = exception.GetBaseException();
                }
            }

            var log = new Log()
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage ?? string.Empty,
                FullMessage = fullMessage ?? string.Empty,
                User = user,
                CreatedOnUtc = CommonHelper.CurrentTime()
            };

            if (exception != null)
            {
                log.FullMessage += Environment.NewLine + "Full Trace:" + Environment.NewLine + exception.StackTrace;
            }

            foreach (var customData in LogStore.GetCustomData(ex ?? new LogException(logLevel, shortMessage, fullMessage), _container))
            {
                log.CustomData.Add(customData.Key, customData.Value);
            }

            // add request context stuff to the property bags
            #region
            if (requestContext != null)
            {
                log.IpAddress = requestContext.GetCurrentIpAddress();
                log.PageUrl = requestContext.Url.ToString();
                log.ReferrerUrl = requestContext.GetReferrerUrl();
                var serverVariables = requestContext.ServerVariables;
                var queryString = requestContext.QueryString;
                var form = requestContext.Form;
                var cookies = requestContext.Cookies;
                foreach (string key in serverVariables)
                {
                    log.ServerVariables.Add(key, serverVariables[key]);
                }
                foreach (string key in queryString)
                {
                    log.QueryString.Add(key, queryString[key]);
                }
                foreach (string key in form)
                {
                    log.Form.Add(key, form[key]);
                }
                foreach (HttpCookie cookie in cookies)
                {
                    if (!log.Cookies.ContainsKey(cookie.Name))
                    {
                        log.Form.Add(cookie.Name, cookie.Value);
                    }
                }
            }
            #endregion

            _logCollection.Insert(log);

            return log;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// returns if the type of the exception is built into .Net core
        /// </summary>
        /// <param name="e">The exception to check</param>
        /// <returns>True if the exception is a type from within the CLR, false if it's a user/third party type</returns>
        private bool IsBuiltInException(Exception e)
        {
            return e.GetType().Module.ScopeName == "CommonLanguageRuntimeLibrary";
        }

        #endregion
    }
}
