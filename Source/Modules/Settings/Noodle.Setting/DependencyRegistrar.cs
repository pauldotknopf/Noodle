using System;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Noodle.Engine;
using Noodle.MongoDB;
namespace Noodle.Settings
{
    public class SettingsDiscriminatorConvention : IDiscriminatorConvention
    {
        public string ElementName
        {
            get { return "Discriminator"; }
        }

        public Type GetActualType(BsonReader bsonReader, System.Type nominalType)
        {
            var currentBsonType = bsonReader.GetCurrentBsonType();
            if (bsonReader.State == BsonReaderState.Value)
            {
                if (currentBsonType == BsonType.Document)
                {
                    var bookmark = bsonReader.GetBookmark();
                    bsonReader.ReadStartDocument();
                    var type = nominalType;
                    if (bsonReader.FindElement(ElementName))
                    {
                        var discriminator = BsonSerializer.Deserialize<BsonString>(bsonReader).AsString;
                        try
                        {
                            if(discriminator == "Typed")
                            {
                                type = typeof(TypedSettings<>);

                                bsonReader.ReturnToBookmark(bookmark);
                                bsonReader.ReadStartDocument();
                                bsonReader.FindElement("Name");
                                var stringType = BsonSerializer.Deserialize<BsonString>(bsonReader).AsString;
                                type = type.MakeGenericType(Type.GetType(stringType));
                            }else if(discriminator == "Setting")
                            {
                                type = typeof(Setting);
                            }
                        }
                        catch (Exception ex)
                        {
                            type = typeof(Setting);
                        }
                    }
                    bsonReader.ReturnToBookmark(bookmark);
                    return type;
                }
            }
            return nominalType;
        }

        public BsonValue GetDiscriminator(System.Type nominalType, System.Type actualType)
        {
            if(actualType.IsGenericType && actualType.GetGenericTypeDefinition() == typeof(TypedSettings<>))
            {
                return "Typed";
            }
            if(actualType == typeof(Setting))
            {
                return "Setting";
            }
            throw new InvalidOperationException("Invalid settings type. " + actualType.FullName);
        }
    }

    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<ISettingService, SettingService>();
            container.Register(typeof (IConfigurationProvider<>), typeof (ConfigurationProvider<>));
            container.Register((context, p) => GetSettingsDatabase(context));
            container.Register((context, p) => GetSettingsDatabase(context).GetCollection<Setting>("Settings"));
           
            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (typeof(ISettings).IsAssignableFrom(e.Request.Type))
                {
                    var buildMethod = BuildSettingsMethod.MakeGenericMethod(e.Request.Type);
                    e.Result = buildMethod.Invoke(null, new object[] { container });
                }
            };

            try
            {
                BsonSerializer.RegisterDiscriminatorConvention(typeof(Setting), new SettingsDiscriminatorConvention());
            }catch(BsonSerializationException ex)
            {
                // ensure that we can call this multiple times in same app domain for unit tests
            }
        }

        public int Importance
        {
            get { return 0; }
        }

        public static MongoDatabase GetSettingsDatabase(TinyIoCContainer container)
        {
            return container.Resolve<IMongoService>().GetDatabase();
        }

        static readonly MethodInfo BuildSettingsMethod = typeof(DependencyRegistrar).GetMethod(
            "BuildSettings",
            BindingFlags.Static | BindingFlags.NonPublic);

        static TSettings BuildSettings<TSettings>(TinyIoCContainer container) where TSettings : ISettings, new()
        {
            try
            {
                return container.Resolve<IConfigurationProvider<TSettings>>().Settings;
            }
            catch (NoodleException ex)
            {
                // we want to avoid errors if database/connection strings haven't been setup
                if (!ex.Message.StartsWith("The default connection string name was"))
                    throw;

                return new TSettings();
            }
        }
    }
}
