using System;
using System.Web.Mvc;

namespace Dnn.Mvc.Web.Helpers
{
    public class ResourceNotFoundResult : ActionResult
    {
        private static readonly Func<ActionResult> FallbackInnerResultFactory = () => new EmptyResult();

        private static Func<ActionResult> _defaultInnerResultFactory;

        public static Func<ActionResult> DefaultInnerResultFactory
        {
            get
            {
                return _defaultInnerResultFactory ?? FallbackInnerResultFactory;
            }
            set { _defaultInnerResultFactory = value; }
        }

        public ActionResult InnerResult { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = 404;
            ActionResult result = InnerResult ?? DefaultInnerResultFactory();
            result.ExecuteResult(context);
        }
    }
}