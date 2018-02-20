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
        private static string classNameImage = "g-image";

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

                if (domElements.Elements.Count > 0 || domElements.Images.Count > 0)
                {
                    string path = HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder;
                    string fileName = "";

                    if (domElements.Elements.Count > 0) fileName = path + "/" + domElements.Elements[0].pageid + ".json";
                    else fileName = path + "/" + domElements.Images[0].pageid + ".json";

                    domElementsExisting = GetDOMElementsFromFile(fileName);

                    //Elements
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

                    //Images
                    if (domElementsExisting != null && domElementsExisting.Images != null)
                    {
                        foreach (Image im in domElementsExisting.Images)
                        {
                            try
                            {
                                Image tmp = domElements.Images.Single(item => item.id == im.id);
                            }
                            catch (Exception)
                            {
                                domElements.Images.Add(im);
                            }
                        }
                    }

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(domElements);
                    System.IO.File.WriteAllText(fileName, jsonString);

                    return Content("-Text content saved successfully.");
                }

                else { return Content("-Text changes can not be detected."); }
            }
            catch (Exception ex)
            {
                return Content(ex.HResult + " " + ex.Message);
            }
        }

        //SAVE IMAGE FILES
        [HttpPost]
        public ActionResult UpdateImages()
        {
            string result = "";
            try
            {
                if (Request.Files.Count > 0)
                {
                    result += " -File Count: " + Request.Files.Count.ToString();
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        result += " -File Content Length " + fileContent.ContentLength.ToString();
                        if (fileContent != null && fileContent.ContentLength > 0)
                        {
                            // get a stream 
                            var stream = fileContent.InputStream;
                            // and optionally write the file to disk 
                            var fileName = Path.GetFileName(file);

                            string path = HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder + "/img";
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                            path += "/" + fileName;

                            result += " -Path: " + path;

                            using (var fileStream = System.IO.File.Create(path))
                            {
                                stream.CopyTo(fileStream);
                            }
                        }
                    }

                    return Content("-Images saved successfully.");
                    //return Content(result + " -Files saved successfully.");
                }
                else { return Content("-Image changes can not be detected."); }

                ////Stream input = this.Request.InputStream;
                ////using (StreamReader streamReader = new StreamReader(input))
                //using (Stream output = new FileStream(HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder + "/test.jpg", FileMode.Create))
                //{
                //    //StreamWriter streamWriter = new StreamWriter(HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder + "/test.jpg");

                //    //System.IO.File.WriteAllBytes

                //    var buffer = new byte[2048];
                //    var bytesRead = default(int);
                //    while ((bytesRead = this.Request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        output.Write(buffer, 0, bytesRead);
                //    }
                //}
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
            DOMElements DomElements = GetDOMElementsFromFile(fileName);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //Change Text Element's Content
            var htmlNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, '" + classNameText + "')]");
            if (htmlNodes != null)
            {
                foreach (var node in htmlNodes)
                {
                    node.InnerHtml = GetContent(DomElements, node.Id, node.InnerText);
                }
            }

            //Change Image Element's Background
            htmlNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, '" + classNameImage + "')]");
            if (htmlNodes != null)
            {
                foreach (var node in htmlNodes)
                {
                    string imageFileName = GetImageFileName(DomElements, node.Id);
                    if (imageFileName != string.Empty)
                    {
                        string imagePath = "../" + ContentFolder + "/img/" + imageFileName;
                        node.Attributes.Append("style", "background-image:url('" + imagePath + "')");
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        //SAVE IMAGE FILES
        [HttpPost]
        public ActionResult GetLastImage(string pageid, string id)
        {
            try
            {
                DOMElements domElements = new DOMElements();
                string path = HttpContext.Request.PhysicalApplicationPath + "/" + ContentFolder;
                string fileName = path + "/" + pageid + ".json";
                domElements = GetDOMElementsFromFile(fileName);

                string imageFileName = GetImageFileName(domElements, id);

                if (imageFileName != string.Empty)
                {
                    return Content("../" + ContentFolder + "/img/" + imageFileName);
                }
                else
                {
                    return Content("");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.HResult + " " + ex.Message);
            }
        }

        private static string GetContent(DOMElements DomElements, string Id, string Content)
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

        private static string GetImageFileName(DOMElements DomElements, string Id)
        {
            try
            {
                if (DomElements != null && DomElements.Images.Count > 0)
                {
                    Image im = DomElements.Images.Single(item => item.id == Id);
                    return im.updatedFileName;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

    }
}