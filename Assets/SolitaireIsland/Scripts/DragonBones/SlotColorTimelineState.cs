using System;

namespace DragonBones
{
	internal class SlotColorTimelineState : SlotTimelineState
	{
		private bool _dirty;

		private readonly int[] _current = new int[8];

		private readonly int[] _delta = new int[8];

		private readonly float[] _result = new float[8];

		protected override void _OnClear()
		{
			base._OnClear();
			_dirty = false;
		}

		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			if (_timelineData != null)
			{
				short[] intArray = _dragonBonesData.intArray;
				short[] frameIntArray = _dragonBonesData.frameIntArray;
				long num = _animationData.frameIntOffset + _frameValueOffset + _frameIndex;
				int num2 = frameIntArray[num];
				if (num2 < 0)
				{
					num2 += 65536;
				}
				_current[0] = intArray[num2++];
				_current[1] = intArray[num2++];
				_current[2] = intArray[num2++];
				_current[3] = intArray[num2++];
				_current[4] = intArray[num2++];
				_current[5] = intArray[num2++];
				_current[6] = intArray[num2++];
				_current[7] = intArray[num2++];
				if (_tweenState == TweenState.Always)
				{
					num2 = ((_frameIndex != _frameCount - 1) ? frameIntArray[num + 1] : frameIntArray[_animationData.frameIntOffset + _frameValueOffset]);
					if (num2 < 0)
					{
						num2 += 65536;
					}
					_delta[0] = intArray[num2++] - _current[0];
					_delta[1] = intArray[num2++] - _current[1];
					_delta[2] = intArray[num2++] - _current[2];
					_delta[3] = intArray[num2++] - _current[3];
					_delta[4] = intArray[num2++] - _current[4];
					_delta[5] = intArray[num2++] - _current[5];
					_delta[6] = intArray[num2++] - _current[6];
					_delta[7] = intArray[num2++] - _current[7];
				}
			}
			else
			{
				ColorTransform color = slot._slotData.color;
				_current[0] = (int)(color.alphaMultiplier * 100f);
				_current[1] = (int)(color.redMultiplier * 100f);
				_current[2] = (int)(color.greenMultiplier * 100f);
				_current[3] = (int)(color.blueMultiplier * 100f);
				_current[4] = color.alphaOffset;
				_current[5] = color.redOffset;
				_current[6] = color.greenOffset;
				_current[7] = color.blueOffset;
			}
		}

		protected override void _OnUpdateFrame()
		{
			base._OnUpdateFrame();
			_dirty = true;
			if (_tweenState != TweenState.Always)
			{
				_tweenState = TweenState.None;
			}
			_result[0] = ((float)_current[0] + (float)_delta[0] * _tweenProgress) * 0.01f;
			_result[1] = ((float)_current[1] + (float)_delta[1] * _tweenProgress) * 0.01f;
			_result[2] = ((float)_current[2] + (float)_delta[2] * _tweenProgress) * 0.01f;
			_result[3] = ((float)_current[3] + (float)_delta[3] * _tweenProgress) * 0.01f;
			_result[4] = (float)_current[4] + (float)_delta[4] * _tweenProgress;
			_result[5] = (float)_current[5] + (float)_delta[5] * _tweenProgress;
			_result[6] = (float)_current[6] + (float)_delta[6] * _tweenProgress;
			_result[7] = (float)_current[7] + (float)_delta[7] * _tweenProgress;
		}

		public override void FadeOut()
		{
			_tweenState = TweenState.None;
			_dirty = false;
		}

		public override void Update(float passedTime)
		{
			base.Update(passedTime);
			if (_tweenState == TweenState.None && !_dirty)
			{
				return;
			}
			ColorTransform colorTransform = slot._colorTransform;
			if (_animationState._fadeState != 0 || _animationState._subFadeState != 0)
			{
				if (colorTransform.alphaMultiplier != _result[0] || colorTransform.redMultiplier != _result[1] || colorTransform.greenMultiplier != _result[2] || colorTransform.blueMultiplier != _result[3] || (float)colorTransform.alphaOffset != _result[4] || (float)colorTransform.redOffset != _result[5] || (float)colorTransform.greenOffset != _result[6] || (float)colorTransform.blueOffset != _result[7])
				{
					float num = (float)Math.Pow(_animationState._fadeProgress, 4.0);
					colorTransform.alphaMultiplier += (_result[0] - colorTransform.alphaMultiplier) * num;
					colorTransform.redMultiplier += (_result[1] - colorTransform.redMultiplier) * num;
					colorTransform.greenMultiplier += (_result[2] - colorTransform.greenMultiplier) * num;
					colorTransform.blueMultiplier += (_result[3] - colorTransform.blueMultiplier) * num;
					colorTransform.alphaOffset += (int)((_result[4] - (float)colorTransform.alphaOffset) * num);
					colorTransform.redOffset += (int)((_result[5] - (float)colorTransform.redOffset) * num);
					colorTransform.greenOffset += (int)((_result[6] - (float)colorTransform.greenOffset) * num);
					colorTransform.blueOffset += (int)((_result[7] - (float)colorTransform.blueOffset) * num);
					slot._colorDirty = true;
				}
			}
			else if (_dirty)
			{
				_dirty = false;
				if (colorTransform.alphaMultiplier != _result[0] || colorTransform.redMultiplier != _result[1] || colorTransform.greenMultiplier != _result[2] || colorTransform.blueMultiplier != _result[3] || colorTransform.alphaOffset != (int)_result[4] || colorTransform.redOffset != (int)_result[5] || colorTransform.greenOffset != (int)_result[6] || colorTransform.blueOffset != (int)_result[7])
				{
					colorTransform.alphaMultiplier = _result[0];
					colorTransform.redMultiplier = _result[1];
					colorTransform.greenMultiplier = _result[2];
					colorTransform.blueMultiplier = _result[3];
					colorTransform.alphaOffset = (int)_result[4];
					colorTransform.redOffset = (int)_result[5];
					colorTransform.greenOffset = (int)_result[6];
					colorTransform.blueOffset = (int)_result[7];
					slot._colorDirty = true;
				}
			}
		}
	}
}
