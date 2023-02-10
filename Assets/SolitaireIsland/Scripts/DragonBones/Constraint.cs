namespace DragonBones
{
	internal abstract class Constraint : BaseObject
	{
		protected static readonly Matrix _helpMatrix = new Matrix();

		protected static readonly Transform _helpTransform = new Transform();

		protected static readonly Point _helpPoint = new Point();

		internal ConstraintData _constraintData;

		protected Armature _armature;

		internal Bone _target;

		internal Bone _root;

		internal Bone _bone;

		public string name => _constraintData.name;

		protected override void _OnClear()
		{
			_armature = null;
			_target = null;
			_root = null;
			_bone = null;
		}

		public abstract void Init(ConstraintData constraintData, Armature armature);

		public abstract void Update();

		public abstract void InvalidUpdate();
	}
}
