using System.Web.Mvc;
using Dnn.Mvc.Web.Helpers;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public abstract class DnnControllerBase : Controller
    {
        protected internal virtual ResourceNotFoundResult ResourceNotFound()
        {
            return new ResourceNotFoundResult();
        }

        protected internal virtual ResourceNotFoundResult ResourceNotFound(string viewName)
        {
            Requires.NotNullOrEmpty("viewName", viewName);
            return ResourceNotFound(View(viewName));
        }

        protected internal virtual ResourceNotFoundResult ResourceNotFound(ActionResult innerResult)
        {
            Requires.NotNull("innerResult", innerResult);
            return new ResourceNotFoundResult { InnerResult = innerResult };
        }

        public ModuleInfo ActiveModule { get; set; }

        public TabInfo ActivePage
        {
            get { return (SiteContext == null) ? null : SiteContext.ActivePage; }
        }

        public PortalInfo ActiveSite
        {
            get { return (SiteContext == null) ? null : SiteContext.ActiveSite; }
        }

        public PortalAliasInfo ActiveSiteAlias
        {
            get { return (SiteContext == null) ? null : SiteContext.ActiveSiteAlias; }
        }

        public SiteContext SiteContext
        {
            get { return HttpContext.GetSiteContext(); }
        }

        public new UserInfo User
        {
            get { return (SiteContext == null) ? null : SiteContext.User; }
        }

        protected override ViewResult View(IView view, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }

            return new DnnViewResult
                        {
                            View = view,
                            ViewData = ViewData,
                            TempData = TempData
                        };
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }

            return new DnnViewResult
                            {
                                ViewName = viewName,
                                MasterName = masterName,
                                ViewData = ViewData,
                                TempData = TempData,
                                ViewEngineCollection = ViewEngineCollection
                            };
        }
    }
}