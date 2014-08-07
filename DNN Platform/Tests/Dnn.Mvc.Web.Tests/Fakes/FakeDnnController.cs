using System.Web.Mvc;

using Dnn.Mvc.Web.Framework.Modules;

namespace Dnn.Mvc.Web.Tests.Fakes
{
    public class FakeDnnController : DnnController
    {
        public ActionResult Index()
        {
            return new ViewResult();
        }
    }
}
