using System.Web.Mvc;
using DotNetNuke.Common;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class PageOverrideResult : ActionResult
    {
        public ActionResult InnerResult { get; private set; }

        public PageOverrideResult(ActionResult innerResult)
        {
            Requires.NotNull("innerResult", innerResult);
            InnerResult = innerResult;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            Requires.NotNull("context", context);
            InnerResult.ExecuteResult(context);
        }
    }
}