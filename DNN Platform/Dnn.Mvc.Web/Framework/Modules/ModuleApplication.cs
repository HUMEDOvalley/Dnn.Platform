using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dnn.Mvc.Web.Routing;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public abstract class ModuleApplication
    {
        private const string ControllerMasterFormat = "~/Modules/{0}/Views/{{1}}/{{0}}.cshtml";
        private const string SharedMasterFormat = "~/Modules/{0}/Views/Shared/{{0}}.cshtml";
        private const string ControllerViewFormat = "~/Modules/{0}/Views/{{1}}/{{0}}.cshtml";
        private const string SharedViewFormat = "~/Modules/{0}/Views/Shared/{{0}}.cshtml";
        private const string ControllerPartialFormat = "~/Modules/{0}/Views/{{1}}/{{0}}.cshtml";
        private const string SharedPartialFormat = "~/Modules/{0}/Views/Shared/{{0}}.cshtml";
        
        private bool _initialized;
        private readonly object _lock = new object();

        protected ModuleApplication()
        {
            ControllerFactory = ControllerBuilder.Current.GetControllerFactory();
            Routes = new RouteCollection();
            ViewEngines = new ViewEngineCollection();
        }

        public virtual IControllerFactory ControllerFactory { get; set; }

        public abstract string DefaultActionName { get; }

        public abstract string DefaultControllerName { get; }

        protected abstract string FolderPath { get; }

        public abstract string ModuleName { get; }

        public RouteCollection Routes { get; set; }

        public ViewEngineCollection ViewEngines { get; set; }

        protected internal virtual IDnnController AdaptController(IController controller)
        {
            var mvcController = controller as Controller;
            if (mvcController != null && mvcController.ActionInvoker is ControllerActionInvoker)
            {
                return new DnnControllerAdapter(mvcController);
            }
            return null;
        }

        private void EnsureInitialized(HttpContextBase context)
        {
            // Double-check lock to wait for initialization
            // TODO: Is there a better (preferably using events and waits) way to do this?
            if (!_initialized)
            {
                lock (_lock)
                {
                    if (!_initialized)
                    {
                        Init(context.ApplicationInstance as DnnMvcApplication);
                        _initialized = true;
                    }
                }
            }
        }

        public virtual ModuleRequestResult ExecuteRequest(ModuleRequestContext context)
        {
            EnsureInitialized(context.HttpContext);

            // Create a rewritten HttpRequest (wrapped in an HttpContext) to provide to the routing system
            HttpContextBase rewrittenContext = new RewrittenHttpContext(context.HttpContext, context.ModuleRoutingUrl);

            // Route the request
            RouteData routeData = GetRouteData(rewrittenContext);

            // Setup request context
            string controllerName = routeData.GetRequiredString("controller");
            var requestContext = new RequestContext(context.HttpContext, routeData);

            // Construct the controller using the ControllerFactory
            IController controller = ControllerFactory.CreateController(requestContext, controllerName);
            try
            {
                // Check if the controller supports IDnnController and if not, try to adapt it
                var moduleController = controller as IDnnController ?? AdaptController(controller);

                // If we couldn't adapt it, we fail.  We can't support IController implementations without some kind of adaptor :(
                // Because we need to retrieve the ActionResult without executing it, IController won't cut it
                if (moduleController == null)
                {
                    throw new InvalidOperationException("Could Not Construct Controller");
                }
                moduleController.ActiveModule = context.Module;

                // Execute the controller and capture the result
                moduleController.Execute(requestContext);
                ActionResult result = moduleController.ResultOfLastExecute;

                // Check if the result should override the rest of the page content, and if so, package it in a PageOverrideResult
                if (!(result is PageOverrideResult) && ShouldOverrideOtherModules(result, context, moduleController.ControllerContext))
                {
                    result = new PageOverrideResult(result);
                }

                // Return the final result
                return new ModuleRequestResult
                {
                    Application = this,
                    ActionResult = result,
                    ControllerContext = moduleController.ControllerContext,
                    Module = context.Module
                };
            }
            finally
            {
                ControllerFactory.ReleaseController(controller);
            }
        }

        protected internal virtual RouteData GetRouteData(HttpContextBase httpContext)
        {
            return Routes.GetRouteData(httpContext);
        }

        protected internal virtual void Init(DnnMvcApplication application)
        {
            string prefix = NormalizeFolderPath(FolderPath);
            string[] masterFormats = new[] { 
                String.Format(CultureInfo.InvariantCulture, ControllerMasterFormat, prefix),
                String.Format(CultureInfo.InvariantCulture, SharedMasterFormat, prefix) };
            string[] viewFormats = new[] { 
                String.Format(CultureInfo.InvariantCulture, ControllerViewFormat, prefix),
                String.Format(CultureInfo.InvariantCulture, SharedViewFormat, prefix),
                String.Format(CultureInfo.InvariantCulture, ControllerPartialFormat, prefix),
                String.Format(CultureInfo.InvariantCulture, SharedPartialFormat, prefix) };

            ViewEngines.Add(new RazorViewEngine
                                    {
                                        MasterLocationFormats = masterFormats,
                                        ViewLocationFormats = viewFormats,
                                        PartialViewLocationFormats = viewFormats
                                    });
        }

        private static string NormalizeFolderPath(string path)
        {
            // Remove leading and trailing slashes
            if (!String.IsNullOrEmpty(path))
            {
                return path.Trim('/');
            }
            return path;
        }

        protected internal virtual bool ShouldOverrideOtherModules(ActionResult result, ModuleRequestContext moduleRequestContext, ControllerContext controllerContext)
        {
            // All other results, such as "File", "Json", and "Partial View" (which is usually used for AJAX Partial Rendering)
            // will override the page and be rendered as the sole result to the client
            return result is FileResult ||
                   result is HttpUnauthorizedResult ||
                   result is JavaScriptResult ||
                   result is JsonResult ||
                   result is RedirectResult ||
                   result is RedirectToRouteResult ||
                   result is PartialViewResult;
        }

    }
}