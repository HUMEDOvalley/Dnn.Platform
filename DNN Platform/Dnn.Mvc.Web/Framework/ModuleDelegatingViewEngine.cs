using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dnn.Mvc.Web.Framework.Modules;
using Dnn.Mvc.Web.Helpers;

namespace Dnn.Mvc.Web.Framework
{
    public class ModuleDelegatingViewEngine : IViewEngine
    {
        private readonly Dictionary<IView, IViewEngine> _viewEngineMappings = new Dictionary<IView, IViewEngine>();

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return RunAgainstModuleViewEngines(controllerContext, e => e.FindPartialView(controllerContext, partialViewName));
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return RunAgainstModuleViewEngines(controllerContext, e => e.FindView(controllerContext, viewName, masterName));
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            if (_viewEngineMappings.ContainsKey(view))
            {
                _viewEngineMappings[view].ReleaseView(controllerContext, view);
            }
        }

        private ViewEngineResult RunAgainstModuleViewEngines(ControllerContext controllerContext, Func<ViewEngineCollection, ViewEngineResult> engineRequest)
        {
            // Get the current module request
            ModuleRequestResult moduleRequest = GetCurrentModuleRequest(controllerContext);

            // No current request => Skip this view engine
            if (moduleRequest == null)
            {
                return new ViewEngineResult(new string[0]);
            }

            // Delegate to the module's view engine collection
            ViewEngineResult result = engineRequest(moduleRequest.Application.ViewEngines);

            // If there is a view, store the view<->viewengine mapping so release works correctly
            if (result.View != null)
            {
                _viewEngineMappings[result.View] = result.ViewEngine;
            }

            return result;
        }

        private static ModuleRequestResult GetCurrentModuleRequest(ControllerContext controllerContext)
        {
            SiteContext requestContext = controllerContext.HttpContext.GetSiteContext();
            return requestContext.ActiveModuleRequest;
        }
    }
}