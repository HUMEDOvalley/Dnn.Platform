using System.Web.Mvc;

using Dnn.Mvc.Web.Helpers;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public abstract class DnnWebViewPage : WebViewPage
    {
        public DnnHelper Dnn { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Dnn = new DnnHelper<object>(ViewContext, this);
        }
    }
}