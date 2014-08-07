using System.Web.Mvc;

using Dnn.Mvc.Web.Helpers;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public abstract class DnnWebViewPage<TModel> : WebViewPage<TModel>
    {
        public DnnHelper<TModel> Dnn { get; set; }

        public override void InitHelpers() 
        {
            base.InitHelpers();
            Dnn = new DnnHelper<TModel>(ViewContext, this);
        }
    }
}