namespace DragonBones
{
	internal class SlotDislayTimelineState : SlotTimelineState
	{
		protected override void _OnArriveAtFrame()
		{
			if (playState >= 0)
			{
				int num = (_timelineData == null) ? slot.slotData.displayIndex : _frameArray[_frameOffset + 1];
				if (slot.displayIndex != num)
				{
					slot._SetDisplayIndex(num, isAnimation: true);
				}
			}
		}
	}
}
