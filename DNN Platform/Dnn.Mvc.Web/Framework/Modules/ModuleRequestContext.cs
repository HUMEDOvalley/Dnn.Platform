using System.Web;
using System.Web.Routing;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class ModuleRequestContext
    {
        public ModuleInfo Module { get; set; }

        public ModuleApplication Application { get; set; }

        public RouteData RouteData { get; set; }

        public string ModuleRoutingUrl { get; set; }

        public HttpContextBase HttpContext { get; set; }
    }
}