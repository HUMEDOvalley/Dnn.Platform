using System;
using System.Web.Mvc;
using System.Web.Routing;

using Dnn.Mvc.Modules.Html;
using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;

using Moq;

using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Modules.HTML
{
    [TestFixture]
    public class HtmlModuleApplicationTests
    {
        [Test]
        public void ExecuteRequest_Adds_Single_ModuleRoute_To_Routes()
        {
            //Arrange
            var module = new HtmlModuleApplication();
            var context = CreateModuleRequestContext(module);

            //Act
            module.ExecuteRequest(context);

            //Assert
            Assert.AreEqual(1, module.Routes.Count);
        }

        [Test]
        public void ExecuteRequest_Adds_Default_ModuleRoute_To_Routes()
        {
            //Arrange
            var module = new HtmlModuleApplication();
            var context = CreateModuleRequestContext(module);

            //Act
            module.ExecuteRequest(context);

            //Assert
            var route = module.Routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreEqual("Html", route.Defaults["controller"]);
            Assert.AreEqual("Index", route.Defaults["action"]);
            Assert.AreEqual("", route.Defaults["id"]);
        }

        [Test]
        public void ExecuteRequest_ModuleRoute_With_Namespaces_To_Routes()
        {
            //Arrange
            var module = new HtmlModuleApplication();
            var context = CreateModuleRequestContext(module);

            //Act
            module.ExecuteRequest(context);

            //Assert
            var route = module.Routes[0] as Route;
            Assert.IsNotNull(route);
            var dataTokens = route.DataTokens["Namespaces"] as String[];
            Assert.IsNotNull(dataTokens);

            Assert.AreEqual(HtmlModuleApplication.HTML_ControllersNamespace, dataTokens[0]);
        }

        private ModuleRequestContext CreateModuleRequestContext(HtmlModuleApplication module)
        {
            var context = new ModuleRequestContext()
                                {
                                    HttpContext = MockHelper.CreateMockHttpContext(),
                                    ModuleRoutingUrl = "/"
                                };
            Mock.Get(context.HttpContext)
                .SetupGet(c => c.ApplicationInstance)
                .Returns(new DnnMvcApplication());

            var mockRequest = Mock.Get(context.HttpContext.Request);
            mockRequest.SetupGet(r => r.ApplicationPath)
                .Returns("/");
            mockRequest.SetupGet(r => r.Url)
                .Returns(new Uri("http://localhost/foo"));

            var mockFactory = new Mock<IControllerFactory>();
            var mockController = new Mock<IDnnController>();
            mockFactory.Setup(cf => cf.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                .Returns(mockController.Object);
            module.ControllerFactory = mockFactory.Object;

            return context;
        }
    }
}
