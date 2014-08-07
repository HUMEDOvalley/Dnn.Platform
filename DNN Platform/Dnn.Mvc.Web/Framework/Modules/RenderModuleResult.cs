using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Dnn.Mvc.Web.Helpers;

using DotNetNuke.Common;
using DotNetNuke.ComponentModel;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class RenderModuleResult : ActionResult
    {
        public ModuleRequestResult ModuleRequestResult { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            Requires.NotNull("context", context);
            if (ModuleRequestResult != null)
            {
                var moduleExecutionEngine = ComponentFactory.GetComponent<IModuleExecutionEngine>();
                moduleExecutionEngine.ExecuteModuleResult(context.HttpContext.GetSiteContext(), ModuleRequestResult);
            }
        }
    }
}