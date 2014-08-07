﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;
using DotNetNuke.Entities.Modules;
using Moq;
using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Framework.Modules
{
    [TestFixture]
    public class ModuleApplicationTests
    {
        [Test]
        public void Init_Is_Called_In_First_ExecuteRequest_And_Not_In_Subsequent_Requests()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };
            SetupGetRouteDataCall(app);

            int initCounter = 0;
            app.Setup(a => a.Init(It.IsAny<DnnMvcApplication>()))
                .Callback(() => initCounter++);

            var controller = new Mock<IDnnController>();

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            // Act
            app.Object.ExecuteRequest(CreateModuleContext(app.Object, "Foo"));
            app.Object.ExecuteRequest(CreateModuleContext(app.Object, "Bar"));

            // Assert
            Assert.AreEqual(1, initCounter);
        }

        [Test]
        public void ExecuteRequest_Calls_GetRouteData_With_ModuleRoute_To_Route_Request()
        {
            //Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            HttpContextBase actualContext = null;
            app.Setup(a => a.GetRouteData(It.IsAny<HttpContextBase>()))
                .Callback<HttpContextBase>(providedContext => actualContext = providedContext)
                .Returns(CreateTestRouteData());

            var controller = new Mock<IDnnController>();

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            //Act
            app.Object.ExecuteRequest(moduleRequestContext);

            //Assert
            app.Verify(a => a.GetRouteData(It.IsAny<HttpContextBase>()));

            Assert.AreEqual("~/Foo/Bar/Baz", actualContext.Request.AppRelativeCurrentExecutionFilePath);
        }

        [Test]
        public void ExecuteRequest_Calls_ControllerFactory_To_Construct_Controller()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controllerFactory = new Mock<IControllerFactory>();
            RequestContext actualRequestContext = null;
            controllerFactory.Setup(f => f.CreateController(It.IsAny<RequestContext>(), "Foo"))
                                 .Callback<RequestContext, string>((c, n) => actualRequestContext = c)
                                 .Returns(new Mock<IDnnController>().Object);

            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act
            app.Object.ExecuteRequest(moduleRequestContext);

            // Assert
            controllerFactory.Verify(f => f.CreateController(It.IsAny<RequestContext>(), "Foo"));
            Assert.AreSame(expectedRouteData, actualRequestContext.RouteData);
            Assert.AreSame(moduleRequestContext.HttpContext, actualRequestContext.HttpContext);
        }

        [Test]
        public void ExecuteRequest_Throws_InvalidOperationException_If_Controller_Only_Implements_IController()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controller = new Mock<IController>();
            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => app.Object.ExecuteRequest(moduleRequestContext));
        }

        [Test]
        public void ExecuteRequest_Throws_InvalidOperationException_If_Controller_Has_NonStandard_Action_Invoker()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controller = new Mock<Controller>();
            var invoker = new Mock<IActionInvoker>();
            controller.Object.ActionInvoker = invoker.Object;

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => app.Object.ExecuteRequest(moduleRequestContext));
        }

        [Test]
        public void ExecuteRequest_Does_Not_Throw_If_Controller_Implements_IDnnController()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controller = new Mock<IController>();
            controller.As<IDnnController>();

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act and Assert
            app.Object.ExecuteRequest(moduleRequestContext);
        }

        [Test]
        public void ExecuteRequest_Does_Not_Throw_If_Controller_Inherits_From_Controller()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controller = new Mock<Controller>();
            var invoker = new ControllerActionInvoker();
            controller.Object.ActionInvoker = invoker;

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act and Assert
            app.Object.ExecuteRequest(moduleRequestContext);
        }

        [Test]
        public void ExecuteRequest_Returns_Result_And_ControllerContext_From_Controller()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            ControllerContext controllerContext = MockHelper.CreateMockControllerContext();
            ActionResult actionResult = new Mock<ActionResult>().Object;

            var controller = SetupMockController(actionResult, controllerContext);

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act
            ModuleRequestResult result = app.Object.ExecuteRequest(moduleRequestContext);

            // Assert
            Assert.AreSame(actionResult, result.ActionResult);
            Assert.AreSame(controllerContext, result.ControllerContext);
        }

        //[TestMethod]
        //public void ExecuteRequest_Wraps_Request_In_PageOverrideResult_If_ShouldOverrideOtherModules_Returns_True()
        //{
        //    // Arrange
        //    ModuleApplication app = CreateTestApplication();

        //    RouteData expectedRouteData = SetupGetRouteDataCall(app);

        //    ControllerContext controllerContext = Mockery.CreateMockControllerContext();
        //    ActionResult actionResult = new Mock<ActionResult>().Object;

        //    IModuleController controller = SetupMockController(actionResult, controllerContext);
        //    app.ControllerFactory = SetupControllerFactory(controller);

        //    Mock.Get(app)
        //        .Setup(a => a.ShouldOverrideOtherModules(actionResult, It.IsAny<ModuleRequestContext>(), It.IsAny<ControllerContext>()))
        //        .Returns(true);

        //    ModuleRequestContext moduleRequestContext = CreateModuleContext(app, "Foo/Bar/Baz");

        //    // Act
        //    ModuleRequestResult result = app.ExecuteRequest(moduleRequestContext);

        //    // Assert
        //    Assert.IsInstanceOfType(result.ActionResult, typeof(PageOverrideResult));
        //    Assert.AreSame(actionResult, ((PageOverrideResult)result.ActionResult).InnerResult);
        //}

        [Test]
        public void ExecuteRequest_Executes_Constructed_Controller_And_Provides_RequestContext()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };

            RouteData expectedRouteData = SetupGetRouteDataCall(app);

            var controller = new Mock<IController>();
            controller.As<IDnnController>();

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act
            ModuleRequestResult result = app.Object.ExecuteRequest(moduleRequestContext);

            // Assert
            controller.Verify(c => c.Execute(It.Is<RequestContext>(rc =>
                    rc.HttpContext == moduleRequestContext.HttpContext &&
                    rc.RouteData.GetRequiredString("controller") == "Foo"))
                );
        }

        [Test]
        public void ExecuteRequest_ReleasesController_After_Executing()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };
            SetupGetRouteDataCall(app);

            var controller = new Mock<IDnnController>();

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act
            ModuleRequestResult result = app.Object.ExecuteRequest(moduleRequestContext);

            // Assert
            controllerFactory.Verify(cf => cf.ReleaseController(controller.Object));
        }

        [Test]
        public void ExecuteRequest_ReleasesController_Even_If_It_Throws_An_Exception()
        {
            // Arrange
            var app = new Mock<ModuleApplication> { CallBase = true };
            SetupGetRouteDataCall(app);

            var controller = new Mock<IDnnController>();
            controller.Setup(c => c.Execute(It.IsAny<RequestContext>()))
                .Throws(new Exception("Uh Oh!"));

            var controllerFactory = SetupControllerFactory(controller.Object);
            app.SetupGet(a => a.ControllerFactory).Returns(controllerFactory.Object);

            ModuleRequestContext moduleRequestContext = CreateModuleContext(app.Object, "Foo/Bar/Baz");

            // Act (and verify the exception is thrown; also supresses the exception so it doesn't fail the test)
            Assert.Throws<Exception>(() => app.Object.ExecuteRequest(moduleRequestContext));

            // Assert
            controllerFactory.Verify(f => f.ReleaseController(controller.Object));
        }

        private static Mock<IDnnController> SetupMockController(ActionResult actionResult, ControllerContext controllerContext)
        {
            var mockController = new Mock<IDnnController>();
            mockController.Setup(c => c.ResultOfLastExecute)
                .Returns(actionResult);
            mockController.Setup(c => c.ControllerContext)
                .Returns(controllerContext);
            return mockController;
        }

        private static Mock<IControllerFactory> SetupControllerFactory(IController controller)
        {
            var controllerFactory = new Mock<IControllerFactory>();
            controllerFactory.Setup(cf => cf.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                            .Returns(controller);
            return controllerFactory;
        }

        private static RouteData SetupGetRouteDataCall(Mock<ModuleApplication> app)
        {
            RouteData expectedRouteData = CreateTestRouteData();

            app.Setup(a => a.GetRouteData(It.IsAny<HttpContextBase>()))
                .Returns(expectedRouteData);
            return expectedRouteData;
        }

        private static RouteData CreateTestRouteData()
        {
            var expectedRouteData = new RouteData();
            expectedRouteData.Values["controller"] = "Foo";
            return expectedRouteData;
        }

        private static ModuleRequestContext CreateModuleContext(ModuleApplication app, string moduleRoutingUrl)
        {
            return new ModuleRequestContext
                        {
                            Application = app,
                            HttpContext = MockHelper.CreateMockHttpContext("http://localhost/Portal/Page/ModuleRoute"),
                            Module = new ModuleInfo { ModuleID = 42 },
                            ModuleRoutingUrl = moduleRoutingUrl
                        };
        }

    }
}
