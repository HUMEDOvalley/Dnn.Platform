using System.Web;
using System.Web.Routing;

using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Helpers;

using DotNetNuke.Entities.Tabs;

namespace Dnn.Mvc.Web.Routing
{
    public class WebFormsRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            SiteContext siteContext = httpContext.GetSiteContext();

            TabInfo page = siteContext.ActivePage;

            return (page.TabName == "WebForms Page");
        }
    }
}