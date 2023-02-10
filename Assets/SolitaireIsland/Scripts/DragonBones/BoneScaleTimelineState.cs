namespace DragonBones
{
	internal class BoneScaleTimelineState : BoneTimelineState
	{
		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			if (_timelineData != null)
			{
				long num = _animationData.frameFloatOffset + _frameValueOffset + _frameIndex * 2;
				float[] frameFloatArray = _dragonBonesData.frameFloatArray;
				Transform current = bonePose.current;
				Transform delta = bonePose.delta;
				Transform transform = current;
				float[] array = frameFloatArray;
				long num2 = num;
				num = num2 + 1;
				transform.scaleX = array[num2];
				Transform transform2 = current;
				float[] array2 = frameFloatArray;
				long num3 = num;
				num = num3 + 1;
				transform2.scaleY = array2[num3];
				if (_tweenState == TweenState.Always)
				{
					if (_frameIndex == _frameCount - 1)
					{
						num = _animationData.frameFloatOffset + _frameValueOffset;
					}
					Transform transform3 = delta;
					float[] array3 = frameFloatArray;
					long num4 = num;
					num = num4 + 1;
					transform3.scaleX = array3[num4] - current.scaleX;
					Transform transform4 = delta;
					float[] array4 = frameFloatArray;
					long num5 = num;
					num = num5 + 1;
					transform4.scaleY = array4[num5] - current.scaleY;
				}
				else
				{
					delta.scaleX = 0f;
					delta.scaleY = 0f;
				}
			}
			else
			{
				Transform current2 = bonePose.current;
				Transform delta2 = bonePose.delta;
				current2.scaleX = 1f;
				current2.scaleY = 1f;
				delta2.scaleX = 0f;
				delta2.scaleY = 0f;
			}
		}

		protected override void _OnUpdateFrame()
		{
			base._OnUpdateFrame();
			Transform current = bonePose.current;
			Transform delta = bonePose.delta;
			Transform result = bonePose.result;
			bone._transformDirty = true;
			if (_tweenState != TweenState.Always)
			{
				_tweenState = TweenState.None;
			}
			result.scaleX = current.scaleX + delta.scaleX * _tweenProgress;
			result.scaleY = current.scaleY + delta.scaleY * _tweenProgress;
		}
	}
}
