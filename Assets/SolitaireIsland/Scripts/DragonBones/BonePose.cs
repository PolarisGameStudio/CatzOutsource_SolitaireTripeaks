namespace DragonBones
{
	internal class BonePose : BaseObject
	{
		public readonly Transform current = new Transform();

		public readonly Transform delta = new Transform();

		public readonly Transform result = new Transform();

		protected override void _OnClear()
		{
			current.Identity();
			delta.Identity();
			result.Identity();
		}
	}
}
