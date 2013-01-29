using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Noodle.MongoDB
{
    public interface IMongoDatabaseFactory
    {
        MongoDatabase CreateNew(string name);
    }
}
