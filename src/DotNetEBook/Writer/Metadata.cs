using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bzway.EBook.Writer;
using System.Xml.Linq;

namespace Bzway.EBook.Writer
{
    /// <summary>
    /// Dublin Core 定义了一组常用的元数据，可用于描述各种不同的数字资料
    /// 不是 EPUB 规范的一部分
    /// 所有这些术语都可以出现在 OPF 元数据部分
    /// 编写要分发的 EPUB 时，这里可以放很多内容
    /// </summary>
    class Metadata
    {
        private List<DCItem> _dcItems = new List<DCItem>();
        private List<Item> _items = new List<Item>();

        internal void AddAuthor(string name)
        {
            AddCreator(name, "aut");
        }

        internal void AddTranslator(string name)
        {
            AddCreator(name, "trl");
        }

        internal void AddSubject(string subj)
        {
            DCItem dcitem = new DCItem("subject", subj);
            _dcItems.Add(dcitem);
        }

        internal void AddDescription(string description)
        {
            DCItem dcitem = new DCItem("description", description);
            _dcItems.Add(dcitem);
        }

        internal void AddType(string @type)
        {
            DCItem dcitem = new DCItem("type", @type);
            _dcItems.Add(dcitem);
        }

        internal void AddFormat(string format)
        {
            DCItem dcitem = new DCItem("format", format);
            _dcItems.Add(dcitem);
        }

        internal void AddLanguage(string lang)
        {
            DCItem dcitem = new DCItem("language", lang);
            _dcItems.Add(dcitem);
        }

        internal void AddRelation(string relation)
        {
            DCItem dcitem = new DCItem("relation", relation);
            _dcItems.Add(dcitem);
        }

        internal void AddRights(string rights)
        {
            DCItem dcitem = new DCItem("rights", rights);
            _dcItems.Add(dcitem);
        }

        internal void AddCreator(string name, string role)
        {
            DCItem dcitem = new DCItem("creator", name);
            dcitem.SetOpfAttribute("role", role);
            _dcItems.Add(dcitem);
        }

        internal void AddCcontributor(string name, string role)
        {
            DCItem dcitem = new DCItem("contributor", name);
            dcitem.SetOpfAttribute("role", role);
            _dcItems.Add(dcitem);
        }

        internal void AddTitle(string title)
        {
            DCItem dcitem = new DCItem("title", title);
            _dcItems.Add(dcitem);
        }

        internal void AddBookIdentifier(string id, string uuid)
        {
            AddBookIdentifier(id, uuid, string.Empty);
        }

        internal void AddBookIdentifier(string id, string uuid, string scheme)
        {
            DCItem dcitem = new DCItem("identifier", uuid);
            dcitem.SetAttribute("id", id);
            if (!String.IsNullOrEmpty(scheme))
                dcitem.SetOpfAttribute("scheme", scheme);
            _dcItems.Add(dcitem);
        }

        internal void AddItem(string name, string value)
        {
            Item item = new Item(name, value);
            _items.Add(item);
        }

        internal XElement ToElement()
        {
            XNamespace dc = "http://purl.org/dc/elements/1.1/";
            XNamespace opf = "http://www.idpf.org/2007/opf";

            var element = new XElement(EPubBook.OpfNS + "metadata",
                new XAttribute(XNamespace.Xmlns + "dc", dc),
                new XAttribute(XNamespace.Xmlns + "opf", opf));

            foreach (Item i in _items)
            {
                var itemElement = i.ToElement();
                element.Add(itemElement);
            }

            foreach (DCItem i in _dcItems)
            {
                var itemElement = i.ToElement();
                element.Add(itemElement);
            }

            return element;
        }


    }
}
