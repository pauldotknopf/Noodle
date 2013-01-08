using System.Collections.Generic;
using System.Web;

namespace Noodle.Data.Deploy.Models
{
    /// <summary>
    /// A connection string model that also builds connection strings
    /// </summary>
    public class ConnectionString
    {
        public ConnectionString()
        {
            server = "localhost\\sqlexpress";
            database = "noodle";
        }

        public string server { get; set; }
        public string database { get; set; }
        public bool useIntegratedSecurity { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            if(string.IsNullOrEmpty(server))
                errors.Add("The server is required.");

            if (string.IsNullOrEmpty(database))
                errors.Add("The database is required.");

            if(!useIntegratedSecurity)
            {
                if (string.IsNullOrEmpty(userName))
                    errors.Add("The username is required.");

                if (string.IsNullOrEmpty(password))
                    errors.Add("The password is required.");
            }

            return errors;
        }

        public void Bind(HttpContextBase context)
        {
            server = context.Request["connectionString[server]"];
            database = context.Request["connectionString[database]"];
            useIntegratedSecurity = bool.Parse(context.Request["connectionString[useIntegratedSecurity]"]);
            userName = context.Request["connectionString[userName]"];
            password = context.Request["connectionString[password]"];
        }

        public string BuildConnectionString(bool withDatabase)
        {
            var segments = new List<string>();

            segments.Add("Data Source=" + server);

            if (useIntegratedSecurity)
            {
                segments.Add("Trusted_Connection=True");
            }
            else
            {
                segments.Add("User Id=" + userName);
                segments.Add("Password=" + password);
            }

            if(withDatabase)
            {
                segments.Add("Initial Catalog=" + database);
            }

            return string.Join(";", segments.ToArray()) + ";";
        }
    }
}