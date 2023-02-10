namespace DragonBones
{
	internal abstract class BoneTimelineState : TweenTimelineState
	{
		public Bone bone;

		public BonePose bonePose;

		protected override void _OnClear()
		{
			base._OnClear();
			bone = null;
			bonePose = null;
		}

		public void Blend(int state)
		{
			float blendWeight = bone._blendState.blendWeight;
			Transform animationPose = bone.animationPose;
			Transform result = bonePose.result;
			if (state == 2)
			{
				animationPose.x += result.x * blendWeight;
				animationPose.y += result.y * blendWeight;
				animationPose.rotation += result.rotation * blendWeight;
				animationPose.skew += result.skew * blendWeight;
				animationPose.scaleX += (result.scaleX - 1f) * blendWeight;
				animationPose.scaleY += (result.scaleY - 1f) * blendWeight;
			}
			else if (blendWeight != 1f)
			{
				animationPose.x = result.x * blendWeight;
				animationPose.y = result.y * blendWeight;
				animationPose.rotation = result.rotation * blendWeight;
				animationPose.skew = result.skew * blendWeight;
				animationPose.scaleX = (result.scaleX - 1f) * blendWeight + 1f;
				animationPose.scaleY = (result.scaleY - 1f) * blendWeight + 1f;
			}
			else
			{
				animationPose.x = result.x;
				animationPose.y = result.y;
				animationPose.rotation = result.rotation;
				animationPose.skew = result.skew;
				animationPose.scaleX = result.scaleX;
				animationPose.scaleY = result.scaleY;
			}
			if (_animationState._fadeState != 0 || _animationState._subFadeState != 0)
			{
				bone._transformDirty = true;
			}
		}
	}
}
