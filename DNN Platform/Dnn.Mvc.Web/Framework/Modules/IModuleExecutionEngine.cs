using System.IO;
using System.Web;
using DotNetNuke.Entities.Modules;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public interface IModuleExecutionEngine
    {
        ModuleRequestResult ExecuteModule(HttpContextBase httpContext, ModuleInfo module, string moduleRoute);

        void ExecuteModuleResult(SiteContext siteContext, ModuleRequestResult moduleResult);

        void ExecuteModuleResult(SiteContext siteContext, ModuleRequestResult moduleResult, TextWriter writer);
    }
}
