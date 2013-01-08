using System.Web;
using Noodle.Serialization;

namespace Noodle.Web
{
    public class JsonHttpHandler<T> : IHttpHandler
    {
        private readonly T _value;

        public JsonHttpHandler(T value)
        {
            _value = value;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(new JavaScriptSerializer().Serialize(_value));
        }
    }
}
