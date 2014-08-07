using System;
using System.Web;

using Dnn.Mvc.Web.Framework;

namespace Dnn.Mvc.Web.Helpers
{
    public static class HttpContextBaseExtensions
    {
        public static SiteContext GetSiteContext(this HttpContextBase httpContext)
        {
            SiteContext settings = null;
            if (httpContext != null)
            {
                settings = (SiteContext)httpContext.Items[GetKeyFor<SiteContext>()];
            }
            return settings;
        }

        public static bool HasSiteContext(this HttpContextBase httpContext)
        {
            return (GetSiteContext(httpContext) != null);
        }

        public static void SetSiteContext(this HttpContextBase httpContext, SiteContext siteContext)
        {
            httpContext.Items[GetKeyFor<SiteContext>()] = siteContext;
        }


        internal static string GetKeyFor<T>()
        {
            return String.Format("DnnMvc:{0}", typeof(T).FullName);
        }

    }
}