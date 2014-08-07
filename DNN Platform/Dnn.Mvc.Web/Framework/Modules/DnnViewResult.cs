﻿using System;
using System.IO;
using System.Web.Mvc;

using DotNetNuke.Common;

namespace Dnn.Mvc.Web.Framework.Modules
{
    public class DnnViewResult : ViewResult, IDnnViewResult
    {
        public void ExecuteResult(ControllerContext context, TextWriter writer)
        {
            Requires.NotNull("context", context);
            Requires.NotNull("writer", writer);

            if (String.IsNullOrEmpty(ViewName))
            {
                ViewName = context.RouteData.GetRequiredString("action");
            }

            ViewEngineResult result = null;

            if (View == null)
            {
                result = ViewEngineCollection.FindView(context, ViewName, String.Empty);
                View = result.View;
            }

            var viewContext = new ViewContext(context, View, ViewData, TempData, writer);
            View.Render(viewContext, writer);

            if (result != null)
            {
                result.ViewEngine.ReleaseView(context, View);
            }
        }
    }
}