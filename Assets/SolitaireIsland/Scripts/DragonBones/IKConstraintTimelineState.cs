namespace DragonBones
{
	internal class IKConstraintTimelineState : ConstraintTimelineState
	{
		private float _current;

		private float _delta;

		protected override void _OnClear()
		{
			base._OnClear();
			_current = 0f;
			_delta = 0f;
		}

		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			IKConstraint iKConstraint = constraint as IKConstraint;
			if (_timelineData != null)
			{
				long num = _animationData.frameIntOffset + _frameValueOffset + _frameIndex * 2;
				short[] frameIntArray = _frameIntArray;
				short[] array = frameIntArray;
				long num2 = num;
				num = num2 + 1;
				bool bendPositive = array[num2] != 0;
				short[] array2 = frameIntArray;
				long num3 = num;
				num = num3 + 1;
				_current = (float)array2[num3] * 0.01f;
				if (_tweenState == TweenState.Always)
				{
					if (_frameIndex == _frameCount - 1)
					{
						num = _animationData.frameIntOffset + _frameValueOffset;
					}
					_delta = (float)frameIntArray[num + 1] * 0.01f - _current;
				}
				else
				{
					_delta = 0f;
				}
				iKConstraint._bendPositive = bendPositive;
			}
			else
			{
				IKConstraintData iKConstraintData = iKConstraint._constraintData as IKConstraintData;
				_current = iKConstraintData.weight;
				_delta = 0f;
				iKConstraint._bendPositive = iKConstraintData.bendPositive;
			}
			iKConstraint.InvalidUpdate();
		}

		protected override void _OnUpdateFrame()
		{
			base._OnUpdateFrame();
			if (_tweenState != TweenState.Always)
			{
				_tweenState = TweenState.None;
			}
			IKConstraint iKConstraint = constraint as IKConstraint;
			iKConstraint._weight = _current + _delta * _tweenProgress;
			iKConstraint.InvalidUpdate();
		}
	}
}
