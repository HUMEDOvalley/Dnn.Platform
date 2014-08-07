using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Web.Controllers;
using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;
using Dnn.Mvc.Web.Helpers;
using Dnn.Mvc.Web.Tests.Fakes;

using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Framework.Modules
{
    [TestFixture]
    public class ResultCapturingActionInvokerTests
    {
        [Test]
        public void InvokeActionResult_Sets_ResultOfLastInvoke()
        {
            //Arrange
            HttpContextBase context = MockHelper.CreateMockHttpContext();
            context.SetSiteContext(new SiteContext(context));

            var controller = new FakeController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);
            
            var actionInvoker = new ResultCapturingActionInvoker();

            //Act
            actionInvoker.InvokeAction(controller.ControllerContext, "Index");

            //Assert
            Assert.IsNotNull(actionInvoker.ResultOfLastInvoke);
            Assert.IsInstanceOf<ViewResult>(actionInvoker.ResultOfLastInvoke);
        }
    }
}
