using System.Web.Mvc;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public interface IDnnController : IController
    {
        ModuleInfo ActiveModule { get; set; }

        ControllerContext ControllerContext { get; }

        ActionResult ResultOfLastExecute { get; }
    }
}
