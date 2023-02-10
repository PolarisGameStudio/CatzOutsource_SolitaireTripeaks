namespace DragonBones
{
	public abstract class DisplayData : BaseObject
	{
		public DisplayType type;

		public string name;

		public string path;

		public SkinData parent;

		public readonly Transform transform = new Transform();

		protected override void _OnClear()
		{
			name = string.Empty;
			path = string.Empty;
			transform.Identity();
			parent = null;
		}
	}
}
