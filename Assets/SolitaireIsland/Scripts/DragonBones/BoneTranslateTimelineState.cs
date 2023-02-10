namespace DragonBones
{
	internal class BoneTranslateTimelineState : BoneTimelineState
	{
		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			if (_timelineData != null)
			{
				long num = _animationData.frameFloatOffset + _frameValueOffset + _frameIndex * 2;
				float scale = _armature._armatureData.scale;
				float[] frameFloatArray = _dragonBonesData.frameFloatArray;
				Transform current = bonePose.current;
				Transform delta = bonePose.delta;
				Transform transform = current;
				float[] array = frameFloatArray;
				long num2 = num;
				num = num2 + 1;
				transform.x = array[num2] * scale;
				Transform transform2 = current;
				float[] array2 = frameFloatArray;
				long num3 = num;
				num = num3 + 1;
				transform2.y = array2[num3] * scale;
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
					transform3.x = array3[num4] * scale - current.x;
					Transform transform4 = delta;
					float[] array4 = frameFloatArray;
					long num5 = num;
					num = num5 + 1;
					transform4.y = array4[num5] * scale - current.y;
				}
				else
				{
					delta.x = 0f;
					delta.y = 0f;
				}
			}
			else
			{
				Transform current2 = bonePose.current;
				Transform delta2 = bonePose.delta;
				current2.x = 0f;
				current2.y = 0f;
				delta2.x = 0f;
				delta2.y = 0f;
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
			result.x = current.x + delta.x * _tweenProgress;
			result.y = current.y + delta.y * _tweenProgress;
		}
	}
}
