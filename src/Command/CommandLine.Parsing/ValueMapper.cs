using CommandLine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
namespace CommandLine.Parsing
{
	internal sealed class ValueMapper
	{
		private readonly CultureInfo _parsingCulture;
		private readonly object _target;
		private IList<string> _valueList;
		private ValueListAttribute _valueListAttribute;
		private IList<Pair<PropertyInfo, ValueOptionAttribute>> _valueOptionAttributeList;
		private int _valueOptionIndex;
		public bool CanReceiveValues
		{
			get
			{
				return this.IsValueListDefined || this.IsValueOptionDefined;
			}
		}
		private bool IsValueListDefined
		{
			get
			{
				return this._valueListAttribute != null;
			}
		}
		private bool IsValueOptionDefined
		{
			get
			{
				return this._valueOptionAttributeList.Count > 0;
			}
		}
		public ValueMapper(object target, CultureInfo parsingCulture)
		{
			this._target = target;
			this._parsingCulture = parsingCulture;
			this.InitializeValueList();
			this.InitializeValueOption();
		}
		public bool MapValueItem(string item)
		{
			if (!this.IsValueOptionDefined || this._valueOptionIndex >= this._valueOptionAttributeList.Count)
			{
				return this.IsValueListDefined && this.AddValueItem(item);
			}
			Pair<PropertyInfo, ValueOptionAttribute> pair = this._valueOptionAttributeList[this._valueOptionIndex++];
			PropertyWriter propertyWriter = new PropertyWriter(pair.Left, this._parsingCulture);
			if (!ReflectionHelper.IsNullableType(propertyWriter.Property.PropertyType))
			{
				return propertyWriter.WriteScalar(item, this._target);
			}
			return propertyWriter.WriteNullable(item, this._target);
		}
		private bool AddValueItem(string item)
		{
			if (this._valueListAttribute.MaximumElements == 0 || this._valueList.Count == this._valueListAttribute.MaximumElements)
			{
				return false;
			}
			this._valueList.Add(item);
			return true;
		}
		private void InitializeValueList()
		{
			this._valueListAttribute = ValueListAttribute.GetAttribute(this._target);
			if (this.IsValueListDefined)
			{
				this._valueList = ValueListAttribute.GetReference(this._target);
			}
		}
		private void InitializeValueOption()
		{
			IList<Pair<PropertyInfo, ValueOptionAttribute>> list = ReflectionHelper.RetrievePropertyList<ValueOptionAttribute>(this._target);
			IList<Pair<PropertyInfo, ValueOptionAttribute>> arg_5D_1;
			if (!list.All((Pair<PropertyInfo, ValueOptionAttribute> x) => x.Right.Index == 0))
			{
				arg_5D_1 = (
					from x in list
					orderby x.Right.Index
					select x).ToList<Pair<PropertyInfo, ValueOptionAttribute>>();
			}
			else
			{
				arg_5D_1 = list;
			}
			this._valueOptionAttributeList = arg_5D_1;
		}
	}
}
