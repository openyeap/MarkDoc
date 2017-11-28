using System;
using System.Collections.Generic;
using System.Reflection;
namespace CommandLine.Infrastructure
{
	internal static class ReflectionHelper
	{
		internal static Assembly AssemblyFromWhichToPullInformation
		{
			get;
			set;
		}
		static ReflectionHelper()
		{
			ReflectionHelper.AssemblyFromWhichToPullInformation = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
		}
		public static IList<Pair<PropertyInfo, TAttribute>> RetrievePropertyList<TAttribute>(object target) where TAttribute : Attribute
		{
			Pair<Type, object> key = new Pair<Type, object>(typeof(Pair<PropertyInfo, TAttribute>), target);
			object obj = ReflectionCache.Instance[key];
			if (obj == null)
			{
				IList<Pair<PropertyInfo, TAttribute>> list = new List<Pair<PropertyInfo, TAttribute>>();
				if (target != null)
				{
					PropertyInfo[] properties = target.GetType().GetProperties();
					PropertyInfo[] array = properties;
					for (int i = 0; i < array.Length; i++)
					{
						PropertyInfo propertyInfo = array[i];
						if (!(propertyInfo == null) && propertyInfo.CanRead && propertyInfo.CanWrite)
						{
							MethodInfo setMethod = propertyInfo.GetSetMethod();
							if (!(setMethod == null) && !setMethod.IsStatic)
							{
								Attribute customAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(TAttribute), false);
								if (customAttribute != null)
								{
									list.Add(new Pair<PropertyInfo, TAttribute>(propertyInfo, (TAttribute)((object)customAttribute)));
								}
							}
						}
					}
				}
				ReflectionCache.Instance[key] = list;
				return list;
			}
			return (IList<Pair<PropertyInfo, TAttribute>>)obj;
		}
		public static Pair<MethodInfo, TAttribute> RetrieveMethod<TAttribute>(object target) where TAttribute : Attribute
		{
			Pair<Type, object> key = new Pair<Type, object>(typeof(Pair<MethodInfo, TAttribute>), target);
			object obj = ReflectionCache.Instance[key];
			if (obj == null)
			{
				MethodInfo[] methods = target.GetType().GetMethods();
				MethodInfo[] array = methods;
				for (int i = 0; i < array.Length; i++)
				{
					MethodInfo methodInfo = array[i];
					if (!methodInfo.IsStatic)
					{
						Attribute customAttribute = Attribute.GetCustomAttribute(methodInfo, typeof(TAttribute), false);
						if (customAttribute != null)
						{
							Pair<MethodInfo, TAttribute> pair = new Pair<MethodInfo, TAttribute>(methodInfo, (TAttribute)((object)customAttribute));
							ReflectionCache.Instance[key] = pair;
							return pair;
						}
					}
				}
				return null;
			}
			return (Pair<MethodInfo, TAttribute>)obj;
		}
		public static TAttribute RetrieveMethodAttributeOnly<TAttribute>(object target) where TAttribute : Attribute
		{
			Pair<Type, object> key = new Pair<Type, object>(typeof(TAttribute), target);
			object obj = ReflectionCache.Instance[key];
			if (obj == null)
			{
				MethodInfo[] methods = target.GetType().GetMethods();
				MethodInfo[] array = methods;
				for (int i = 0; i < array.Length; i++)
				{
					MethodInfo methodInfo = array[i];
					if (!methodInfo.IsStatic)
					{
						Attribute customAttribute = Attribute.GetCustomAttribute(methodInfo, typeof(TAttribute), false);
						if (customAttribute != null)
						{
							TAttribute tAttribute = (TAttribute)((object)customAttribute);
							ReflectionCache.Instance[key] = tAttribute;
							return tAttribute;
						}
					}
				}
				return default(TAttribute);
			}
			return (TAttribute)((object)obj);
		}
		public static IList<TAttribute> RetrievePropertyAttributeList<TAttribute>(object target) where TAttribute : Attribute
		{
			Pair<Type, object> key = new Pair<Type, object>(typeof(IList<TAttribute>), target);
			object obj = ReflectionCache.Instance[key];
			if (obj == null)
			{
				IList<TAttribute> list = new List<TAttribute>();
				PropertyInfo[] properties = target.GetType().GetProperties();
				PropertyInfo[] array = properties;
				for (int i = 0; i < array.Length; i++)
				{
					PropertyInfo propertyInfo = array[i];
					if (!(propertyInfo == null) && propertyInfo.CanRead && propertyInfo.CanWrite)
					{
						MethodInfo setMethod = propertyInfo.GetSetMethod();
						if (!(setMethod == null) && !setMethod.IsStatic)
						{
							Attribute customAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(TAttribute), false);
							if (customAttribute != null)
							{
								list.Add((TAttribute)((object)customAttribute));
							}
						}
					}
				}
				ReflectionCache.Instance[key] = list;
				return list;
			}
			return (IList<TAttribute>)obj;
		}
		public static TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
		{
			object[] customAttributes = ReflectionHelper.AssemblyFromWhichToPullInformation.GetCustomAttributes(typeof(TAttribute), false);
			if (customAttributes.Length <= 0)
			{
				return default(TAttribute);
			}
			return (TAttribute)((object)customAttributes[0]);
		}
		public static Pair<PropertyInfo, TAttribute> RetrieveOptionProperty<TAttribute>(object target, string uniqueName) where TAttribute : BaseOptionAttribute
		{
			Pair<Type, object> key = new Pair<Type, object>(typeof(Pair<PropertyInfo, BaseOptionAttribute>), target);
			object obj = ReflectionCache.Instance[key];
			if (obj == null)
			{
				if (target == null)
				{
					return null;
				}
				PropertyInfo[] properties = target.GetType().GetProperties();
				PropertyInfo[] array = properties;
				for (int i = 0; i < array.Length; i++)
				{
					PropertyInfo propertyInfo = array[i];
					if (!(propertyInfo == null) && propertyInfo.CanRead && propertyInfo.CanWrite)
					{
						MethodInfo setMethod = propertyInfo.GetSetMethod();
						if (!(setMethod == null) && !setMethod.IsStatic)
						{
							Attribute customAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(TAttribute), false);
							TAttribute tAttribute = (TAttribute)((object)customAttribute);
							if (tAttribute != null && string.CompareOrdinal(uniqueName, tAttribute.UniqueName) == 0)
							{
								Pair<PropertyInfo, TAttribute> pair = new Pair<PropertyInfo, TAttribute>(propertyInfo, (TAttribute)((object)customAttribute));
								ReflectionCache.Instance[key] = pair;
								return pair;
							}
						}
					}
				}
			}
			return (Pair<PropertyInfo, TAttribute>)obj;
		}
		public static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}
