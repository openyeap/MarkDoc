using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bzway.DotNetBook.ePub
{
    /// <summary>
    /// Dublin Core
    /// </summary>
    class DCItem
    {
        private string _name;
        private string _value;

        private IDictionary<string, string> _attributes;
        private IDictionary<string, string> _opfAttributes;

        internal DCItem(string name, string value)
        {
            _name = name;
            _value = value;
            _attributes = new Dictionary<string, string>();
            _opfAttributes = new Dictionary<string, string>();

        }

        internal void SetAttribute(string name, string value)
        {
            _attributes.Add(name, value);
        }

        internal void SetOpfAttribute(string name, string value)
        {
            _opfAttributes.Add(name, value);
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(EBook.DcNS + _name, _value);
            foreach (string key in _opfAttributes.Keys)
            {
                string value = _opfAttributes[key];
                element.SetAttributeValue(EBook.OpfNS + key, value);
            }
            foreach (string key in _attributes.Keys)
            {
                string value = _attributes[key];
                element.SetAttributeValue(key, value);
            }
            return element;
        }
    }
}
