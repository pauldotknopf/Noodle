using System;
using System.Collections.Generic;
using System.Linq;
using Noodle.Data.Deploy.Models;
using Noodle.Plugins;

namespace Noodle.Data.Deploy
{
    public class DeployService : IDeployService
    {
        private readonly IPluginFinder _pluginFinder;

        public DeployService(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
        }

        public DeployReportResponse DeployReport(ConnectionString connectionString, string schemaName)
        {
            var response = new DeployReportResponse();

            response.errors.AddRange(connectionString.GetValidationErrors());

            if (!response.successful)
                return response;

            var plans = GetPlansFor(schemaName);

            foreach (var plan in plans)
            {
                if (response.errors.Any())
                    continue;

                var schema = Activator.CreateInstance(plan.Decorates) as SqlPackageEmbeddedSchemaProvider;
                schema.OnError = (error) => response.errors.Add(error);

                var report = schema.DeployReport(connectionString.BuildConnectionString(false), connectionString.database);

                if (report != null)
                {
                    // if a report was generated and errors were outputed, clear them because they will exist on the report
                    response.errors.Clear();
                    response.DeploymentReports.Add(plan.Name, report);
                }

                if (!response.errors.Any() && report == null)
                    response.errors.Add("SqlPackage had a problem generating the report.");
            }

            return response;
        }

        public DeployResponse Deploy(ConnectionString connectionString, string schemaName)
        {
            var response = new DeployResponse();

            response.errors.AddRange(connectionString.GetValidationErrors());

            if (!response.successful)
                return response;

            var plans = GetPlansFor(schemaName);

            foreach (var plan in plans)
            {
                if (response.errors.Any())
                    continue;

                var planName = plan.Name;
                response.Output.Add(planName, new List<string>());

                var schema = Activator.CreateInstance(plan.Decorates) as SqlPackageEmbeddedSchemaProvider;

                schema.OnError = (error) => response.errors.Add(error);
                schema.OnOutput = (output) => response.Output[planName].Add(output);

                schema.PublishTo(connectionString.BuildConnectionString(false), connectionString.database);
            }

            return response;
        }

        public DeployScriptsResponse DeployScripts(ConnectionString connectionString, string schemaName)
        {
            var response = new DeployScriptsResponse();

            response.errors.AddRange(connectionString.GetValidationErrors());

            if (!response.successful)
                return response;

            var plans = GetPlansFor(schemaName);

            foreach (var plan in plans)
            {
                if (response.errors.Any())
                    continue;

                var schema = Activator.CreateInstance(plan.Decorates) as SqlPackageEmbeddedSchemaProvider;
                schema.OnError = (error) => response.errors.Add(error);

                var script = schema.DeployScript(connectionString.BuildConnectionString(false), connectionString.database);

                if (script == null && !response.errors.Any())
                    response.errors.Add("The was a problem generating the deploy scripts.");

                if (script != null)
                {
                    response.Scripts.Add(plan.Name,
                        new DeployScriptsResponse.DeployScript
                        {
                            ForSchema = schemaName,
                            Schema = plan.Name,
                            Key = Guid.NewGuid(),
                            Script = script
                        });
                }
            }

            return response;
        }

        private IEnumerable<EmbeddedSchemaProviderAttribute> GetPlansFor(string schemaName)
        {
            var embeddedSchema = _pluginFinder.GetPlugins<EmbeddedSchemaProviderAttribute>().First(x => x.Name.Equals(schemaName));
            return EngineContext.Resolve<IEmbeddedSchemaPlanner>().GetPlansFor(embeddedSchema);
        }
    }
}
