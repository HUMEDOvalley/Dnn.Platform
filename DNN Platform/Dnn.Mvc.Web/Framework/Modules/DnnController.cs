using System.Web.Mvc;

using DotNetNuke.Data;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public abstract class DnnController : DnnControllerBase, IDnnController
    {
        private readonly ResultCapturingActionInvoker _actionInvoker;

        protected DnnController()
        {
            _actionInvoker = new ResultCapturingActionInvoker();
            ActionInvoker = _actionInvoker;
        }

        public ActionResult ResultOfLastExecute
        {
            get { return _actionInvoker.ResultOfLastInvoke; }
        }

    }
}