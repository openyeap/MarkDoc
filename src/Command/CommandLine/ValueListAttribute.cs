using CommandLine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace CommandLine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValueListAttribute : Attribute
	{
		private readonly Type _concreteType;
		public int MaximumElements
		{
			get;
			set;
		}
		public Type ConcreteType
		{
			get
			{
				return this._concreteType;
			}
		}
		public ValueListAttribute(Type concreteType) : this()
		{
			if (concreteType == null)
			{
				throw new ArgumentNullException("concreteType");
			}
			if (!typeof(IList<string>).IsAssignableFrom(concreteType))
			{
				throw new ParserException("The types are incompatible.");
			}
			this._concreteType = concreteType;
		}
		private ValueListAttribute()
		{
			this.MaximumElements = -1;
		}
		internal static IList<string> GetReference(object target)
		{
			Type type;
			PropertyInfo property = ValueListAttribute.GetProperty(target, out type);
			if (property == null || type == null)
			{
				return null;
			}
			property.SetValue(target, Activator.CreateInstance(type), null);
			return (IList<string>)property.GetValue(target, null);
		}
		internal static ValueListAttribute GetAttribute(object target)
		{
			IList<Pair<PropertyInfo, ValueListAttribute>> list = ReflectionHelper.RetrievePropertyList<ValueListAttribute>(target);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if (list.Count > 1)
			{
				throw new InvalidOperationException();
			}
			Pair<PropertyInfo, ValueListAttribute> pair = list[0];
			return pair.Right;
		}
		private static PropertyInfo GetProperty(object target, out Type concreteType)
		{
			concreteType = null;
			IList<Pair<PropertyInfo, ValueListAttribute>> list = ReflectionHelper.RetrievePropertyList<ValueListAttribute>(target);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if (list.Count > 1)
			{
				throw new InvalidOperationException();
			}
			Pair<PropertyInfo, ValueListAttribute> pair = list[0];
			concreteType = pair.Right.ConcreteType;
			return pair.Left;
		}
	}
}
