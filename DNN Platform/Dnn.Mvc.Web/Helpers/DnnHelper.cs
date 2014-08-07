using System.Web.Mvc;
using System.Web.Routing;

using Dnn.Mvc.Web.Framework;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

namespace Dnn.Mvc.Web.Helpers
{
    public class DnnHelper
    {
        public DnnHelper(ViewContext viewContext, IViewDataContainer viewDataContainer) 
            : this(viewContext, viewDataContainer, RouteTable.Routes) 
        {
        }

        public DnnHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
        {
            Requires.NotNull("viewContext", viewContext);
            Requires.NotNull("viewDataContainer", viewDataContainer);
            Requires.NotNull("routeCollection", routeCollection);

            RouteCollection = routeCollection;
            ViewContext = viewContext;
            ViewDataContainer = viewDataContainer;
            ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
        }

        public ModuleInfo ActiveModule
        {
            get { return SiteContext.ActiveModuleRequest.Module; }
        }

        public TabInfo ActivePage
        {
            get { return SiteContext.ActivePage; }
        }

        public PortalInfo ActiveSite
        {
            get { return SiteContext.ActiveSite; }
        }

        public PortalAliasInfo ActiveSiteAlias
        {
            get { return SiteContext.ActiveSiteAlias; }
        }

        public RouteCollection RouteCollection { get; private set; }

        public SiteContext SiteContext
        {
            get { return ViewContext.HttpContext.GetSiteContext(); }
        }

        public UserInfo User
        {
            get { return SiteContext.User; }
        }

        public ViewContext ViewContext { get; private set; }

        public ViewDataDictionary ViewData { get; private set; }

        public IViewDataContainer ViewDataContainer { get; private set; }
    }
}