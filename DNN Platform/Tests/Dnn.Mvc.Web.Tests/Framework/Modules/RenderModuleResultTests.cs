using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Web.Framework;
using Dnn.Mvc.Web.Framework.Modules;
using DotNetNuke.ComponentModel;
using Moq;
using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Framework.Modules
{
    [TestFixture]
    public class RenderModuleResultTests
    {
        [SetUp]
        public void SetUp()
        {
            ComponentFactory.Container= new SimpleContainer();  
        }

        [Test]
        public void ExecuteResult_Throws_On_Null_Context()
        {
            //Arrange
            ControllerContext context = null;
            var result= new RenderModuleResult();

            //Act,Assert
            Assert.Throws<ArgumentNullException>(() => result.ExecuteResult(context));
        }

        [Test]
        public void ExecuteResult_Does_Not_Call_ModuleExecutionEngine_On_Null_ModuleRequestResult()
        {
            //Arrange
            ControllerContext context = MockHelper.CreateMockControllerContext();
            var result = new RenderModuleResult();

            var mockEngine = new Mock<IModuleExecutionEngine>();
            ComponentFactory.RegisterComponentInstance<IModuleExecutionEngine>(mockEngine.Object);

            //Act
            result.ExecuteResult(context);

            //Assert
            mockEngine.Verify(e => e.ExecuteModuleResult(It.IsAny<SiteContext>(), It.IsAny<ModuleRequestResult>()), Times.Never);
        }

        [Test]
        public void ExecuteResult_Calls_ModuleExecutionEngine_On_ModuleRequestResult()
        {
            //Arrange
            ControllerContext context = MockHelper.CreateMockControllerContext();
            var result = new RenderModuleResult();
            result.ModuleRequestResult = new ModuleRequestResult();

            var mockEngine = new Mock<IModuleExecutionEngine>();
            ComponentFactory.RegisterComponentInstance<IModuleExecutionEngine>(mockEngine.Object);

            //Act
            result.ExecuteResult(context);

            //Assert
            mockEngine.Verify(e => e.ExecuteModuleResult(It.IsAny<SiteContext>(), It.IsAny<ModuleRequestResult>()), Times.Once);
        }
    }
}
