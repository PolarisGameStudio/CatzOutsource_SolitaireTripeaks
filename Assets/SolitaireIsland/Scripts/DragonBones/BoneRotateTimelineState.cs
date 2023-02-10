namespace DragonBones
{
	internal class BoneRotateTimelineState : BoneTimelineState
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
				transform.rotation = array[num2];
				Transform transform2 = current;
				float[] array2 = frameFloatArray;
				long num3 = num;
				num = num3 + 1;
				transform2.skew = array2[num3];
				if (_tweenState == TweenState.Always)
				{
					if (_frameIndex == _frameCount - 1)
					{
						num = _animationData.frameFloatOffset + _frameValueOffset;
						Transform transform3 = delta;
						float[] array3 = frameFloatArray;
						long num4 = num;
						num = num4 + 1;
						transform3.rotation = Transform.NormalizeRadian(array3[num4] - current.rotation);
					}
					else
					{
						Transform transform4 = delta;
						float[] array4 = frameFloatArray;
						long num5 = num;
						num = num5 + 1;
						transform4.rotation = array4[num5] - current.rotation;
					}
					Transform transform5 = delta;
					float[] array5 = frameFloatArray;
					long num6 = num;
					num = num6 + 1;
					transform5.skew = array5[num6] - current.skew;
				}
				else
				{
					delta.rotation = 0f;
					delta.skew = 0f;
				}
			}
			else
			{
				Transform current2 = bonePose.current;
				Transform delta2 = bonePose.delta;
				current2.rotation = 0f;
				current2.skew = 0f;
				delta2.rotation = 0f;
				delta2.skew = 0f;
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
			result.rotation = current.rotation + delta.rotation * _tweenProgress;
			result.skew = current.skew + delta.skew * _tweenProgress;
		}

		public override void FadeOut()
		{
			Transform result = bonePose.result;
			result.rotation = Transform.NormalizeRadian(result.rotation);
			result.skew = Transform.NormalizeRadian(result.skew);
		}
	}
}
