using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Dnn.Mvc.Tests.Utilities;
using Dnn.Mvc.Web.Framework;

using NUnit.Framework;

namespace Dnn.Mvc.Web.Tests.Framework
{
    [TestFixture]
    public class SiteContextTests
    {
        [Test]
        public void Constructor_Requires_Non_Null_HttpContext()
        {
            HttpContextBase context = null;
            Assert.Throws<ArgumentNullException>(() => new SiteContext(context));
        }

        [Test]
        public void Constructor_Sets_HttpContext_Property()
        {
            HttpContextBase context = MockHelper.CreateMockHttpContext();
            Assert.AreSame(context, new SiteContext(context).HttpContext);
        }
    }
}
