using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using WebSiteWithCMS.Controllers;
using WebSiteWithCMS.Models;

namespace WebSiteWithCMS.Views
{
    //public abstract class BaseViewT<T> : BaseView
    public abstract class BaseView<T> : WebViewPage<T>
    {
    //    private string ContentFolder = CMSController.ContentFolder;
    //    private string classNameText = "g-text";
    //    private DOMElements DomElements { get; set; }

    //    //public BaseView():base()
    //    //{

    //    //}

    //    public string GetPageId()
    //    {
    //        return this.ViewContext.RouteData.Values["controller"].ToString() + "-" + this.ViewContext.RouteData.Values["action"].ToString();
    //    }

    //    public override void ExecutePageHierarchy()
    //    {
    //        var tmp = this.OutputStack.Pop();
    //        var myWriter = new StringWriter();
    //        this.OutputStack.Push(myWriter);
    //        base.ExecutePageHierarchy();
    //        tmp.Write(GetUpdatedHTML(myWriter.ToString()));
    //    }

    //    private string GetUpdatedHTML(string html)
    //    {
    //        this.DomElements = GetDOMElementsFromFile();
    //        var doc = new HtmlDocument();
    //        doc.LoadHtml(html);
    //        var htmlNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, '"+ classNameText + "')]");

    //        if (htmlNodes != null)
    //        {
    //            foreach (var node in htmlNodes)
    //            {
    //                node.InnerHtml = GetContent(node.Id, node.InnerText);
    //            }
    //        }

    //        return doc.DocumentNode.OuterHtml;
    //    }

    //    private DOMElements GetDOMElementsFromFile()
    //    {
    //        if (this.DomElements == null)
    //        {
    //            //DOMElements domElements = new DOMElements();
    //            string fileName = HttpContext.Current.Request.PhysicalApplicationPath + "/" + ContentFolder + "/" + GetPageId() + ".json";
    //            return CMSController.GetDOMElementsFromFile(fileName);
    //            //string fileName = GetPageId();
    //            //using (Stream input = new FileStream(HttpContext.Current.Request.PhysicalApplicationPath + "/" + ContentFolder + "/" + fileName + ".json", FileMode.Open))
    //            //{
    //            //    string dataJSON = new StreamReader(input).ReadToEnd();
    //            //    domElements = Newtonsoft.Json.JsonConvert.DeserializeObject<DOMElements>(dataJSON);
    //            //}
    //            //return domElements;
    //        }
    //        return this.DomElements;
    //    }

    //    private string GetContent(string Id, string Content)
    //    {
    //        try
    //        {
    //            if (this.DomElements != null && this.DomElements.Elements.Count > 0)
    //            {
    //                Element el = this.DomElements.Elements.Single(item => item.id == Id);
    //                return el.content;
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            return Content;
    //        }
            
    //        return Content;
    //    }


    //    //CSHTML İÇİNE YAZILAN --> GetContent("h1", "Header-1") fonksiyonu:
    //    //Bunun yerine BaseView'de dosyayı bir kez okuma ve html render edilirken dosyadaki değerleri yazma yöntemi kullanıldı.
    //    //----------------------------------------------------------------------------------------------------------------------
    //    //public string GetContent(string Id, string Content)
    //    //{
    //    //    try
    //    //    {
    //    //        string fileName = GetPageId();
    //    //        using (Stream input = new FileStream(HttpContext.Current.Request.PhysicalApplicationPath + "/" + ContentFolder + "/" + fileName + ".json", FileMode.Open))
    //    //        {
    //    //            string dataJSON = new StreamReader(input).ReadToEnd();
    //    //            DOMElements domElements = Newtonsoft.Json.JsonConvert.DeserializeObject<DOMElements>(dataJSON);

    //    //            if (domElements.Elements.Count > 0)
    //    //            {
    //    //                try
    //    //                {
    //    //                    Element el = domElements.Elements.Single(item => item.id == Id);
    //    //                    return el.content;
    //    //                }
    //    //                catch (System.InvalidOperationException)
    //    //                {
    //    //                }
    //    //            }
    //    //        }
    //    //        return Content;
    //    //    }
    //    //    catch (Exception)
    //    //    {
    //    //        return Content;
    //    //    }
    //    //}

    }

}