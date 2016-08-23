using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bzway.DotNetBook.ePub
{
    class Item
    {
        private string _name;
        private string _value;

        internal Item(string name, string value)
        {
            _name = name;
            _value = value;
        }

        internal XElement ToElement()
        {
            var element = new XElement(EBook.OpfNS + "meta");
            element.SetAttributeValue("name", _name);
            element.SetAttributeValue("content", _value);

            return element;
        }
    }
}
