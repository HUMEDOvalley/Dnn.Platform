using System.IO;
using System.Web.Mvc;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public interface IDnnViewResult
    {
        void ExecuteResult(ControllerContext context, TextWriter writer);
    }
}