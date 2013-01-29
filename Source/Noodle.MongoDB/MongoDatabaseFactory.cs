using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Noodle.MongoDB
{
    public class MongoDatabaseFactory : Dictionary<string, Func<MongoDatabase>>, IMongoDatabaseFactory
    {
        public MongoDatabase CreateNew(string name)
        {
            return this[name]();
        }
    }
}
