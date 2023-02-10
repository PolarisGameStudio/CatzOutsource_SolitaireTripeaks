namespace DragonBones
{
	internal class BoneAllTimelineState : BoneTimelineState
	{
		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			if (_timelineData != null)
			{
				int num = (int)_animationData.frameFloatOffset + _frameValueOffset + _frameIndex * 6;
				float scale = _armature._armatureData.scale;
				float[] frameFloatArray = _dragonBonesData.frameFloatArray;
				Transform current = bonePose.current;
				Transform delta = bonePose.delta;
				current.x = frameFloatArray[num++] * scale;
				current.y = frameFloatArray[num++] * scale;
				current.rotation = frameFloatArray[num++];
				current.skew = frameFloatArray[num++];
				current.scaleX = frameFloatArray[num++];
				current.scaleY = frameFloatArray[num++];
				if (_tweenState == TweenState.Always)
				{
					if (_frameIndex == _frameCount - 1)
					{
						num = (int)_animationData.frameFloatOffset + _frameValueOffset;
					}
					delta.x = frameFloatArray[num++] * scale - current.x;
					delta.y = frameFloatArray[num++] * scale - current.y;
					delta.rotation = frameFloatArray[num++] - current.rotation;
					delta.skew = frameFloatArray[num++] - current.skew;
					delta.scaleX = frameFloatArray[num++] - current.scaleX;
					delta.scaleY = frameFloatArray[num++] - current.scaleY;
				}
				else
				{
					delta.x = 0f;
					delta.y = 0f;
					delta.rotation = 0f;
					delta.skew = 0f;
					delta.scaleX = 0f;
					delta.scaleY = 0f;
				}
			}
			else
			{
				Transform current2 = bonePose.current;
				Transform delta2 = bonePose.delta;
				current2.x = 0f;
				current2.y = 0f;
				current2.rotation = 0f;
				current2.skew = 0f;
				current2.scaleX = 1f;
				current2.scaleY = 1f;
				delta2.x = 0f;
				delta2.y = 0f;
				delta2.rotation = 0f;
				delta2.skew = 0f;
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
			result.x = current.x + delta.x * _tweenProgress;
			result.y = current.y + delta.y * _tweenProgress;
			result.rotation = current.rotation + delta.rotation * _tweenProgress;
			result.skew = current.skew + delta.skew * _tweenProgress;
			result.scaleX = current.scaleX + delta.scaleX * _tweenProgress;
			result.scaleY = current.scaleY + delta.scaleY * _tweenProgress;
		}

		public override void FadeOut()
		{
			Transform result = bonePose.result;
			result.rotation = Transform.NormalizeRadian(result.rotation);
			result.skew = Transform.NormalizeRadian(result.skew);
		}
	}
}
