using System.Collections.Generic;

namespace DragonBones
{
	public class WeightData : BaseObject
	{
		public int count;

		public int offset;

		public readonly List<BoneData> bones = new List<BoneData>();

		protected override void _OnClear()
		{
			count = 0;
			offset = 0;
			bones.Clear();
		}

		internal void AddBone(BoneData value)
		{
			bones.Add(value);
		}
	}
}
