using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Dnn.Mvc.Modules.Html.Models;
using Dnn.Mvc.Web.Framework.Modules;

using DotNetNuke.Common;
using DotNetNuke.Data;

namespace Dnn.Mvc.Modules.Html.Controllers
{
    public class HtmlController : DnnController
    {
        private IDataContext _dataContext;

        public HtmlController() : this(DataContext.Instance()) { }

        public HtmlController(IDataContext dataContext) : base()
        {
            Requires.NotNull("dataContext", dataContext);

            _dataContext = dataContext;
        }

        public ActionResult Index()
        {
            HtmlText content;

            using (_dataContext)
            {
                var rep = _dataContext.GetRepository<HtmlText>();

                content = rep.Find("WHERE ModuleID = " + ActiveModule.ModuleID)
                                .OrderByDescending(c => c.Version)
                                .FirstOrDefault();

                if (!String.IsNullOrEmpty(content.Content))
                {
                    content.Content = HttpUtility.HtmlDecode(content.Content);
                }
            }

            return View(content);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            HtmlText content;

            using (_dataContext)
            {
                var rep = _dataContext.GetRepository<HtmlText>();

                content = rep.Find("WHERE ModuleID = " + ActiveModule.ModuleID)
                                .OrderByDescending(c => c.Version)
                                .FirstOrDefault();

                content.Content = HttpUtility.HtmlDecode(content.Content);

            }

            return View(content);
        }

        [HttpPost]
        public ActionResult Edit(HtmlText item)
        {
            using (_dataContext)
            {
                var rep = _dataContext.GetRepository<HtmlText>();

                rep.Update(item);

                return Json(new { Success = true });
            }
        }
    }
}