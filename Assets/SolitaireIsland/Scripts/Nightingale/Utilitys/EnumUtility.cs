using System;
using System.Reflection;

namespace Nightingale.Utilitys
{
	public static class EnumUtility
	{
		public static Type GetStringType(Enum value)
		{
			Type result = null;
			Type type = value.GetType();
			FieldInfo field = type.GetField(value.ToString());
			TypeAttribute[] array = field.GetCustomAttributes(typeof(TypeAttribute), inherit: false) as TypeAttribute[];
			if (array.Length > 0)
			{
				result = array[0].Type;
			}
			return result;
		}

		public static T GetEnumType<T>(string code, T defaultVaule)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), code);
			}
			catch (Exception)
			{
				return defaultVaule;
			}
		}

		public static T GetEnumType<T>(int code, T defaultVaule)
		{
			try
			{
				return (T)Enum.ToObject(typeof(T), code);
			}
			catch (Exception)
			{
				return defaultVaule;
			}
		}
	}
}
