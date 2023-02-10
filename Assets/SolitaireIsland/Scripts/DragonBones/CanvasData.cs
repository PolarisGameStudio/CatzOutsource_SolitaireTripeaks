namespace DragonBones
{
	public class CanvasData : BaseObject
	{
		public bool hasBackground;

		public int color;

		public float x;

		public float y;

		public float width;

		public float height;

		protected override void _OnClear()
		{
			hasBackground = false;
			color = 0;
			x = 0f;
			y = 0f;
			width = 0f;
			height = 0f;
		}
	}
}
