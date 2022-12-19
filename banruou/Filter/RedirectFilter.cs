using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace banruou.Filter
{
    public class RedirectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var redirect = File.ReadAllText(HostingEnvironment.MapPath(Path.Combine("/images/Redirect.txt")));
            if (!string.IsNullOrEmpty(redirect))
            {
                var items = Regex.Split(redirect, @"\n");
                if (items.Length > 0)
                {
                    foreach (var item in items)
                    {
                        var urlItem = Regex.Split(item, "->");
                        var source = urlItem[0].Trim();
                        var destination = "";
                        if (urlItem.Length > 1)
                        {
                            destination = urlItem[1].Trim();
                        }

                        var requestUrl = HttpContext.Current.Request.Url.ToString();

                        if (requestUrl.Equals(source, StringComparison.OrdinalIgnoreCase))
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.Status = "301 Moved Permanently";
                            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                            HttpContext.Current.Response.AddHeader("Location", destination);
                            HttpContext.Current.Response.End();
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}