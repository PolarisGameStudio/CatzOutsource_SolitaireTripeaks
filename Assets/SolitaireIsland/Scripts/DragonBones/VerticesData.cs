namespace DragonBones
{
	public class VerticesData
	{
		public bool isShared;

		public bool inheritDeform;

		public int offset;

		public DragonBonesData data;

		public WeightData weight;

		public void Clear()
		{
			if (!isShared && weight != null)
			{
				weight.ReturnToPool();
			}
			isShared = false;
			inheritDeform = false;
			offset = 0;
			data = null;
			weight = null;
		}

		public void ShareFrom(VerticesData value)
		{
			isShared = true;
			offset = value.offset;
			weight = value.weight;
		}
	}
}
