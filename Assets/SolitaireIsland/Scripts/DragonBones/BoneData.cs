namespace DragonBones
{
	public class BoneData : BaseObject
	{
		public bool inheritTranslation;

		public bool inheritRotation;

		public bool inheritScale;

		public bool inheritReflection;

		public float length;

		public string name;

		public readonly Transform transform = new Transform();

		public UserData userData;

		public BoneData parent;

		protected override void _OnClear()
		{
			if (userData != null)
			{
				userData.ReturnToPool();
			}
			inheritTranslation = false;
			inheritRotation = false;
			inheritScale = false;
			inheritReflection = false;
			length = 0f;
			name = string.Empty;
			transform.Identity();
			userData = null;
			parent = null;
		}
	}
}
