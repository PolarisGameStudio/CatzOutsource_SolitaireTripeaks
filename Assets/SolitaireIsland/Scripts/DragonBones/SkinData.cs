using System.Collections.Generic;

namespace DragonBones
{
	public class SkinData : BaseObject
	{
		public string name;

		public readonly Dictionary<string, List<DisplayData>> displays = new Dictionary<string, List<DisplayData>>();

		public ArmatureData parent;

		protected override void _OnClear()
		{
			foreach (List<DisplayData> value in displays.Values)
			{
				foreach (DisplayData item in value)
				{
					item.ReturnToPool();
				}
			}
			name = string.Empty;
			displays.Clear();
			parent = null;
		}

		public void AddDisplay(string slotName, DisplayData value)
		{
			if (!string.IsNullOrEmpty(slotName) && value != null && !string.IsNullOrEmpty(value.name))
			{
				if (!displays.ContainsKey(slotName))
				{
					displays[slotName] = new List<DisplayData>();
				}
				if (value != null)
				{
					value.parent = this;
				}
				List<DisplayData> list = displays[slotName];
				list.Add(value);
			}
		}

		public DisplayData GetDisplay(string slotName, string displayName)
		{
			List<DisplayData> list = GetDisplays(slotName);
			if (list != null)
			{
				foreach (DisplayData item in list)
				{
					if (item != null && item.name == displayName)
					{
						return item;
					}
				}
			}
			return null;
		}

		public List<DisplayData> GetDisplays(string slotName)
		{
			if (string.IsNullOrEmpty(slotName) || !displays.ContainsKey(slotName))
			{
				return null;
			}
			return displays[slotName];
		}
	}
}
