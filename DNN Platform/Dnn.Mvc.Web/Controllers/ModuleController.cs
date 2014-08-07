using System.Web.Mvc;
using Dnn.Mvc.Web.Framework.Modules;
using Dnn.Mvc.Web.Helpers;
using Dnn.Mvc.Web.Models;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Controllers
{
    public class ModuleController : DnnControllerBase
    {
        public ActionResult Render(int? moduleId, string moduleRoute)
        {

            ModuleRequestResult moduleResult = null;
            if (moduleId.HasValue)
            {
                var module = DotNetNuke.Entities.Modules.ModuleController.Instance.GetModule(moduleId.Value, ActivePage.TabID, false);;

                if (module == null)
                {
                    return ResourceNotFound();
                }

                var moduleExecutionEngine = ComponentFactory.GetComponent<IModuleExecutionEngine>();

                moduleResult = moduleExecutionEngine.ExecuteModule(HttpContext, module, moduleRoute);

                ControllerContext.HttpContext.GetSiteContext().ActiveModuleRequest = moduleResult;

                if (moduleResult != null && moduleResult.ActionResult is PageOverrideResult)
                {
                    return new RenderModuleResult { ModuleRequestResult = moduleResult };
                }
            }

            return View(moduleResult);
        }
    }
}