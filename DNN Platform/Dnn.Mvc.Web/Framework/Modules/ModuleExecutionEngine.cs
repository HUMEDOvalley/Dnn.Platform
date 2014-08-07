using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using Dnn.Mvc.Utils.Entities.Modules;

using DotNetNuke.Common;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class ModuleExecutionEngine : IModuleExecutionEngine
    {
        public virtual ModuleRequestResult ExecuteModule(HttpContextBase httpContext, ModuleInfo module, string moduleRoute)
        {
            Requires.NotNull("httpContext", httpContext);
            Requires.NotNull("module", module);
            Requires.NotNull("moduleRoute", moduleRoute); // Empty route is OK!

            //TODO DesktopModuleControllerAdapter usage is temporary in order to make method testable
            DesktopModuleInfo desktopModule = DesktopModuleControllerAdapter.Instance.GetDesktopModule(module.DesktopModuleID, module.PortalID);

            // If the module application for this module is installed
            var moduleApplications = ComponentFactory.GetComponent<IDictionary<string, ModuleApplication>>();
            ModuleApplication app;
            if (desktopModule != null && moduleApplications.TryGetValue(desktopModule.ModuleName, out app))
            {
                // Setup the module's context
                var moduleRequestContext = new ModuleRequestContext
                                                {
                                                    Application = app,
                                                    Module = module,
                                                    ModuleRoutingUrl = moduleRoute,
                                                    HttpContext = httpContext
                                                };


                // Run the module
                ModuleRequestResult result = app.ExecuteRequest(moduleRequestContext);
                return result;
            }
            return null;
        }

        public virtual void ExecuteModuleResult(SiteContext siteContext, ModuleRequestResult moduleResult)
        {
            RunInModuleResultContext(siteContext,
                                     moduleResult,
                                     () => moduleResult.ActionResult.ExecuteResult(moduleResult.ControllerContext));
        }

        public virtual void ExecuteModuleResult(SiteContext siteContext, ModuleRequestResult moduleResult, TextWriter writer)
        {
            RunInModuleResultContext(siteContext,
                                     moduleResult,
                                     () => ((IDnnViewResult)moduleResult.ActionResult).ExecuteResult(moduleResult.ControllerContext, writer));
        }

        protected internal void RunInModuleResultContext(SiteContext siteContext, ModuleRequestResult moduleResult, Action action)
        {
            // Set the active module
            ModuleRequestResult oldRequest = siteContext.ActiveModuleRequest;
            siteContext.ActiveModuleRequest = moduleResult;

            // Run the action
            action();

            // Restore the previous active module
            siteContext.ActiveModuleRequest = oldRequest;
        }
    }
}