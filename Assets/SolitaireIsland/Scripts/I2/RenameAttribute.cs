using UnityEngine;

namespace I2
{
	public class RenameAttribute : PropertyAttribute
	{
		public readonly string Name;

		public readonly string Tooltip;

		public readonly int HorizSpace;

		public RenameAttribute(int hspace, string name, string tooltip = null)
		{
			Name = name;
			Tooltip = tooltip;
			HorizSpace = hspace;
		}

		public RenameAttribute(string name, string tooltip = null)
			: this(0, name, tooltip)
		{
		}
	}
}
