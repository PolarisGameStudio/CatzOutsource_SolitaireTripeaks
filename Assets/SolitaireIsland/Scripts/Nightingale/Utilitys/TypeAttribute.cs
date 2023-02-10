using System;

namespace Nightingale.Utilitys
{
	public class TypeAttribute : Attribute
	{
		private Type _type;

		public Type Type => _type;

		public TypeAttribute(Type type)
		{
			_type = type;
		}
	}
}
