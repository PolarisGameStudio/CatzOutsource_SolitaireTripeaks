namespace DragonBones
{
	internal class ZOrderTimelineState : TimelineState
	{
		protected override void _OnArriveAtFrame()
		{
			if (playState >= 0)
			{
				short num = _frameArray[_frameOffset + 1];
				if (num > 0)
				{
					_armature._SortZOrder(_frameArray, (int)(_frameOffset + 2));
				}
				else
				{
					_armature._SortZOrder(null, 0);
				}
			}
		}

		protected override void _OnUpdateFrame()
		{
		}
	}
}
