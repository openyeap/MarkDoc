using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
namespace CommandLine.Parsing
{
	internal sealed class PropertyWriter
	{
		private readonly CultureInfo _parsingCulture;
		public PropertyInfo Property
		{
			get;
			private set;
		}
		public PropertyWriter(PropertyInfo property, CultureInfo parsingCulture)
		{
			this._parsingCulture = parsingCulture;
			this.Property = property;
		}
		public bool WriteScalar(string value, object target)
		{
			try
			{
				object value2;
				if (this.Property.PropertyType.IsEnum)
				{
					value2 = Enum.Parse(this.Property.PropertyType, value, true);
				}
				else
				{
					value2 = Convert.ChangeType(value, this.Property.PropertyType, this._parsingCulture);
				}
				this.Property.SetValue(target, value2, null);
			}
			catch (InvalidCastException)
			{
				bool result = false;
				return result;
			}
			catch (FormatException)
			{
				bool result = false;
				return result;
			}
			catch (ArgumentException)
			{
				bool result = false;
				return result;
			}
			catch (OverflowException)
			{
				bool result = false;
				return result;
			}
			return true;
		}
		public bool WriteNullable(string value, object target)
		{
			NullableConverter nullableConverter = new NullableConverter(this.Property.PropertyType);
			try
			{
				this.Property.SetValue(target, nullableConverter.ConvertFromString(null, this._parsingCulture, value), null);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
	}
}
