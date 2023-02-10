namespace DragonBones
{
	public class Rectangle
	{
		public float x;

		public float y;

		public float width;

		public float height;

		public void CopyFrom(Rectangle value)
		{
			x = value.x;
			y = value.y;
			width = value.width;
			height = value.height;
		}

		public void Clear()
		{
			x = (y = 0f);
			width = (height = 0f);
		}
	}
}
