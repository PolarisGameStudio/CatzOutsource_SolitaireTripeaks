using System;

namespace DragonBones
{
	internal abstract class TweenTimelineState : TimelineState
	{
		protected TweenType _tweenType;

		protected int _curveCount;

		protected float _framePosition;

		protected float _frameDurationR;

		protected float _tweenProgress;

		protected float _tweenEasing;

		private static float _GetEasingValue(TweenType tweenType, float progress, float easing)
		{
			float num = progress;
			switch (tweenType)
			{
			case TweenType.QuadIn:
				num = (float)Math.Pow(progress, 2.0);
				break;
			case TweenType.QuadOut:
				num = 1f - (float)Math.Pow(1f - progress, 2.0);
				break;
			case TweenType.QuadInOut:
				num = 0.5f * (1f - (float)Math.Cos((double)progress * 3.1415926535897931));
				break;
			}
			return (num - progress) * easing + progress;
		}

		private static float _GetEasingCurveValue(float progress, short[] samples, int count, int offset)
		{
			if (progress <= 0f)
			{
				return 0f;
			}
			if (progress >= 1f)
			{
				return 1f;
			}
			int num = count + 1;
			int num2 = (int)Math.Floor(progress * (float)num);
			float num3 = (num2 != 0) ? ((float)samples[offset + num2 - 1]) : 0f;
			float num4 = (num2 != num - 1) ? ((float)samples[offset + num2]) : 10000f;
			return (num3 + (num4 - num3) * (progress * (float)num - (float)num2)) * 0.0001f;
		}

		protected override void _OnClear()
		{
			base._OnClear();
			_tweenType = TweenType.None;
			_curveCount = 0;
			_framePosition = 0f;
			_frameDurationR = 0f;
			_tweenProgress = 0f;
			_tweenEasing = 0f;
		}

		protected override void _OnArriveAtFrame()
		{
			if (_frameCount > 1 && (_frameIndex != _frameCount - 1 || _animationState.playTimes == 0 || _animationState.currentPlayTimes < _animationState.playTimes - 1))
			{
				_tweenType = (TweenType)_frameArray[_frameOffset + 1];
				_tweenState = ((_tweenType == TweenType.None) ? TweenState.Once : TweenState.Always);
				if (_tweenType == TweenType.Curve)
				{
					_curveCount = _frameArray[_frameOffset + 2];
				}
				else if (_tweenType != 0 && _tweenType != TweenType.Line)
				{
					_tweenEasing = (float)_frameArray[_frameOffset + 2] * 0.01f;
				}
				_framePosition = (float)_frameArray[_frameOffset] * _frameRateR;
				if (_frameIndex == _frameCount - 1)
				{
					_frameDurationR = 1f / (_animationData.duration - _framePosition);
					return;
				}
				uint num = _animationData.frameOffset + _timelineArray[_timelineData.offset + 5 + _frameIndex + 1];
				float num2 = (float)_frameArray[num] * _frameRateR - _framePosition;
				if (num2 > 0f)
				{
					_frameDurationR = 1f / num2;
				}
				else
				{
					_frameDurationR = 0f;
				}
			}
			else
			{
				_tweenState = TweenState.Once;
			}
		}

		protected override void _OnUpdateFrame()
		{
			if (_tweenState == TweenState.Always)
			{
				_tweenProgress = (currentTime - _framePosition) * _frameDurationR;
				if (_tweenType == TweenType.Curve)
				{
					_tweenProgress = _GetEasingCurveValue(_tweenProgress, _frameArray, _curveCount, (int)(_frameOffset + 3));
				}
				else if (_tweenType != TweenType.Line)
				{
					_tweenProgress = _GetEasingValue(_tweenType, _tweenProgress, _tweenEasing);
				}
			}
			else
			{
				_tweenProgress = 0f;
			}
		}
	}
}
