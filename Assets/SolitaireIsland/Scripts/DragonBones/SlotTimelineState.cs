namespace DragonBones
{
	internal abstract class SlotTimelineState : TweenTimelineState
	{
		public Slot slot;

		protected override void _OnClear()
		{
			base._OnClear();
			slot = null;
		}
	}
}
