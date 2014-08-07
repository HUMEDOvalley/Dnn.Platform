using System.Web.Routing;

using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;
using Dnn.Mvc.Web.Helpers;

namespace Dnn.Mvc.Modules.Html
{
	public class HtmlModuleApplication : ModuleApplication
	{
        public const string HTML_ControllersNamespace = "Dnn.Mvc.Modules.Html.Controllers";

	    public override string DefaultActionName
	    {
            get { return "Index"; }
	    }

	    public override string DefaultControllerName
	    {
	        get { return "Html"; }
	    }

	    protected override string FolderPath
	    {
	        get { return "HTML"; }
	    }

	    public override string ModuleName
	    {
            get { return "Dnn.Modules.Html"; }
	    }

	    protected override void Init(DnnMvcApplication application)
	    {
	        base.Init(application);
            RegisterRoutes(Routes);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.RegisterDefaultRoute(DefaultControllerName, new[] { HTML_ControllersNamespace });
        }
	}
}