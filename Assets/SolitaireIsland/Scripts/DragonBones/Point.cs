namespace DragonBones
{
	public class Point
	{
		public float x;

		public float y;

		public void CopyFrom(Point value)
		{
			x = value.x;
			y = value.y;
		}

		public void Clear()
		{
			x = (y = 0f);
		}
	}
}
