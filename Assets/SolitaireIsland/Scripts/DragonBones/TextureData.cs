namespace DragonBones
{
	public abstract class TextureData : BaseObject
	{
		public bool rotated;

		public string name;

		public readonly Rectangle region = new Rectangle();

		public TextureAtlasData parent;

		public Rectangle frame;

		public static Rectangle CreateRectangle()
		{
			return new Rectangle();
		}

		protected override void _OnClear()
		{
			rotated = false;
			name = string.Empty;
			region.Clear();
			parent = null;
			frame = null;
		}

		public virtual void CopyFrom(TextureData value)
		{
			rotated = value.rotated;
			name = value.name;
			region.CopyFrom(value.region);
			parent = value.parent;
			if (frame == null && value.frame != null)
			{
				frame = CreateRectangle();
			}
			else if (frame != null && value.frame == null)
			{
				frame = null;
			}
			if (frame != null && value.frame != null)
			{
				frame.CopyFrom(value.frame);
			}
		}
	}
}
