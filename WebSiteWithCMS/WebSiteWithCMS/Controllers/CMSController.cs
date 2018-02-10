using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteWithCMS.Models;
using WebSiteWithCMS.Filters;

namespace WebSiteWithCMS.Controllers
{
    public class CMSController : Controller
    {
        public static string ContentFolder = "WebContentData";

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

                    //input.Seek(0, SeekOrigin.Begin);
                    //using (Stream output = new FileStream(fileName, FileMode.Create))
                    //{
                    //var bytes = new byte[8092];
                    //int dataRead;
                    //while ((dataRead = input.Read(bytes, 0, bytes.Length)) > 0)
                    //    output.Write(bytes, 0, dataRead);

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

        public static DOMElements GetDOMElementsFromFile(string fileName)
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

    }
}