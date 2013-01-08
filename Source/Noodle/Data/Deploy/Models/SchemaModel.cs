using System.Collections.Generic;

namespace Noodle.Data.Deploy.Models
{
    public class SchemaModel
    {
        public SchemaModel(EmbeddedSchemaProviderAttribute schema)
        {
            plans = new List<SchemaModel>();
            if (schema.Name == null)
                schema.Name = schema.Decorates.Name;
            name = schema.Name;
            displayName = schema.DisplayName;
        }

        public string name { get; set; }

        public string displayName { get; set; }

        public bool hasAllSqlPackages { get; set; }

        public List<SchemaModel> plans { get; set; }
    }
}