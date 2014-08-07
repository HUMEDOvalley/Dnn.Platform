using System.Web.Mvc;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class ResultCapturingActionInvoker : ControllerActionInvoker
    {
        public ActionResult ResultOfLastInvoke { get; set; }

        // TODO: Capture result filters to execute later

        protected override void InvokeActionResult(ControllerContext controllerContext, ActionResult actionResult)
        {
            //Do not invoke the action.  Instead, store it for later retrieval
            ResultOfLastInvoke = actionResult;
        }
    }
}