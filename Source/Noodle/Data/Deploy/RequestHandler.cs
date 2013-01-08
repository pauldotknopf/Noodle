using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using Noodle.Collections;
using Noodle.Data.Deploy.Models;
using Noodle.Engine;
using Noodle.Plugins;
using Noodle.Routing;
using Noodle.Web;

namespace Noodle.Data.Deploy
{
    [Service]
    public class RequestHandler : IStartupTask
    {
        private static RouteCollection _routes = new RouteCollection();
        private static List<ExpiringItem<DeployScriptsResponse.DeployScript>> _temporaryScripts = new List<ExpiringItem<DeployScriptsResponse.DeployScript>>();

        public void Execute()
        {
            _routes.Add(new Route("noodle/datadeploy", new DelegateRouteHandler(requestContext =>
            {
                var deploy = new Deploy();
                deploy.EmbeddedSchemaPoviders = EngineContext.Resolve<IPluginFinder>().GetPlugins<EmbeddedSchemaProviderAttribute>().ToList();
                deploy.Planner = EngineContext.Resolve<IEmbeddedSchemaPlanner>();
                return deploy;
            })));

            _routes.Add(BuildJsonDeployReport("noodle/datadeploy/deploy",
                (connectionString, schema) => EngineContext.Resolve<IDeployService>().Deploy(connectionString, schema)));
            _routes.Add(BuildJsonDeployReport("noodle/datadeploy/deployReport",
                (connectionString, schema) => EngineContext.Resolve<IDeployService>().DeployReport(connectionString, schema)));
            _routes.Add(BuildJsonDeployReport("noodle/datadeploy/deployScript", 
                (connectionString, schema) =>
                    {
                        var scripts = EngineContext.Resolve<IDeployService>().DeployScripts(connectionString, schema);
                        
                        // remove all temporary scripts for this schema
                        _temporaryScripts.RemoveAll(x => x.Item.Schema.Equals(schema, StringComparison.InvariantCultureIgnoreCase));

                        // add the new scripts
                        _temporaryScripts.AddRange(
                            scripts.Scripts
                            .Select(x => new ExpiringItem<DeployScriptsResponse.DeployScript>
                            (
                                x.Value, 
                                60, 
                                TimeUnit.Seconds, 
                                _temporaryScripts
                            ))
                        );

                        return scripts;
                    }));

            _routes.Add(new Route("noodle/datadeploy/deployScriptDownload/{key}", 
                new DelegateRouteHandler(requestContext => new DelegateHttpHandler(context =>
                {
                    if (!requestContext.RouteData.Values.ContainsKey("key"))
                        throw new HttpException((int)HttpStatusCode.InternalServerError, "you must provide a key (guid) to the download.");

                    Guid key;
                    if(!CommonHelper.GuidTryParse((string)requestContext.RouteData.Values["key"], out key))
                        throw new HttpException((int)HttpStatusCode.InternalServerError, "the key was not a valid guid");

                    var script = _temporaryScripts.SingleOrDefault(x => x.Item.Key == key);

                    if(script == null)
                        throw new HttpException((int)HttpStatusCode.InternalServerError, "there was no script found with the given key. maybe the download expired?");

                    context.Response.Write(script.Item.Script);
                    context.Response.ContentType = "text/plain";
                }))));


            EventBroker.Instance.PostResolveAnyRequestCache += (sender, e) =>
            {
                var httpApplication = sender as HttpApplication;
                if (httpApplication == null)
                    return;

                var url = new Url(httpApplication.Context.Request.RawUrl);

                if (!string.IsNullOrEmpty(url.Path) && url.Path.StartsWith("/noodle/datadeploy", StringComparison.InvariantCultureIgnoreCase))
                {
                    var httpContextBase = new HttpContextWrapper(httpApplication.Context);

                    var routeData = _routes.GetRouteData(httpContextBase);
                    if (routeData == null)
                        return;

                    httpApplication.Context.RemapHandler(routeData.RouteHandler.GetHttpHandler(new RequestContext(httpContextBase, routeData)));
                }
            };
        }

        public int Order
        {
            get { return 0; }
        }

        private Route BuildJsonDeployReport<T>(string url, Func<ConnectionString, string, T> result) where T: ActionResponse
        {
            return new Route(url, new DelegateRouteHandler(requestContext =>
            {
                var connectionString = new ConnectionString();
                connectionString.Bind(requestContext.HttpContext);
                var schema = requestContext.HttpContext.Request["schemaName"];
                return new JsonHttpHandler<T>(result(connectionString, schema));
            }));
        }
    }
}
