using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using DotNetNuke.Common;

namespace Dnn.Mvc.Web.Framework
{
    public class DnnRazorViewEngine : RazorViewEngine {
        public DnnRazorViewEngine()
        {
            ViewLocationFormats = new[] {
                "~/Views/{0}/{1}.cshtml",
                "~/Views/Shared/{1}.cshtml"
            };
            PartialViewLocationFormats = ViewLocationFormats;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            Requires.NotNull("controllerContext", controllerContext);
            Requires.NotNullOrEmpty("viewName", partialViewName);

            IList<string> searchedLocations = new List<string>();
            string viewPath = GetPath(controllerContext, ViewLocationFormats, partialViewName, searchedLocations);
            if (String.IsNullOrEmpty(viewPath)) {
                return new ViewEngineResult(searchedLocations);
            }
            return new ViewEngineResult(CreatePartialView(controllerContext, viewPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            Requires.NotNull("controllerContext", controllerContext);
            Requires.NotNullOrEmpty("viewName", viewName);

            IList<string> searchedLocations = new List<string>();
            string viewPath = GetPath(controllerContext, ViewLocationFormats, viewName, searchedLocations);
            string masterPath = String.Empty;
            if(!String.IsNullOrEmpty(masterName)) {
                masterPath = GetPath(controllerContext, MasterLocationFormats, masterName, searchedLocations);
            }
            if(String.IsNullOrEmpty(viewPath) || (String.IsNullOrEmpty(masterPath) && !String.IsNullOrEmpty(masterName))) {
                return new ViewEngineResult(searchedLocations);
            }
            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        public static bool IsSpecificPath(string name)
        {
            return name.Length > 0 && name[0] == '~' || name[0] == '/';
        }

        private string GetPath(ControllerContext context, string[] locationFormats, string name, IList<string> searchedLocations) {
            Requires.NotNull("context", context);
            Requires.NotNull("locationFormats", locationFormats);
            Requires.NotNullOrEmpty("name", name);
            Requires.NotNull("searchedLocations", searchedLocations);

            if(IsSpecificPath(name)) {
                return GetSpecificPath(context, name, searchedLocations);
            }
            return GetGeneralPath(context, name, locationFormats, searchedLocations);
        }

        private string GetGeneralPath(ControllerContext context, string name, string[] locationFormats, IList<string> searchedLocations) {
            string controllerName = context.RouteData.GetRequiredString("controller");
            foreach(string format in locationFormats) {
                string path = String.Format(CultureInfo.InvariantCulture, format, controllerName, name);
                if(FileExists(context, path)) {
                    return path;
                }
                searchedLocations.Add(path);
            }
            return String.Empty;
        }

        private string GetSpecificPath(ControllerContext context, string name, IList<string> searchedLocations) {
            if(!FileExists(context, name)) {
                searchedLocations.Add(name);
                return String.Empty;
            }
            return name;
        }

    }
}