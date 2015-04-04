namespace Noodle.Extensions.Email
{
    /// <summary>
    /// Email server connection settings
    /// </summary>
    public class EmailAccountSettings
    {
        /// <summary>
        /// Gets or sets an email host
        /// </summary>
        public virtual string Host { get; set; }

        /// <summary>
        /// Gets or sets an email port
        /// </summary>
        public virtual int Port { get; set; }

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
        /// </summary>
        public virtual bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
        public virtual bool UseDefaultCredentials { get; set; }
    }
}
