using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bzway.DotNetBook.ePub
{
    /// <summary>
    /// OPF 内容文件的最后一部分是 guide。这一节是可选的，但最好保留。
    /// </summary>
    class Guide
    {
        private XElement _element;

        internal Guide()
        {
            _element = new XElement(EBook.OpfNS + "guide");
        }

        internal void AddReference(string href, string type)
        {
            AddReference(href, type, String.Empty);
        }

        internal void AddReference(string href, string type, string title)
        {
            var itemref = new XElement(EBook.OpfNS + "reference",
                new XAttribute("href", href), new XAttribute("type", type), new XAttribute("title", title));
            if (!String.IsNullOrEmpty(title))
                itemref.SetAttributeValue("title", title);
            _element.Add(itemref);
        }

        internal XElement ToElement()
        {
            return _element;
        }
    }
}
