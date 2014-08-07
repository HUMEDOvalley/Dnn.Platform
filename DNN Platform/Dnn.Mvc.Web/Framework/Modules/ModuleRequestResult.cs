using System.Web.Mvc;

using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class ModuleRequestResult
    {
        public ModuleRequestResult()
        {
            ModuleMode = ModuleInjectMode.None;
        }

        public ModuleApplication Application { get; set; }

        public ActionResult ActionResult { get; set; }

        public ControllerContext ControllerContext { get; set; }

        public ModuleInfo Module { get; set; }

        public ModuleInjectMode ModuleMode { get; set; }
    }
}