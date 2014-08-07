using System;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class DnnControllerAdapter : IDnnController  
    {
        private readonly Controller _adaptedController;
        private readonly ResultCapturingActionInvoker _actionInvoker;

        public DnnControllerAdapter(Controller controller)
        {
            _adaptedController = controller;
            _actionInvoker = new ResultCapturingActionInvoker();
            _adaptedController.ActionInvoker = _actionInvoker; 
        }

        public void Execute(RequestContext requestContext) 
        {
            if(_adaptedController.ActionInvoker != _actionInvoker) 
            {
                throw new InvalidOperationException("Could not construct Controller");
            }
            ((IController)_adaptedController).Execute(requestContext);
        }

        public ActionResult ResultOfLastExecute 
        {
            get { return _actionInvoker.ResultOfLastInvoke; }
        }

        public ModuleInfo ActiveModule { get; set; }

        public ControllerContext ControllerContext 
        {
            get { return _adaptedController.ControllerContext; }
        }
    }
}