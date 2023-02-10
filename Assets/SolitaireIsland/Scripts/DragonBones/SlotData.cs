namespace DragonBones
{
	public class SlotData : BaseObject
	{
		public static readonly ColorTransform DEFAULT_COLOR = new ColorTransform();

		public BlendMode blendMode;

		public int displayIndex;

		public int zOrder;

		public string name;

		public ColorTransform color;

		public UserData userData;

		public BoneData parent;

		public static ColorTransform CreateColor()
		{
			return new ColorTransform();
		}

		protected override void _OnClear()
		{
			if (userData != null)
			{
				userData.ReturnToPool();
			}
			blendMode = BlendMode.Normal;
			displayIndex = 0;
			zOrder = 0;
			name = string.Empty;
			color = null;
			userData = null;
			parent = null;
		}
	}
}
