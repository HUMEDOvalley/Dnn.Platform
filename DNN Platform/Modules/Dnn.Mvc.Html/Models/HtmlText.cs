using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using DotNetNuke.ComponentModel.DataAnnotations;

namespace Dnn.Mvc.Modules.Html.Models
{
    [TableName("HtmlText")]
    [PrimaryKey("ItemID")]
    public class HtmlText
    {
        public int ItemID { get; set; }

        public int ModuleID { get; set; }

        [AllowHtml]
        [DisplayFormat(HtmlEncode = false)]
        public string Content { get; set; }

        public int Version { get; set; }
    }
}