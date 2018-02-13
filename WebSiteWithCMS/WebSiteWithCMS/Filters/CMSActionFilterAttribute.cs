using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSiteWithCMS.Filters { 

    public class CMSActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Write("merhaba");

            using (StreamReader sr = new StreamReader(filterContext.HttpContext.Response.OutputStream))
            {
                string html = sr.ReadToEnd();
                filterContext.HttpContext.Response.Output.Write(html);
            }
            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //filterContext.
            base.OnResultExecuting(filterContext);

            
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //var bytes = new byte[8092];
            //int dataRead;
            //while ((dataRead = filterContext.HttpContext.Response.OutputStream.Read(bytes, 0, bytes.Length)) > 0)


                base.OnResultExecuted(filterContext);

            //using (Stream input = filterContext.HttpContext.Response.OutputStream)
            //{
            //    var bytes = new byte[8092];
            //    int dataRead;
            //    while ((dataRead = input.Read(bytes, 0, bytes.Length)) > 0)
            //        filterContext.HttpContext.Response.Output.Write

                //using (StreamReader sr = new StreamReader(filterContext.HttpContext.Response.OutputStream))
                //{
                //    string html = sr.ReadToEnd();
                //    filterContext.HttpContext.Response.Output.Write(html);
                //}
                

                //filterContext.ParentActionViewContext

                //filterContext.ParentActionViewContext
            }
    }
}