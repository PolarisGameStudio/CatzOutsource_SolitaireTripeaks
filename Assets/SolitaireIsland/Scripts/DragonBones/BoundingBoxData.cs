namespace DragonBones
{
	public abstract class BoundingBoxData : BaseObject
	{
		public BoundingBoxType type;

		public uint color;

		public float width;

		public float height;

		protected override void _OnClear()
		{
			color = 0u;
			width = 0f;
			height = 0f;
		}

		public abstract bool ContainsPoint(float pX, float pY);

		public abstract int IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null);
	}
}
