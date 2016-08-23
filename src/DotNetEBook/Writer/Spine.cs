using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bzway.EPubBook.Writer
{
    /// <summary>
    /// 指定这些文件出现的顺序
    /// 按照 EPUB 的说法 — 数字图书的线性阅读顺序。
    /// 可以将 OPF spine 看作是书中 “页面” 的顺序。
    /// 按照文档顺序从上到下依次读取 spine。
    /// </summary>
    class Spine
    {
        private struct ItemRef
        {
            public string id;
            public bool linear;
        };


        private string _toc;
        private List<ItemRef> _itemRefs;

        internal Spine()
        {
            _itemRefs = new List<ItemRef>();
        }

        internal void SetToc(string toc)
        {
            _toc = toc;
        }

        internal void AddItemRef(string id, bool linear)
        {
            ItemRef r;
            r.id = id;
            r.linear = linear;
            _itemRefs.Add(r);
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(EPubBook.OpfNS + "spine");
            if (!String.IsNullOrEmpty(_toc))
                element.Add(new XAttribute("toc", _toc));
            foreach (ItemRef r in _itemRefs)
            {
                var item = new XElement(EPubBook.OpfNS + "itemref", new XAttribute("idref", r.id));
                if (!r.linear)
                    item.SetAttributeValue("linear", "no");
                element.Add(item);
            }
            return element;
        }
    }
}
