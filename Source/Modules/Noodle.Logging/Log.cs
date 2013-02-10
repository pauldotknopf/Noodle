using System;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson;

namespace Noodle.Logging
{
    /// <summary>
    /// Represents a log record
    /// </summary>
    public class Log : BaseEntity<ObjectId>
    {
        public Log()
        {
            CustomData = new Dictionary<string, string>();
            ServerVariables  = new Dictionary<string, string>();
            QueryString = new Dictionary<string, string>();
            Form = new Dictionary<string, string>();
            Cookies = new Dictionary<string, string>();
        }

        private string _customDataSerialized = string.Empty;
        private string _serverVariablesSerialized = string.Empty;
        private string _queryStringSerialized = string.Empty;
        private string _formSerialized = string.Empty;
        private string _cookiesSerialized = string.Empty;
        private IDictionary<string, string> _customData = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the log level identifier
        /// </summary>
        public virtual int LogLevelId { get; set; }

        /// <summary>
        /// Gets or sets the short message
        /// </summary>
        public virtual string ShortMessage { get; set; }

        /// <summary>
        /// Gets or sets the full exception
        /// </summary>
        public virtual string FullMessage { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public virtual string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets user if available
        /// </summary>
        public virtual string User { get; set; }

        /// <summary>
        /// Gets or sets the page URL
        /// </summary>
        public virtual string PageUrl { get; set; }

        /// <summary>
        /// Gets or sets the referrer URL
        /// </summary>
        public virtual string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public virtual LogLevel LogLevel { get { return (LogLevel)this.LogLevelId; } set { LogLevelId = (int)value; } }

        /// <summary>
        /// Custom data associated to this log
        /// </summary>
        public Dictionary<string, string> CustomData { get; protected set; }

        /// <summary>
        /// The server variables associated to the log
        /// </summary>
        public Dictionary<string, string> ServerVariables { get; protected set; }

        /// <summary>
        /// The query string associated to the log
        /// </summary>
        public Dictionary<string, string> QueryString { get; protected set; }

        /// <summary>
        /// The form values associated to this log
        /// </summary>
        public Dictionary<string, string> Form { get; protected set; }

        /// <summary>
        /// The cookies associated to the log.
        /// </summary>
        public Dictionary<string, string> Cookies { get; protected set; }
    }
}
