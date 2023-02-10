using System.Collections.Generic;

namespace DragonBones
{
	internal class ActionTimelineState : TimelineState
	{
		private void _OnCrossFrame(int frameIndex)
		{
			IArmatureProxy proxy = _armature.proxy;
			if (!_animationState.actionEnabled)
			{
				return;
			}
			uint num = _animationData.frameOffset + _timelineArray[_timelineData.offset + 5 + frameIndex];
			short num2 = _frameArray[num + 1];
			List<ActionData> actions = _animationData.parent.actions;
			for (int i = 0; i < num2; i++)
			{
				short index = _frameArray[num + 2 + i];
				ActionData actionData = actions[index];
				if (actionData.type == ActionType.Play)
				{
					EventObject eventObject = BaseObject.BorrowObject<EventObject>();
					eventObject.time = (long)_frameArray[num] / (long)_frameRate;
					eventObject.animationState = _animationState;
					EventObject.ActionDataToInstance(actionData, eventObject, _armature);
					_armature._BufferAction(eventObject, append: true);
					continue;
				}
				string type = (actionData.type != ActionType.Frame) ? "soundEvent" : "frameEvent";
				if (actionData.type == ActionType.Sound || proxy.HasDBEventListener(type))
				{
					EventObject eventObject2 = BaseObject.BorrowObject<EventObject>();
					eventObject2.time = (float)_frameArray[num] / (float)(double)_frameRate;
					eventObject2.animationState = _animationState;
					EventObject.ActionDataToInstance(actionData, eventObject2, _armature);
					_armature._dragonBones.BufferEvent(eventObject2);
				}
			}
		}

		protected override void _OnArriveAtFrame()
		{
		}

		protected override void _OnUpdateFrame()
		{
		}

		public override void Update(float passedTime)
		{
			int playState = base.playState;
			int currentPlayTimes = base.currentPlayTimes;
			float currentTime = base.currentTime;
			if (!_SetCurrentTime(passedTime))
			{
				return;
			}
			IArmatureProxy proxy = _armature.proxy;
			if (playState < 0)
			{
				if (base.playState == playState)
				{
					return;
				}
				if (_animationState.displayControl && _animationState.resetToPose)
				{
					_armature._SortZOrder(null, 0);
				}
				currentPlayTimes = base.currentPlayTimes;
				if (proxy.HasDBEventListener("start"))
				{
					EventObject eventObject = BaseObject.BorrowObject<EventObject>();
					eventObject.type = "start";
					eventObject.armature = _armature;
					eventObject.animationState = _animationState;
					_armature._dragonBones.BufferEvent(eventObject);
				}
			}
			bool flag = _animationState.timeScale < 0f;
			EventObject eventObject2 = null;
			EventObject eventObject3 = null;
			if (base.currentPlayTimes != currentPlayTimes)
			{
				if (proxy.HasDBEventListener("loopComplete"))
				{
					eventObject2 = BaseObject.BorrowObject<EventObject>();
					eventObject2.type = "loopComplete";
					eventObject2.armature = _armature;
					eventObject2.animationState = _animationState;
				}
				if (base.playState > 0 && proxy.HasDBEventListener("complete"))
				{
					eventObject3 = BaseObject.BorrowObject<EventObject>();
					eventObject3.type = "complete";
					eventObject3.armature = _armature;
					eventObject3.animationState = _animationState;
				}
			}
			if (_frameCount > 1)
			{
				TimelineData timelineData = _timelineData;
				int num = (int)(base.currentTime * (float)(double)_frameRate);
				int num2 = (int)_frameIndices[timelineData.frameIndicesOffset + num];
				if (_frameIndex != num2)
				{
					int num3 = _frameIndex;
					_frameIndex = num2;
					if (_timelineArray != null)
					{
						_frameOffset = _animationData.frameOffset + _timelineArray[timelineData.offset + 5 + _frameIndex];
						if (flag)
						{
							if (num3 < 0)
							{
								int num4 = (int)(currentTime * (float)(double)_frameRate);
								num3 = (int)_frameIndices[timelineData.frameIndicesOffset + num4];
								if (base.currentPlayTimes == currentPlayTimes && num3 == num2)
								{
									num3 = -1;
								}
							}
							while (num3 >= 0)
							{
								uint num5 = _animationData.frameOffset + _timelineArray[timelineData.offset + 5 + num3];
								float num6 = (float)_frameArray[num5] / (float)(double)_frameRate;
								if (_position <= num6 && num6 <= _position + _duration)
								{
									_OnCrossFrame(num3);
								}
								if (eventObject2 != null && num3 == 0)
								{
									_armature._dragonBones.BufferEvent(eventObject2);
									eventObject2 = null;
								}
								num3 = ((num3 <= 0) ? ((int)(_frameCount - 1)) : (num3 - 1));
								if (num3 == num2)
								{
									break;
								}
							}
						}
						else
						{
							if (num3 < 0)
							{
								int num7 = (int)(currentTime * (float)(double)_frameRate);
								num3 = (int)_frameIndices[timelineData.frameIndicesOffset + num7];
								uint num8 = _animationData.frameOffset + _timelineArray[timelineData.offset + 5 + num3];
								float num9 = (float)_frameArray[num8] / (float)(double)_frameRate;
								if (base.currentPlayTimes == currentPlayTimes)
								{
									if (currentTime <= num9)
									{
										num3 = ((num3 <= 0) ? ((int)(_frameCount - 1)) : (num3 - 1));
									}
									else if (num3 == num2)
									{
										num3 = -1;
									}
								}
							}
							while (num3 >= 0)
							{
								num3 = ((num3 < _frameCount - 1) ? (num3 + 1) : 0);
								uint num10 = _animationData.frameOffset + _timelineArray[timelineData.offset + 5 + num3];
								float num11 = (float)_frameArray[num10] / (float)(double)_frameRate;
								if (_position <= num11 && num11 <= _position + _duration)
								{
									_OnCrossFrame(num3);
								}
								if (eventObject2 != null && num3 == 0)
								{
									_armature._dragonBones.BufferEvent(eventObject2);
									eventObject2 = null;
								}
								if (num3 == num2)
								{
									break;
								}
							}
						}
					}
				}
			}
			else if (_frameIndex < 0)
			{
				_frameIndex = 0;
				if (_timelineData != null)
				{
					_frameOffset = _animationData.frameOffset + _timelineArray[_timelineData.offset + 5];
					float num12 = (float)_frameArray[_frameOffset] / (float)(double)_frameRate;
					if (base.currentPlayTimes == currentPlayTimes)
					{
						if (currentTime <= num12)
						{
							_OnCrossFrame(_frameIndex);
						}
					}
					else if (_position <= num12)
					{
						if (!flag && eventObject2 != null)
						{
							_armature._dragonBones.BufferEvent(eventObject2);
							eventObject2 = null;
						}
						_OnCrossFrame(_frameIndex);
					}
				}
			}
			if (eventObject2 != null)
			{
				_armature._dragonBones.BufferEvent(eventObject2);
			}
			if (eventObject3 != null)
			{
				_armature._dragonBones.BufferEvent(eventObject3);
			}
		}

		public void SetCurrentTime(float value)
		{
			_SetCurrentTime(value);
			_frameIndex = -1;
		}
	}
}
