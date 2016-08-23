using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bzway.EPubBook.Writer
{
    /// <summary>
    /// 告诉 EPUB 阅读器哪些文件属于档案
    /// </summary>
    class Manifest
    {
        XElement _element;
        internal Manifest()
        {
            _element = new XElement(EPubBook.OpfNS + "manifest");
        }

        internal void AddItem(string id, string href, string type)
        {
            XElement item = new XElement(EPubBook.OpfNS + "item");
            item.SetAttributeValue("id", id);
            item.SetAttributeValue("href", href);
            item.SetAttributeValue("media-type", type);
            _element.Add(item);
        }


        internal XElement ToElement()
        {
            return _element;
        }
    }
}
