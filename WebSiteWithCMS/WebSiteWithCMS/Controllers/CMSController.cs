using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteWithCMS.Models;
using HtmlAgilityPack;

namespace WebSiteWithCMS.Controllers
{
    public class CMSController : Controller
    {
        public static string ContentFolder = "WebContentData";
        private static string classNameText = "g-text";
        private static DOMElements DomElements { get; set; }

        //SAVE JSON FILE
        [HttpPost]
        public ActionResult UpdateData()
        {
            try
            {
                Stream input = this.Request.InputStream;
                string dataJSON = new StreamReader(input).ReadToEnd();
                DOMElements domElements = Newtonsoft.Json.JsonConvert.DeserializeObject<DOMElements>(dataJSON);
                DOMElements domElementsExisting = new DOMElements();

                if (domElements.Elements.Count > 0)
                {
                    string path = HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder;
                    string fileName = path + "/" + domElements.Elements[0].pageid + ".json";

                    domElementsExisting = GetDOMElementsFromFile(fileName);

                    if (domElementsExisting != null && domElementsExisting.Elements != null)
                    {
                        foreach (Element el in domElementsExisting.Elements)
                        {
                            try
                            {
                                Element tmp = domElements.Elements.Single(item => item.id == el.id);
                            }
                            catch (Exception)
                            {
                                domElements.Elements.Add(el);
                            }
                        }
                    }

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(domElements);
                    System.IO.File.WriteAllText(fileName, jsonString);                   

                    return Content("Saved successfully.");
                }
            
                else { return Content("Changes can not be detected."); }
            }
            catch (Exception ex)
            {
                return Content(ex.HResult + " " + ex.Message);
            }
        }

        private DOMElements GetExistingDOMElements()
        {
            return new DOMElements();
        }

        private static DOMElements GetDOMElementsFromFile(string fileName)
        {
            DOMElements domElements = new DOMElements();
            if (!new FileInfo(fileName).Exists) return domElements;

            using (Stream input = new FileStream(fileName, FileMode.Open))
            {
                string dataJSON = new StreamReader(input).ReadToEnd();
                domElements = Newtonsoft.Json.JsonConvert.DeserializeObject<DOMElements>(dataJSON);
            }
            return domElements;
        }

        public static string GetUpdatedHTML(string html, string AppPath, string PageId)
        {
            string fileName = AppPath + "/" + ContentFolder + "/" + PageId + ".json";
            DomElements = GetDOMElementsFromFile(fileName);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var htmlNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, '" + classNameText + "')]");

            if (htmlNodes != null)
            {
                foreach (var node in htmlNodes)
                {
                    node.InnerHtml = GetContent(node.Id, node.InnerText);
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        private static string GetContent(string Id, string Content)
        {
            try
            {
                if (DomElements != null && DomElements.Elements.Count > 0)
                {
                    Element el = DomElements.Elements.Single(item => item.id == Id);
                    return el.content;
                }
            }
            catch (Exception)
            {
                return Content;
            }

            return Content;
        }

    }
}