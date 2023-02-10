namespace DragonBones
{
	public abstract class TransformObject : BaseObject
	{
		protected static readonly Matrix _helpMatrix = new Matrix();

		protected static readonly Transform _helpTransform = new Transform();

		protected static readonly Point _helpPoint = new Point();

		public readonly Matrix globalTransformMatrix = new Matrix();

		public readonly Transform global = new Transform();

		public readonly Transform offset = new Transform();

		public Transform origin;

		public object userData;

		protected bool _globalDirty;

		internal Armature _armature;

		public Armature armature => _armature;

		protected override void _OnClear()
		{
			globalTransformMatrix.Identity();
			global.Identity();
			offset.Identity();
			origin = null;
			userData = null;
			_globalDirty = false;
			_armature = null;
		}

		public void UpdateGlobalTransform()
		{
			if (_globalDirty)
			{
				_globalDirty = false;
				global.FromMatrix(globalTransformMatrix);
			}
		}
	}
}
