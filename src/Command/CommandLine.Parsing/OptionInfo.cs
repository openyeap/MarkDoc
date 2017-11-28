using CommandLine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
namespace CommandLine.Parsing
{
	[DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
	internal sealed class OptionInfo
	{
		private readonly CultureInfo _parsingCulture;
		private readonly BaseOptionAttribute _attribute;
		private readonly PropertyInfo _property;
		private readonly PropertyWriter _propertyWriter;
		private readonly bool _required;
		private readonly char? _shortName;
		private readonly string _longName;
		private readonly string _mutuallyExclusiveSet;
		private readonly object _defaultValue;
		private readonly bool _hasDefaultValue;
		public char? ShortName
		{
			get
			{
				return this._shortName;
			}
		}
		public string LongName
		{
			get
			{
				return this._longName;
			}
		}
		public string MutuallyExclusiveSet
		{
			get
			{
				return this._mutuallyExclusiveSet;
			}
		}
		public bool Required
		{
			get
			{
				return this._required;
			}
		}
		public bool IsBoolean
		{
			get
			{
				return this._property.PropertyType == typeof(bool);
			}
		}
		public bool IsArray
		{
			get
			{
				return this._property.PropertyType.IsArray;
			}
		}
		public bool IsAttributeArrayCompatible
		{
			get
			{
				return this._attribute is OptionArrayAttribute;
			}
		}
		public bool IsDefined
		{
			get;
			set;
		}
		public bool ReceivedValue
		{
			get;
			private set;
		}
		public bool HasBothNames
		{
			get
			{
				char? shortName = this._shortName;
				return (shortName.HasValue ? new int?((int)shortName.GetValueOrDefault()) : null).HasValue && this._longName != null;
			}
		}
		public bool HasParameterLessCtor
		{
			get;
			set;
		}
		public OptionInfo(BaseOptionAttribute attribute, PropertyInfo property, CultureInfo parsingCulture)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute", "The attribute is mandatory.");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property", "The property is mandatory.");
			}
			this._required = attribute.Required;
			this._shortName = attribute.ShortName;
			this._longName = attribute.LongName;
			this._mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
			this._defaultValue = attribute.DefaultValue;
			this._hasDefaultValue = attribute.HasDefaultValue;
			this._attribute = attribute;
			this._property = property;
			this._parsingCulture = parsingCulture;
			this._propertyWriter = new PropertyWriter(this._property, this._parsingCulture);
		}
		internal OptionInfo(char? shortName, string longName)
		{
			this._shortName = shortName;
			this._longName = longName;
		}
		public object GetValue(object target)
		{
			return this._property.GetValue(target, null);
		}
		public object CreateInstance(object target)
		{
			object obj = null;
			try
			{
				obj = Activator.CreateInstance(this._property.PropertyType);
				this._property.SetValue(target, obj, null);
			}
			catch (Exception innerException)
			{
				throw new ParserException("Instance defined for verb command could not be created.", innerException);
			}
			return obj;
		}
		public bool SetValue(string value, object options)
		{
			if (this._attribute is OptionListAttribute)
			{
				return this.SetValueList(value, options);
			}
			if (ReflectionHelper.IsNullableType(this._property.PropertyType))
			{
				return this.ReceivedValue = this._propertyWriter.WriteNullable(value, options);
			}
			return this.ReceivedValue = this._propertyWriter.WriteScalar(value, options);
		}
		public bool SetValue(IList<string> values, object options)
		{
			Type elementType = this._property.PropertyType.GetElementType();
			Array array = Array.CreateInstance(elementType, values.Count);
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					array.SetValue(Convert.ChangeType(values[i], elementType, this._parsingCulture), i);
					this._property.SetValue(options, array, null);
				}
				catch (FormatException)
				{
					return false;
				}
			}
			return this.ReceivedValue = true;
		}
		public bool SetValue(bool value, object options)
		{
			this._property.SetValue(options, value, null);
			return this.ReceivedValue = true;
		}
		public void SetDefault(object options)
		{
			if (this._hasDefaultValue)
			{
				try
				{
					this._property.SetValue(options, this._defaultValue, null);
				}
				catch (Exception innerException)
				{
					throw new ParserException("Bad default value.", innerException);
				}
			}
		}
		private bool SetValueList(string value, object options)
		{
			this._property.SetValue(options, new List<string>(), null);
			IList<string> list = (IList<string>)this._property.GetValue(options, null);
			string[] array = value.Split(new char[]
			{
				((OptionListAttribute)this._attribute).Separator
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string item = array2[i];
				list.Add(item);
			}
			return this.ReceivedValue = true;
		}
	}
}
