using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Noodle.Web;

namespace Noodle.Routing
{
    public class EmbeddedResourceRouteHandler : IRouteHandler
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;
        private string _contentType;

        public EmbeddedResourceRouteHandler(Assembly assembly, string resourceName, string contentType = null)
        {
            _assembly = assembly;
            _resourceName = resourceName;
            _contentType = contentType;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DelegateHttpHandler(context =>
            {
                using (var stream = _assembly.GetManifestResourceStream(_resourceName))
                {
                    var fileExtension = Path.GetExtension(_resourceName);
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            if (string.IsNullOrEmpty(_contentType))
                            {
                                switch (fileExtension)
                                {
                                    case ".js":
                                        _contentType = "text/javascript";
                                        break;
                                    case ".css":
                                        _contentType = "text/css";
                                        break;
                                    default:
                                        throw new HttpException((int)HttpStatusCode.InternalServerError, "The resource '{0}' doesn't have a content type defined and the extension is unknown.".F(_resourceName));
                                }
                            }

                            var content = reader.ReadToEnd();

                            context.Response.Write(content);
                            context.Response.ContentType = _contentType;
                            context.Response.SetOutputCache(DateTime.MaxValue);

                            return;
                        }
                    }
                    throw new HttpException((int)HttpStatusCode.NotFound, "404");
                }   
            });
        }
    }
}
