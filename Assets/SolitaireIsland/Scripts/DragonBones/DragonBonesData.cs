using System.Collections.Generic;

namespace DragonBones
{
	public class DragonBonesData : BaseObject
	{
		public bool autoSearch;

		public uint frameRate;

		public string version;

		public string name;

		public ArmatureData stage;

		public readonly List<uint> frameIndices = new List<uint>();

		public readonly List<float> cachedFrames = new List<float>();

		public readonly List<string> armatureNames = new List<string>();

		public readonly Dictionary<string, ArmatureData> armatures = new Dictionary<string, ArmatureData>();

		internal byte[] binary;

		internal short[] intArray;

		internal float[] floatArray;

		internal short[] frameIntArray;

		internal float[] frameFloatArray;

		internal short[] frameArray;

		internal ushort[] timelineArray;

		internal UserData userData;

		protected override void _OnClear()
		{
			foreach (string key in armatures.Keys)
			{
				armatures[key].ReturnToPool();
			}
			if (userData != null)
			{
				userData.ReturnToPool();
			}
			autoSearch = false;
			frameRate = 0u;
			version = string.Empty;
			name = string.Empty;
			stage = null;
			frameIndices.Clear();
			cachedFrames.Clear();
			armatureNames.Clear();
			armatures.Clear();
			binary = null;
			intArray = null;
			floatArray = null;
			frameIntArray = null;
			frameFloatArray = null;
			frameArray = null;
			timelineArray = null;
			userData = null;
		}

		public void AddArmature(ArmatureData value)
		{
			if (armatures.ContainsKey(value.name))
			{
				Helper.Assert(condition: false, "Same armature: " + value.name);
				armatures[value.name].ReturnToPool();
			}
			value.parent = this;
			armatures[value.name] = value;
			armatureNames.Add(value.name);
		}

		public ArmatureData GetArmature(string armatureName)
		{
			return (!armatures.ContainsKey(armatureName)) ? null : armatures[armatureName];
		}
	}
}
