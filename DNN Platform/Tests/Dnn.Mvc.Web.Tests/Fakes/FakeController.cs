using System.Web.Mvc;

namespace Dnn.Mvc.Web.Tests.Fakes
{
    public class FakeController : Controller
    {
        public ActionResult Index()
        {
            return new ViewResult();
        }
    }
}
