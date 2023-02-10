namespace DragonBones
{
	internal abstract class ConstraintTimelineState : TweenTimelineState
	{
		public Constraint constraint;

		protected override void _OnClear()
		{
			base._OnClear();
			constraint = null;
		}
	}
}
