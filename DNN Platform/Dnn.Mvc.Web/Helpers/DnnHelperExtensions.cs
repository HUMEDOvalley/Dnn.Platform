using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Dnn.Mvc.Web.Framework.Modules;
using Dnn.Mvc.Web.Models;

using DotNetNuke.Common;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;

namespace Dnn.Mvc.Web.Helpers
{
    public static class DnnHelperExtensions
    {
        public static string Action(this DnnHelper helper, string actionName)
        {
            var defaultControllerName = helper.SiteContext.ActiveModuleRequest.Application.DefaultControllerName;
            return Action(helper, actionName, defaultControllerName);
        }

        public static string Action(this DnnHelper helper, string actionName, string controllerName)
        {
            const string parentActionName = "Render";
            const string parentControllerName = "Module";

            var moduleId = helper.ActiveModule.ModuleID;

            var routeValues = new RouteValueDictionary
                            {
                                {"moduleId", moduleId},
                                {"moduleRoute", String.Format("{0}/{1}/{2}", controllerName, actionName, moduleId)}
                            };

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);

            return urlHelper.Action(parentActionName, parentControllerName, routeValues);
        }

        public static string Action(this DnnHelper<ModuleRequestResult> helper, bool isModal)
        {
            var defaultActionName = helper.ViewData.Model.Application.DefaultActionName;
            return Action(helper, defaultActionName, isModal);
        }

        public static string Action(this DnnHelper<ModuleRequestResult> helper, string actionName, bool isModal)
        {
            string parentActionName = (isModal) ? "Render" : "Index";
            string parentControllerName = (isModal) ? "Module" : "Page";

            var moduleId = helper.ViewData.Model.Module.ModuleID;
            var defaultControllerName = helper.ViewData.Model.Application.DefaultControllerName;

            var routeValues = new RouteValueDictionary
                            {
                                {"moduleId", moduleId},
                                {"moduleRoute", String.Format("{0}/{1}/{2}", defaultControllerName, actionName, moduleId)}
                            };

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);

            return urlHelper.Action(parentActionName, parentControllerName, routeValues);
        }

        public static MvcHtmlString ActionLink(this DnnHelper<ModuleRequestResult> helper, string linkText)
        {
            return ActionLink(helper, linkText, false);
        }

        public static MvcHtmlString ActionLink(this DnnHelper<ModuleRequestResult> helper, string linkText, bool isModal)
        {
            string parentActionName = (isModal) ? "Render" : "Index";
            string parentControllerName = (isModal) ? "Module" : "Page";

            var moduleId = helper.ViewData.Model.Module.ModuleID;
            var defaultActionName = helper.ViewData.Model.Application.DefaultActionName;
            var defaultControllerName = helper.ViewData.Model.Application.DefaultControllerName;

            var routeValues = new RouteValueDictionary
                            {
                                {"moduleId", moduleId},
                                {"moduleRoute", String.Format("{0}/{1}/{2}", defaultControllerName, defaultActionName, moduleId)}
                            };

            var htmlHelper = new HtmlHelper(helper.ViewContext, helper.ViewDataContainer, helper.RouteCollection);

            return htmlHelper.ActionLink(linkText, parentActionName, parentControllerName, routeValues);
        }

        public static MvcHtmlString RenderBreadCrumbs(this DnnHelper helper)
        {
            var page = helper.ActivePage;

            TabController.Instance.PopulateBreadCrumbs(ref page);

            var breadCrumbBuilder = new TagBuilder("ol");
            breadCrumbBuilder.MergeAttribute("class", "breadcrumb");

            foreach (TabInfo breadCrumbPage in page.BreadCrumbs)
            {
                var breadCrumbItemBuilder = new TagBuilder("li");
                if (breadCrumbPage.TabID == page.TabID || breadCrumbPage.DisableLink)
                {
                    breadCrumbItemBuilder.MergeAttribute("class", "active");
                    breadCrumbItemBuilder.InnerHtml = breadCrumbPage.TabName;
                }
                else
                {
                    var htmlHelper = new HtmlHelper(helper.ViewContext, helper.ViewDataContainer, helper.RouteCollection);

                    var link = htmlHelper.ActionLink(breadCrumbPage.TabName, "Index", "Page", new { page = breadCrumbPage }, null);
                    breadCrumbItemBuilder.InnerHtml = link.ToHtmlString();
                }

                breadCrumbBuilder.InnerHtml += breadCrumbItemBuilder.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(breadCrumbBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString RenderLogo(this DnnHelper helper, string text)
        {
            var site = helper.ActiveSite;

            var logoBuilder = new TagBuilder("h1");

            if (!String.IsNullOrEmpty(site.LogoFile))
            {
                var logoFile = FileManager.Instance.GetFile(site.PortalID, site.LogoFile);

                if (logoFile != null)
                {
                    var imageUrl = Path.Combine(Globals.ApplicationPath, site.HomeDirectory, logoFile.RelativePath).Replace("\\", "/");
                    var imageBuilder = new TagBuilder("img");
                    imageBuilder.MergeAttribute("src", imageUrl);
                    imageBuilder.MergeAttribute("alt", site.PortalName);

                    logoBuilder.InnerHtml += imageBuilder.ToString(TagRenderMode.Normal);
                }

                if (!String.IsNullOrEmpty(text))
                {
                    logoBuilder.InnerHtml += text;
                }
            }

            return new MvcHtmlString(logoBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString RenderMenu(this DnnHelper helper, IList<TabInfo> pages)
        {
            var currentPage = helper.ActivePage;

            var menuBuilder = new TagBuilder("ul");
            menuBuilder.MergeAttribute("class", "nav navbar-nav");

            foreach (var page in pages)
            {
                var menuItemBuilder = new TagBuilder("li");

                var htmlHelper = new HtmlHelper(helper.ViewContext, helper.ViewDataContainer, helper.RouteCollection);

                if (page.HasChildren)
                {
                    menuItemBuilder.MergeAttribute("class", "dropdown");

                    var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
                    var url = urlHelper.Action("Index", "Page", new { page = page });

                    var linkBuilder = new TagBuilder("a");
                    linkBuilder.MergeAttribute("class", "dropdown-toggle");
                    linkBuilder.MergeAttribute("data-toggle", "dropdown");
                    linkBuilder.MergeAttribute("a", url);
                    linkBuilder.InnerHtml += page.TabName;
                    linkBuilder.InnerHtml += "&nbsp;";

                    var caretBuilder = new TagBuilder("span");
                    caretBuilder.MergeAttribute("class", "caret");

                    linkBuilder.InnerHtml += caretBuilder.ToString(TagRenderMode.Normal);

                    menuItemBuilder.InnerHtml += linkBuilder.ToString(TagRenderMode.Normal);
                  
                    var childMenuBuilder = new TagBuilder("ul");
                    childMenuBuilder.MergeAttribute("class", "dropdown-menu");
                    childMenuBuilder.MergeAttribute("role", "menu");

                    foreach (var childPage in TabController.Instance.GetTabsByPortal(page.PortalID).WithParentId(page.TabID))
                    {
                        var childMenuItemBuilder = new TagBuilder("li");
                        var link = htmlHelper.ActionLink(childPage.TabName, "Index", "Page", new { page = childPage }, null);
                        childMenuItemBuilder.InnerHtml = link.ToHtmlString();

                        childMenuBuilder.InnerHtml += childMenuItemBuilder.ToString(TagRenderMode.Normal);
                    }

                    menuItemBuilder.InnerHtml += childMenuBuilder.ToString(TagRenderMode.Normal);
                }
                else
                {
                    if (page.TabID == currentPage.TabID)
                    {
                        menuItemBuilder.MergeAttribute("class", "active");
                    }

                    var link = htmlHelper.ActionLink(page.TabName, "Index", "Page", new { page = page }, null);
                    menuItemBuilder.InnerHtml = link.ToHtmlString();
                }
 
                menuBuilder.InnerHtml += menuItemBuilder.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(menuBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString RenderModule(this DnnHelper<ModuleRequestResult> helper)
        {
            var moduleResult = helper.ViewData.Model;

            MvcHtmlString moduleOutput;

            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                var moduleExecutionEngine = ComponentFactory.GetComponent<IModuleExecutionEngine>();
                RenderWithinCommentedBlock(writer,
                                                "Body",
                                                moduleResult.Module.ModuleID,
                                                () => moduleExecutionEngine.ExecuteModuleResult(
                                                    helper.ViewContext.HttpContext.GetSiteContext(),
                                                    moduleResult,
                                                    writer)
                                            );
                moduleOutput = MvcHtmlString.Create(writer.ToString());
            }

            return moduleOutput;
        }

        public static MvcHtmlString RenderModuleActions(this DnnHelper<ModuleRequestResult> helper)
        {
            var request = helper.ViewContext.RequestContext.HttpContext.Request;
            var model = helper.ViewData.Model;

            var actionMenuBuilder = new TagBuilder("div");
            actionMenuBuilder.MergeAttribute("class", "btn-group");

            if (request.IsAuthenticated && model.ModuleMode != ModuleInjectMode.None)
            {
                if(ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "CONTENT", model.Module))
                {
                    if (model.ModuleMode != ModuleInjectMode.Modal)
                    {
                        var spanBuilder = new TagBuilder("span")
                                                {
                                                    InnerHtml = helper.ActionLink("Edit").ToString()
                                                };

                        actionMenuBuilder.InnerHtml = spanBuilder.ToString(TagRenderMode.Normal);
                    }
                    else
                    {
                        var buttonBuilder = new TagBuilder("button");
                        buttonBuilder.MergeAttribute("class", "btn btn-default dropdown-toggle");
                        buttonBuilder.MergeAttribute("type", "button");
                        buttonBuilder.MergeAttribute("data-toggle", "dropdown");

                        var spanBuilder = new TagBuilder("span");
                        spanBuilder.MergeAttribute("class", "caret");

                        buttonBuilder.InnerHtml += "Actions ";
                        buttonBuilder.InnerHtml += spanBuilder.ToString(TagRenderMode.Normal);

                        actionMenuBuilder.InnerHtml += buttonBuilder.ToString(TagRenderMode.Normal);

                        var menuBuilder = new TagBuilder("ul");
                        menuBuilder.MergeAttribute("class", "dropdown-menu");
                        menuBuilder.MergeAttribute("role", "menu");

                        var menuItemBuilder = new TagBuilder("li");

                        var linkBuilder = new TagBuilder("a");
                        linkBuilder.MergeAttribute("class", "module-"+ model.Module.ModuleID);
                        linkBuilder.InnerHtml = "Edit";

                        menuItemBuilder.InnerHtml += linkBuilder.ToString(TagRenderMode.Normal);

                        menuBuilder.InnerHtml += menuItemBuilder.ToString(TagRenderMode.Normal);

                        actionMenuBuilder.InnerHtml += menuBuilder.ToString(TagRenderMode.Normal);
                    }
                }
            }

            return new MvcHtmlString(actionMenuBuilder.ToString(TagRenderMode.Normal));
        }

        public static void RenderPane(this DnnHelper<PageViewModel> helper, string paneName)
        {
            var htmlHelper = new HtmlHelper(helper.ViewContext, helper.ViewDataContainer, helper.RouteCollection);

            var model = helper.ViewData.Model;
            if (model.Panes.ContainsKey(paneName))
            {
                foreach (var moduleResult in model.Panes[paneName].ModuleResults)
                {
                    htmlHelper.RenderPartial("Module", moduleResult);
                }
            }
        }

        private static void RenderWithinCommentedBlock(TextWriter writer, string blockName, int? moduleId, Action renderAction)
        {
            writer.WriteLine();
            writer.WriteLine("<!-- Start Module{0}{1} -->", moduleId.ToFormattedString("#{0}", String.Empty), blockName);
            renderAction();
            writer.WriteLine();
            writer.WriteLine("<!-- End Module{0}{1} -->", moduleId.ToFormattedString("#{0}", String.Empty), blockName);
        }

        private static string ToFormattedString<T>(this T? nullable, string formatString, string nullString) where T : struct
        {
            return nullable.HasValue ? String.Format(formatString, nullable.Value) : nullString;
        }

        private static string Action(DnnHelper helper, ModuleInfo module, string actionName, string controllerName)
        {
            var routeValues = new RouteValueDictionary
                            {
                                {"moduleId", module.ModuleID},
                                {"moduleRoute", String.Format("{0}/{1}/{2}", controllerName, actionName, module.ModuleID)}
                            };

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);

            return urlHelper.Action(actionName, controllerName, routeValues);
        }

        public static MvcHtmlString ActionLink(DnnHelper helper, ModuleInfo module, string actionName, string controllerName)
        {
            var routeValues = new RouteValueDictionary
                            {
                                {"moduleId", module.ModuleID},
                                {"moduleRoute", "Html/Edit/" + module.ModuleID}
                            };

            var htmlHelper = new HtmlHelper(helper.ViewContext, helper.ViewDataContainer, helper.RouteCollection);

            return htmlHelper.ActionLink(actionName, controllerName, routeValues);
        }
    }
}