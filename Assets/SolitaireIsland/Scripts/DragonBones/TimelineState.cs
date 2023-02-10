using System;
using System.Collections.Generic;

namespace DragonBones
{
	internal abstract class TimelineState : BaseObject
	{
		public int playState;

		public int currentPlayTimes;

		public float currentTime;

		protected TweenState _tweenState;

		protected uint _frameRate;

		protected int _frameValueOffset;

		protected uint _frameCount;

		protected uint _frameOffset;

		protected int _frameIndex;

		protected float _frameRateR;

		protected float _position;

		protected float _duration;

		protected float _timeScale;

		protected float _timeOffset;

		protected DragonBonesData _dragonBonesData;

		protected AnimationData _animationData;

		protected TimelineData _timelineData;

		protected Armature _armature;

		protected AnimationState _animationState;

		protected TimelineState _actionTimeline;

		protected short[] _frameArray;

		protected short[] _frameIntArray;

		protected float[] _frameFloatArray;

		protected ushort[] _timelineArray;

		protected List<uint> _frameIndices;

		protected override void _OnClear()
		{
			playState = -1;
			currentPlayTimes = -1;
			currentTime = -1f;
			_tweenState = TweenState.None;
			_frameRate = 0u;
			_frameValueOffset = 0;
			_frameCount = 0u;
			_frameOffset = 0u;
			_frameIndex = -1;
			_frameRateR = 0f;
			_position = 0f;
			_duration = 0f;
			_timeScale = 1f;
			_timeOffset = 0f;
			_dragonBonesData = null;
			_animationData = null;
			_timelineData = null;
			_armature = null;
			_animationState = null;
			_actionTimeline = null;
			_frameArray = null;
			_frameIntArray = null;
			_frameFloatArray = null;
			_timelineArray = null;
			_frameIndices = null;
		}

		protected abstract void _OnArriveAtFrame();

		protected abstract void _OnUpdateFrame();

		protected bool _SetCurrentTime(float passedTime)
		{
			int num = playState;
			int num2 = currentPlayTimes;
			float num3 = currentTime;
			if (_actionTimeline != null && _frameCount <= 1)
			{
				playState = ((_actionTimeline.playState >= 0) ? 1 : (-1));
				currentPlayTimes = 1;
				currentTime = _actionTimeline.currentTime;
			}
			else if (_actionTimeline == null || _timeScale != 1f || _timeOffset != 0f)
			{
				int playTimes = _animationState.playTimes;
				float num4 = (float)playTimes * _duration;
				passedTime *= _timeScale;
				if (_timeOffset != 0f)
				{
					passedTime += _timeOffset * _animationData.duration;
				}
				if (playTimes > 0 && (passedTime >= num4 || passedTime <= 0f - num4))
				{
					if (playState <= 0 && _animationState._playheadState == 3)
					{
						playState = 1;
					}
					currentPlayTimes = playTimes;
					if (passedTime < 0f)
					{
						currentTime = 0f;
					}
					else
					{
						currentTime = _duration + 1E-06f;
					}
				}
				else
				{
					if (playState != 0 && _animationState._playheadState == 3)
					{
						playState = 0;
					}
					if (passedTime < 0f)
					{
						passedTime = 0f - passedTime;
						currentPlayTimes = (int)(passedTime / _duration);
						currentTime = _duration - passedTime % _duration;
					}
					else
					{
						currentPlayTimes = (int)(passedTime / _duration);
						currentTime = passedTime % _duration;
					}
				}
				currentTime += _position;
			}
			else
			{
				playState = _actionTimeline.playState;
				currentPlayTimes = _actionTimeline.currentPlayTimes;
				currentTime = _actionTimeline.currentTime;
			}
			if (currentPlayTimes == num2 && currentTime == num3)
			{
				return false;
			}
			if ((num < 0 && playState != num) || (playState <= 0 && currentPlayTimes != num2))
			{
				_frameIndex = -1;
			}
			return true;
		}

		public virtual void Init(Armature armature, AnimationState animationState, TimelineData timelineData)
		{
			_armature = armature;
			_animationState = animationState;
			_timelineData = timelineData;
			_actionTimeline = _animationState._actionTimeline;
			if (this == _actionTimeline)
			{
				_actionTimeline = null;
			}
			_frameRate = _armature.armatureData.frameRate;
			_frameRateR = 1f / (float)(double)_frameRate;
			_position = _animationState._position;
			_duration = _animationState._duration;
			_dragonBonesData = _armature.armatureData.parent;
			_animationData = _animationState._animationData;
			if (_timelineData != null)
			{
				_frameIntArray = _dragonBonesData.frameIntArray;
				_frameFloatArray = _dragonBonesData.frameFloatArray;
				_frameArray = _dragonBonesData.frameArray;
				_timelineArray = _dragonBonesData.timelineArray;
				_frameIndices = _dragonBonesData.frameIndices;
				_frameCount = _timelineArray[_timelineData.offset + 2];
				_frameValueOffset = _timelineArray[_timelineData.offset + 4];
				ushort num = _timelineArray[_timelineData.offset];
				_timeScale = 100f / ((num != 0) ? ((float)(int)num) : 100f);
				_timeOffset = (float)(int)_timelineArray[_timelineData.offset + 1] * 0.01f;
			}
		}

		public virtual void FadeOut()
		{
		}

		public virtual void Update(float passedTime)
		{
			if (!_SetCurrentTime(passedTime))
			{
				return;
			}
			if (_frameCount > 1)
			{
				int num = (int)Math.Floor(currentTime * (float)(double)_frameRate);
				uint num2 = _frameIndices[_timelineData.frameIndicesOffset + num];
				if (_frameIndex != num2)
				{
					_frameIndex = (int)num2;
					_frameOffset = _animationData.frameOffset + _timelineArray[_timelineData.offset + 5 + _frameIndex];
					_OnArriveAtFrame();
				}
			}
			else if (_frameIndex < 0)
			{
				_frameIndex = 0;
				if (_timelineData != null)
				{
					_frameOffset = _animationData.frameOffset + _timelineArray[_timelineData.offset + 5];
				}
				_OnArriveAtFrame();
			}
			if (_tweenState != 0)
			{
				_OnUpdateFrame();
			}
		}
	}
}
