using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteWithCMS.Models
{
    public class DOMElements
    {
        public DOMElements() { }
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        public Element() { }

        public string pageid { get; set; }
        public string id { get; set; }
        public string content { get; set; }
    }
}