﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Utils.Entities.Modules;
using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;
using Moq;
using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Framework.Modules
{
    [TestFixture]
    public class ModuleExecutionEngineTests
    {
        [SetUp]
        public void SetUp()
        {
            ComponentFactory.Container = new SimpleContainer();
            DesktopModuleControllerAdapter.ClearInstance();
        }

        [Test]
        public void ExecuteModule_Requires_NonNull_HttpContextBase()
        {
            var engine = new ModuleExecutionEngine();
            Assert.Throws<ArgumentNullException>(() => engine.ExecuteModule(null, new ModuleInfo(), "Foo"));
        }

        [Test]
        public void ExecuteModule_Requires_NonNull_Module()
        {
            var engine = new ModuleExecutionEngine();
            Assert.Throws<ArgumentNullException>(() => engine.ExecuteModule(MockHelper.CreateMockHttpContext(), null, "Foo"));
        }

        [Test]
        public void ExecuteModule_Requires_NonNull_ModuleRoute()
        {
            // Empty route is OK
            var engine = new ModuleExecutionEngine();
            Assert.Throws<ArgumentNullException>(() => engine.ExecuteModule(MockHelper.CreateMockHttpContext(), new ModuleInfo(), null));
        }

        [Test]
        public void ExecuteModule_Returns_Null_If_Application_For_Provided_Module_Does_Not_Exist()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            SetUpMockDesktopController(It.IsAny<int>(), It.IsAny<int>(), null);
            
            // Act
            ModuleRequestResult result = engine.ExecuteModule(MockHelper.CreateMockHttpContext(),
                                                              new ModuleInfo() { DesktopModuleID = 1, PortalID = 1},
                                                              String.Empty);
            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ExecuteModule_Executes_ModuleApplication_For_Module_If_Exists()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            var desktopModule = new DesktopModuleInfo() { DesktopModuleID = 1, ModuleName = "TestModule"};
            SetUpMockDesktopController(1, 1, desktopModule);

            var moduleApp = new Mock<ModuleApplication>();
            var apps = new Dictionary<string, ModuleApplication> { { "TestModule", moduleApp.Object } };
            ComponentFactory.RegisterComponentInstance<IDictionary<string, ModuleApplication>>(apps);

            // Act
            ModuleRequestResult result = engine.ExecuteModule(MockHelper.CreateMockHttpContext(),
                                                              new ModuleInfo() { DesktopModuleID = 1, PortalID = 1 },
                                                              String.Empty);

            // Assert
            moduleApp.Verify(app => app.ExecuteRequest(It.IsAny<ModuleRequestContext>()));
        }

        [Test]
        public void ExecuteModule_Returns_Result_Of_Executing_ModuleApplication()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();
            var expected = new ModuleRequestResult();

            var desktopModule = new DesktopModuleInfo() { DesktopModuleID = 1, ModuleName = "TestModule" };
            SetUpMockDesktopController(1, 1, desktopModule);

            var moduleApp = new Mock<ModuleApplication>();
            var apps = new Dictionary<string, ModuleApplication> { { "TestModule", moduleApp.Object } };
            ComponentFactory.RegisterComponentInstance<IDictionary<string, ModuleApplication>>(apps);

            moduleApp.Setup(app => app.ExecuteRequest(It.IsAny<ModuleRequestContext>()))
                    .Returns(expected);

            // Act
            ModuleRequestResult actual = engine.ExecuteModule(MockHelper.CreateMockHttpContext(),
                                                              new ModuleInfo() { DesktopModuleID = 1, PortalID = 1 },
                                                              String.Empty);

            // Assert
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ExecuteModule_Provides_Context_Data_To_Executed_ModuleApplication()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            var desktopModule = new DesktopModuleInfo() { DesktopModuleID = 1, ModuleName = "TestModule" };
            SetUpMockDesktopController(1, 1, desktopModule);

            var moduleApp = new Mock<ModuleApplication>();
            var apps = new Dictionary<string, ModuleApplication> { { "TestModule", moduleApp.Object } };
            ComponentFactory.RegisterComponentInstance<IDictionary<string, ModuleApplication>>(apps);

            HttpContextBase httpContext = MockHelper.CreateMockHttpContext();
            var module = new ModuleInfo { DesktopModuleID = 1, PortalID = 1 };
            const string route = "Foo/Bar/Baz";

            ModuleRequestContext providedContext = null;
            moduleApp.Setup(app => app.ExecuteRequest(It.IsAny<ModuleRequestContext>()))
                    .Callback<ModuleRequestContext>(c => providedContext = c);

            // Act
            engine.ExecuteModule(httpContext, module, route);

            // Assert
            Assert.AreSame(moduleApp.Object, providedContext.Application);
            Assert.AreSame(module, providedContext.Module);
            Assert.AreSame(httpContext, providedContext.HttpContext);
            Assert.AreEqual(route, providedContext.ModuleRoutingUrl);
        }

        [Test]
        public void ExecuteModuleResult_Calls_IDnnViewResult_ExecuteResult()
        {
            //Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var viewResultMock = actionResultMock.As<IDnnViewResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult()
                                            {
                                                ActionResult = actionResultMock.Object,
                                                ControllerContext = controllerContext
                                            };

            //Act
            engine.ExecuteModuleResult(new SiteContext(MockHelper.CreateMockHttpContext()), moduleRequestResult, new StringWriter());


            //Arrange
            viewResultMock.Verify(v => v.ExecuteResult(It.IsAny<ControllerContext>(), It.IsAny<TextWriter>()));
        }

        [Test]
        public void ExecuteModuleResult_Overload_Calls_ActionResult_ExecuteResult()
        {
            //Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult()
                                            {
                                                ActionResult = actionResultMock.Object,
                                                ControllerContext = controllerContext
                                            };

            //Act
            engine.ExecuteModuleResult(new SiteContext(MockHelper.CreateMockHttpContext()), moduleRequestResult);


            //Arrange
            actionResultMock.Verify(v => v.ExecuteResult(It.IsAny<ControllerContext>()));
        }

        [Test]
        public void ExecuteModuleResult_Calls_IDnnViewResult_ExecuteResult_With_ModuleRequestResult_ControllerContext()
        {
            //Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var viewResultMock = actionResultMock.As<IDnnViewResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult()
                                            {
                                                ActionResult = actionResultMock.Object,
                                                ControllerContext = controllerContext
                                            };

            //Act
            engine.ExecuteModuleResult(new SiteContext(MockHelper.CreateMockHttpContext()), moduleRequestResult, new StringWriter());


            //Arrange
            viewResultMock.Verify(v => v.ExecuteResult(controllerContext, It.IsAny<TextWriter>()));
        }

        [Test]
        public void ExecuteModuleResult_Overload_Calls_ActionResult_ExecuteResult_With_ModuleRequestResult_ControllerContext()
        {
            //Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult()
                                                {
                                                    ActionResult = actionResultMock.Object,
                                                    ControllerContext = controllerContext
                                                };

            //Act
            engine.ExecuteModuleResult(new SiteContext(MockHelper.CreateMockHttpContext()), moduleRequestResult);


            //Arrange
            actionResultMock.Verify(v => v.ExecuteResult(controllerContext));
        }

        [Test]
        public void RunInModuleContext_Runs_Provided_Action()
        {
            // Arrange
            HttpContextBase httpContext = MockHelper.CreateMockHttpContext();
            var moduleResult = new ModuleRequestResult();
            var siteContext = new SiteContext(httpContext);
            var engine = new ModuleExecutionEngine();
            bool actionRun = false;

            // Act
            engine.RunInModuleResultContext(siteContext,
                                            moduleResult,
                                            () =>
                                            {
                                                actionRun = true;
                                            });

            // Assert
            Assert.IsTrue(actionRun);
        }

        [Test]
        public void RunInModuleContext_Sets_ActiveModuleRequest_Before_Calling_Delegate()
        {
            // Arrange
            HttpContextBase httpContext = MockHelper.CreateMockHttpContext();
            var moduleResult = new ModuleRequestResult();
            var siteContext = new SiteContext(httpContext);
            var engine = new ModuleExecutionEngine();

            // Act
            engine.RunInModuleResultContext(siteContext,
                                            moduleResult,
                                            () => Assert.AreSame(moduleResult, siteContext.ActiveModuleRequest));
        }

        [Test]
        public void RunInModuleContext_Restores_Original_ActiveModuleRequest_After_Returning_From_Delegate()
        {
            // Arrange
            HttpContextBase httpContext = MockHelper.CreateMockHttpContext();
            var moduleResult = new ModuleRequestResult();
            var originalModuleResult = new ModuleRequestResult();
            var siteContext = new SiteContext(httpContext);
            var engine = new ModuleExecutionEngine();
            siteContext.ActiveModuleRequest = originalModuleResult;

            // Act
            engine.RunInModuleResultContext(siteContext,
                                            moduleResult,
                                            () => { });

            // Assert
            Assert.AreSame(originalModuleResult, siteContext.ActiveModuleRequest);
        }

        private void SetUpMockDesktopController(int desktopModuleId, int portalId, DesktopModuleInfo desktopModule)
        {
            var mockDesktopController = new Mock<IDesktopModuleController>();
            mockDesktopController.Setup(c => c.GetDesktopModule(desktopModuleId, portalId))
                                    .Returns(desktopModule);
            DesktopModuleControllerAdapter.SetTestableInstance(mockDesktopController.Object);
           
        }
    }
}
