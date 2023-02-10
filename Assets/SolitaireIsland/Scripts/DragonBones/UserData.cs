using System.Collections.Generic;

namespace DragonBones
{
	public class UserData : BaseObject
	{
		public readonly List<int> ints = new List<int>();

		public readonly List<float> floats = new List<float>();

		public readonly List<string> strings = new List<string>();

		protected override void _OnClear()
		{
			ints.Clear();
			floats.Clear();
			strings.Clear();
		}

		internal void AddInt(int value)
		{
			ints.Add(value);
		}

		internal void AddFloat(float value)
		{
			floats.Add(value);
		}

		internal void AddString(string value)
		{
			strings.Add(value);
		}

		public int GetInt(int index = 0)
		{
			return (index >= 0 && index < ints.Count) ? ints[index] : 0;
		}

		public float GetFloat(int index = 0)
		{
			return (index < 0 || index >= floats.Count) ? 0f : floats[index];
		}

		public string GetString(int index = 0)
		{
			return (index < 0 || index >= strings.Count) ? string.Empty : strings[index];
		}
	}
}
