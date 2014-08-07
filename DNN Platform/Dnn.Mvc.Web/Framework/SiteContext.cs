using System.Web;
using Dnn.Mvc.Web.Framework.Modules;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

namespace Dnn.Mvc.Web.Framework
{
    public class SiteContext
    {
        public SiteContext(HttpContextBase httpContext)
        {
            Requires.NotNull("httpContext", httpContext);
            HttpContext = httpContext;
        }

        public ModuleRequestResult ActiveModuleRequest { get; set; }

        public TabInfo ActivePage { get; set; }

        public PortalInfo ActiveSite { get; set; }

        public PortalAliasInfo ActiveSiteAlias { get; set; }

        public HttpContextBase HttpContext { get; private set; }

        public UserInfo User { get; private set; }

        internal void SetUser(UserInfo user)
        {
            User = user;
            HttpContext.Items.Add("UserInfo", user);
        }

        //public string CurrentTheme
        //{
        //    get
        //    {
        //        if (_currentTheme == null && DnnMvcApplication.Container != null)
        //        {
        //            _currentTheme = DnnMvcApplication.Container
        //                                               .GetExportedObjectOrDefault<string>(WebContractNames.AppDefaultTheme);
        //        }
        //        return _currentTheme;
        //    }
        //    set { _currentTheme = value; }
        //}
    }
}