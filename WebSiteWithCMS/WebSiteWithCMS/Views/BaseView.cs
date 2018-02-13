using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using WebSiteWithCMS.Controllers;

namespace WebSiteWithCMS.Views
{
    public abstract class BaseView : WebViewPage
    {
        public string GetPageId()
        {
            return this.ViewContext.RouteData.Values["controller"].ToString() + "-" + this.ViewContext.RouteData.Values["action"].ToString();
        }

        public override void ExecutePageHierarchy()
        {
            var tmp = this.OutputStack.Pop();
            var myWriter = new StringWriter();
            this.OutputStack.Push(myWriter);
            base.ExecutePageHierarchy();
            tmp.Write(CMSController.GetUpdatedHTML(myWriter.ToString(), HttpContext.Current.Request.PhysicalApplicationPath, GetPageId()));
        }
    }

    public abstract class BaseView<T> : WebViewPage<T>
    {
        public string GetPageId()
        {
            return this.ViewContext.RouteData.Values["controller"].ToString() + "-" + this.ViewContext.RouteData.Values["action"].ToString();
        }

        public override void ExecutePageHierarchy()
        {
            var tmp = this.OutputStack.Pop();
            var myWriter = new StringWriter();
            this.OutputStack.Push(myWriter);
            base.ExecutePageHierarchy();
            tmp.Write(CMSController.GetUpdatedHTML(myWriter.ToString(), HttpContext.Current.Request.PhysicalApplicationPath, GetPageId()));
        }
    }

}