using System;
using System.Collections.Generic;

using Dnn.Mvc.Modules.Html.Controllers;
using Dnn.Mvc.Modules.Html.Models;
using Dnn.Mvc.Web.Framework.Modules;

using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;

using Moq;

using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Modules.HTML
{
    [TestFixture]
    public class HtmlControllerTests
    {
        [Test]
        public void Constructor_Throws_On_Null_DataContext()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new HtmlController(null));
        }

        [Test]
        public void Index_Returns_DnnViewResult()
        {
            //Arrange
            var mockDataContext = new Mock<IDataContext>();
            var mockRepository = new Mock<IRepository<HtmlText>>();
            mockDataContext.Setup(d => d.GetRepository<HtmlText>())
                            .Returns(mockRepository.Object);
            mockRepository.Setup(r => r.Find(It.IsAny<string>()))
                            .Returns(new List<HtmlText>() { new HtmlText()});
            var controller = new HtmlController(mockDataContext.Object) {ActiveModule = new ModuleInfo()};

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IDnnViewResult>(result);
        }
    }
}
