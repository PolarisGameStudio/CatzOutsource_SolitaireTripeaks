using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class AnimationState : BaseObject
	{
		public bool actionEnabled;

		public bool additiveBlending;

		public bool displayControl;

		public bool resetToPose;

		public int playTimes;

		public int layer;

		public float timeScale;

		public float weight;

		public float autoFadeOutTime;

		public float fadeTotalTime;

		public string name;

		public string group;

		private int _timelineDirty;

		internal int _playheadState;

		internal int _fadeState;

		internal int _subFadeState;

		internal float _position;

		internal float _duration;

		private float _fadeTime;

		private float _time;

		internal float _fadeProgress;

		private float _weightResult;

		internal readonly BlendState _blendState = new BlendState();

		private readonly List<string> _boneMask = new List<string>();

		private readonly List<BoneTimelineState> _boneTimelines = new List<BoneTimelineState>();

		private readonly List<SlotTimelineState> _slotTimelines = new List<SlotTimelineState>();

		private readonly List<ConstraintTimelineState> _constraintTimelines = new List<ConstraintTimelineState>();

		private readonly List<TimelineState> _poseTimelines = new List<TimelineState>();

		private readonly Dictionary<string, BonePose> _bonePoses = new Dictionary<string, BonePose>();

		public AnimationData _animationData;

		private Armature _armature;

		internal ActionTimelineState _actionTimeline;

		private ZOrderTimelineState _zOrderTimeline;

		public AnimationState _parent;

		public bool isFadeIn => _fadeState < 0;

		public bool isFadeOut => _fadeState > 0;

		public bool isFadeComplete => _fadeState == 0;

		public bool isPlaying => (_playheadState & 2) != 0 && _actionTimeline.playState <= 0;

		public bool isCompleted => _actionTimeline.playState > 0;

		public int currentPlayTimes => _actionTimeline.currentPlayTimes;

		public float totalTime => _duration;

		public float currentTime
		{
			get
			{
				return _actionTimeline.currentTime;
			}
			set
			{
				int num = _actionTimeline.currentPlayTimes - ((_actionTimeline.playState > 0) ? 1 : 0);
				if (value < 0f || _duration < value)
				{
					value = value % _duration + (float)num * _duration;
					if (value < 0f)
					{
						value += _duration;
					}
				}
				if (playTimes > 0 && num == playTimes - 1 && value == _duration)
				{
					value = _duration - 1E-06f;
				}
				if (_time != value)
				{
					_time = value;
					_actionTimeline.SetCurrentTime(_time);
					if (_zOrderTimeline != null)
					{
						_zOrderTimeline.playState = -1;
					}
					foreach (BoneTimelineState boneTimeline in _boneTimelines)
					{
						boneTimeline.playState = -1;
					}
					foreach (SlotTimelineState slotTimeline in _slotTimelines)
					{
						slotTimeline.playState = -1;
					}
				}
			}
		}

		protected override void _OnClear()
		{
			foreach (BoneTimelineState boneTimeline in _boneTimelines)
			{
				boneTimeline.ReturnToPool();
			}
			foreach (SlotTimelineState slotTimeline in _slotTimelines)
			{
				slotTimeline.ReturnToPool();
			}
			foreach (ConstraintTimelineState constraintTimeline in _constraintTimelines)
			{
				constraintTimeline.ReturnToPool();
			}
			foreach (TimelineState poseTimeline in _poseTimelines)
			{
				poseTimeline.ReturnToPool();
			}
			foreach (BonePose value in _bonePoses.Values)
			{
				value.ReturnToPool();
			}
			if (_actionTimeline != null)
			{
				_actionTimeline.ReturnToPool();
			}
			if (_zOrderTimeline != null)
			{
				_zOrderTimeline.ReturnToPool();
			}
			actionEnabled = false;
			additiveBlending = false;
			displayControl = false;
			resetToPose = false;
			playTimes = 1;
			layer = 0;
			timeScale = 1f;
			weight = 1f;
			autoFadeOutTime = 0f;
			fadeTotalTime = 0f;
			name = string.Empty;
			group = string.Empty;
			_timelineDirty = 2;
			_playheadState = 0;
			_fadeState = -1;
			_subFadeState = -1;
			_position = 0f;
			_duration = 0f;
			_fadeTime = 0f;
			_time = 0f;
			_fadeProgress = 0f;
			_weightResult = 0f;
			_blendState.Clear();
			_boneMask.Clear();
			_boneTimelines.Clear();
			_slotTimelines.Clear();
			_constraintTimelines.Clear();
			_poseTimelines.Clear();
			_bonePoses.Clear();
			_animationData = null;
			_armature = null;
			_actionTimeline = null;
			_zOrderTimeline = null;
			_parent = null;
		}

		private void _UpdateTimelines()
		{
			foreach (Constraint constraint in _armature._constraints)
			{
				List<TimelineData> constraintTimelines = _animationData.GetConstraintTimelines(constraint.name);
				if (constraintTimelines != null)
				{
					foreach (TimelineData item in constraintTimelines)
					{
						TimelineType type = item.type;
						if (type == TimelineType.IKConstraint)
						{
							IKConstraintTimelineState iKConstraintTimelineState = BaseObject.BorrowObject<IKConstraintTimelineState>();
							iKConstraintTimelineState.constraint = constraint;
							iKConstraintTimelineState.Init(_armature, this, item);
							_constraintTimelines.Add(iKConstraintTimelineState);
						}
					}
				}
				else if (resetToPose)
				{
					IKConstraintTimelineState iKConstraintTimelineState2 = BaseObject.BorrowObject<IKConstraintTimelineState>();
					iKConstraintTimelineState2.constraint = constraint;
					iKConstraintTimelineState2.Init(_armature, this, null);
					_constraintTimelines.Add(iKConstraintTimelineState2);
					_poseTimelines.Add(iKConstraintTimelineState2);
				}
			}
		}

		private void _UpdateBoneAndSlotTimelines()
		{
			Dictionary<string, List<BoneTimelineState>> dictionary = new Dictionary<string, List<BoneTimelineState>>();
			foreach (BoneTimelineState boneTimeline in _boneTimelines)
			{
				string key = boneTimeline.bone.name;
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = new List<BoneTimelineState>();
				}
				dictionary[key].Add(boneTimeline);
			}
			foreach (Bone bone in _armature.GetBones())
			{
				string text = bone.name;
				if (ContainsBoneMask(text))
				{
					List<TimelineData> boneTimelines = _animationData.GetBoneTimelines(text);
					if (dictionary.ContainsKey(text))
					{
						dictionary.Remove(text);
					}
					else
					{
						object bonePose;
						if (_bonePoses.ContainsKey(text))
						{
							bonePose = _bonePoses[text];
						}
						else
						{
							BonePose bonePose2 = BaseObject.BorrowObject<BonePose>();
							_bonePoses[text] = bonePose2;
							bonePose = bonePose2;
						}
						BonePose bonePose3 = (BonePose)bonePose;
						if (boneTimelines != null)
						{
							foreach (TimelineData item in boneTimelines)
							{
								switch (item.type)
								{
								case TimelineType.BoneAll:
								{
									BoneAllTimelineState boneAllTimelineState = BaseObject.BorrowObject<BoneAllTimelineState>();
									boneAllTimelineState.bone = bone;
									boneAllTimelineState.bonePose = bonePose3;
									boneAllTimelineState.Init(_armature, this, item);
									_boneTimelines.Add(boneAllTimelineState);
									break;
								}
								case TimelineType.BoneTranslate:
								{
									BoneTranslateTimelineState boneTranslateTimelineState = BaseObject.BorrowObject<BoneTranslateTimelineState>();
									boneTranslateTimelineState.bone = bone;
									boneTranslateTimelineState.bonePose = bonePose3;
									boneTranslateTimelineState.Init(_armature, this, item);
									_boneTimelines.Add(boneTranslateTimelineState);
									break;
								}
								case TimelineType.BoneRotate:
								{
									BoneRotateTimelineState boneRotateTimelineState = BaseObject.BorrowObject<BoneRotateTimelineState>();
									boneRotateTimelineState.bone = bone;
									boneRotateTimelineState.bonePose = bonePose3;
									boneRotateTimelineState.Init(_armature, this, item);
									_boneTimelines.Add(boneRotateTimelineState);
									break;
								}
								case TimelineType.BoneScale:
								{
									BoneScaleTimelineState boneScaleTimelineState = BaseObject.BorrowObject<BoneScaleTimelineState>();
									boneScaleTimelineState.bone = bone;
									boneScaleTimelineState.bonePose = bonePose3;
									boneScaleTimelineState.Init(_armature, this, item);
									_boneTimelines.Add(boneScaleTimelineState);
									break;
								}
								}
							}
						}
						else if (resetToPose)
						{
							BoneAllTimelineState boneAllTimelineState2 = BaseObject.BorrowObject<BoneAllTimelineState>();
							boneAllTimelineState2.bone = bone;
							boneAllTimelineState2.bonePose = bonePose3;
							boneAllTimelineState2.Init(_armature, this, null);
							_boneTimelines.Add(boneAllTimelineState2);
							_poseTimelines.Add(boneAllTimelineState2);
						}
					}
				}
			}
			foreach (List<BoneTimelineState> value in dictionary.Values)
			{
				foreach (BoneTimelineState item2 in value)
				{
					_boneTimelines.Remove(item2);
					item2.ReturnToPool();
				}
			}
			Dictionary<string, List<SlotTimelineState>> dictionary2 = new Dictionary<string, List<SlotTimelineState>>();
			List<int> list = new List<int>();
			foreach (SlotTimelineState slotTimeline in _slotTimelines)
			{
				string key2 = slotTimeline.slot.name;
				if (!dictionary2.ContainsKey(key2))
				{
					dictionary2[key2] = new List<SlotTimelineState>();
				}
				dictionary2[key2].Add(slotTimeline);
			}
			foreach (Slot slot in _armature.GetSlots())
			{
				string boneName = slot.parent.name;
				if (ContainsBoneMask(boneName))
				{
					string text2 = slot.name;
					List<TimelineData> slotTimelines = _animationData.GetSlotTimelines(text2);
					if (dictionary2.ContainsKey(text2))
					{
						dictionary2.Remove(text2);
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						list.Clear();
						if (slotTimelines != null)
						{
							foreach (TimelineData item3 in slotTimelines)
							{
								switch (item3.type)
								{
								case TimelineType.SlotDisplay:
								{
									SlotDislayTimelineState slotDislayTimelineState = BaseObject.BorrowObject<SlotDislayTimelineState>();
									slotDislayTimelineState.slot = slot;
									slotDislayTimelineState.Init(_armature, this, item3);
									_slotTimelines.Add(slotDislayTimelineState);
									flag = true;
									break;
								}
								case TimelineType.SlotColor:
								{
									SlotColorTimelineState slotColorTimelineState = BaseObject.BorrowObject<SlotColorTimelineState>();
									slotColorTimelineState.slot = slot;
									slotColorTimelineState.Init(_armature, this, item3);
									_slotTimelines.Add(slotColorTimelineState);
									flag2 = true;
									break;
								}
								case TimelineType.SlotDeform:
								{
									DeformTimelineState deformTimelineState = BaseObject.BorrowObject<DeformTimelineState>();
									deformTimelineState.slot = slot;
									deformTimelineState.Init(_armature, this, item3);
									_slotTimelines.Add(deformTimelineState);
									list.Add(deformTimelineState.vertexOffset);
									break;
								}
								}
							}
						}
						if (resetToPose)
						{
							if (!flag)
							{
								SlotDislayTimelineState slotDislayTimelineState2 = BaseObject.BorrowObject<SlotDislayTimelineState>();
								slotDislayTimelineState2.slot = slot;
								slotDislayTimelineState2.Init(_armature, this, null);
								_slotTimelines.Add(slotDislayTimelineState2);
								_poseTimelines.Add(slotDislayTimelineState2);
							}
							if (!flag2)
							{
								SlotColorTimelineState slotColorTimelineState2 = BaseObject.BorrowObject<SlotColorTimelineState>();
								slotColorTimelineState2.slot = slot;
								slotColorTimelineState2.Init(_armature, this, null);
								_slotTimelines.Add(slotColorTimelineState2);
								_poseTimelines.Add(slotColorTimelineState2);
							}
							if (slot.rawDisplayDatas != null)
							{
								foreach (DisplayData rawDisplayData in slot.rawDisplayDatas)
								{
									if (rawDisplayData != null && rawDisplayData.type == DisplayType.Mesh)
									{
										int offset = (rawDisplayData as MeshDisplayData).vertices.offset;
										if (!list.Contains(offset))
										{
											DeformTimelineState deformTimelineState2 = BaseObject.BorrowObject<DeformTimelineState>();
											deformTimelineState2.vertexOffset = offset;
											deformTimelineState2.slot = slot;
											deformTimelineState2.Init(_armature, this, null);
											_slotTimelines.Add(deformTimelineState2);
											_poseTimelines.Add(deformTimelineState2);
										}
									}
								}
							}
						}
					}
				}
			}
			foreach (List<SlotTimelineState> value2 in dictionary2.Values)
			{
				foreach (SlotTimelineState item4 in value2)
				{
					_slotTimelines.Remove(item4);
					item4.ReturnToPool();
				}
			}
		}

		private void _AdvanceFadeTime(float passedTime)
		{
			bool flag = _fadeState > 0;
			if (_subFadeState < 0)
			{
				_subFadeState = 0;
				string type = (!flag) ? "fadeIn" : "fadeOut";
				if (_armature.eventDispatcher.HasDBEventListener(type))
				{
					EventObject eventObject = BaseObject.BorrowObject<EventObject>();
					eventObject.type = type;
					eventObject.armature = _armature;
					eventObject.animationState = this;
					_armature._dragonBones.BufferEvent(eventObject);
				}
			}
			if (passedTime < 0f)
			{
				passedTime = 0f - passedTime;
			}
			_fadeTime += passedTime;
			if (_fadeTime >= fadeTotalTime)
			{
				_subFadeState = 1;
				_fadeProgress = ((!flag) ? 1f : 0f);
			}
			else if (_fadeTime > 0f)
			{
				_fadeProgress = ((!flag) ? (_fadeTime / fadeTotalTime) : (1f - _fadeTime / fadeTotalTime));
			}
			else
			{
				_fadeProgress = ((!flag) ? 0f : 1f);
			}
			if (_subFadeState > 0)
			{
				if (!flag)
				{
					_playheadState |= 1;
					_fadeState = 0;
				}
				string type2 = (!flag) ? "fadeInComplete" : "fadeOutComplete";
				if (_armature.eventDispatcher.HasDBEventListener(type2))
				{
					EventObject eventObject2 = BaseObject.BorrowObject<EventObject>();
					eventObject2.type = type2;
					eventObject2.armature = _armature;
					eventObject2.animationState = this;
					_armature._dragonBones.BufferEvent(eventObject2);
				}
			}
		}

		internal void Init(Armature armature, AnimationData animationData, AnimationConfig animationConfig)
		{
			if (_armature != null)
			{
				return;
			}
			_armature = armature;
			_animationData = animationData;
			resetToPose = animationConfig.resetToPose;
			additiveBlending = animationConfig.additiveBlending;
			displayControl = animationConfig.displayControl;
			actionEnabled = animationConfig.actionEnabled;
			layer = animationConfig.layer;
			playTimes = animationConfig.playTimes;
			timeScale = animationConfig.timeScale;
			fadeTotalTime = animationConfig.fadeInTime;
			autoFadeOutTime = animationConfig.autoFadeOutTime;
			weight = animationConfig.weight;
			name = ((animationConfig.name.Length <= 0) ? animationConfig.animation : animationConfig.name);
			group = animationConfig.group;
			if (animationConfig.pauseFadeIn)
			{
				_playheadState = 2;
			}
			else
			{
				_playheadState = 3;
			}
			if (animationConfig.duration < 0f)
			{
				_position = 0f;
				_duration = _animationData.duration;
				if (animationConfig.position != 0f)
				{
					if (timeScale >= 0f)
					{
						_time = animationConfig.position;
					}
					else
					{
						_time = animationConfig.position - _duration;
					}
				}
				else
				{
					_time = 0f;
				}
			}
			else
			{
				_position = animationConfig.position;
				_duration = animationConfig.duration;
				_time = 0f;
			}
			if (timeScale < 0f && _time == 0f)
			{
				_time = -1E-06f;
			}
			if (fadeTotalTime <= 0f)
			{
				_fadeProgress = 0.999999f;
			}
			if (animationConfig.boneMask.Count > 0)
			{
				_boneMask.ResizeList(animationConfig.boneMask.Count);
				int i = 0;
				for (int count = _boneMask.Count; i < count; i++)
				{
					_boneMask[i] = animationConfig.boneMask[i];
				}
			}
			_actionTimeline = BaseObject.BorrowObject<ActionTimelineState>();
			_actionTimeline.Init(_armature, this, _animationData.actionTimeline);
			_actionTimeline.currentTime = _time;
			if (_actionTimeline.currentTime < 0f)
			{
				_actionTimeline.currentTime = _duration - _actionTimeline.currentTime;
			}
			if (_animationData.zOrderTimeline != null)
			{
				_zOrderTimeline = BaseObject.BorrowObject<ZOrderTimelineState>();
				_zOrderTimeline.Init(_armature, this, _animationData.zOrderTimeline);
			}
		}

		internal void AdvanceTime(float passedTime, float cacheFrameRate)
		{
			_blendState.dirty = true;
			if (_fadeState != 0 || _subFadeState != 0)
			{
				_AdvanceFadeTime(passedTime);
			}
			if (_playheadState == 3)
			{
				if (timeScale != 1f)
				{
					passedTime *= timeScale;
				}
				_time += passedTime;
			}
			if (_timelineDirty != 0)
			{
				if (_timelineDirty == 2)
				{
					_UpdateTimelines();
				}
				_timelineDirty = 0;
				_UpdateBoneAndSlotTimelines();
			}
			if (weight == 0f)
			{
				return;
			}
			bool flag = _fadeState == 0 && cacheFrameRate > 0f;
			bool flag2 = true;
			bool flag3 = true;
			float time = _time;
			_weightResult = weight * _fadeProgress;
			if (_parent != null)
			{
				_weightResult *= _parent._weightResult / _parent._fadeProgress;
			}
			if (_actionTimeline.playState <= 0)
			{
				_actionTimeline.Update(time);
			}
			if (flag)
			{
				float num = cacheFrameRate * 2f;
				_actionTimeline.currentTime = (float)Math.Floor(_actionTimeline.currentTime * num) / num;
			}
			if (_zOrderTimeline != null && _zOrderTimeline.playState <= 0)
			{
				_zOrderTimeline.Update(time);
			}
			if (flag)
			{
				int num2 = (int)Math.Floor(_actionTimeline.currentTime * cacheFrameRate);
				if (_armature._cacheFrameIndex == num2)
				{
					flag2 = false;
					flag3 = false;
				}
				else
				{
					_armature._cacheFrameIndex = num2;
					if (_animationData.cachedFrames[num2])
					{
						flag3 = false;
					}
					else
					{
						_animationData.cachedFrames[num2] = true;
					}
				}
			}
			if (flag2)
			{
				if (flag3)
				{
					int i = 0;
					for (int count = _boneTimelines.Count; i < count; i++)
					{
						BoneTimelineState boneTimelineState = _boneTimelines[i];
						if (boneTimelineState.playState <= 0)
						{
							boneTimelineState.Update(time);
						}
						if (i == count - 1 || boneTimelineState.bone != _boneTimelines[i + 1].bone)
						{
							int num3 = boneTimelineState.bone._blendState.Update(_weightResult, layer);
							if (num3 != 0)
							{
								boneTimelineState.Blend(num3);
							}
						}
					}
				}
				if (displayControl)
				{
					int j = 0;
					for (int count2 = _slotTimelines.Count; j < count2; j++)
					{
						SlotTimelineState slotTimelineState = _slotTimelines[j];
						string displayController = slotTimelineState.slot.displayController;
						if ((displayController == null || displayController == name || displayController == group) && slotTimelineState.playState <= 0)
						{
							slotTimelineState.Update(time);
						}
					}
				}
				int k = 0;
				for (int count3 = _constraintTimelines.Count; k < count3; k++)
				{
					ConstraintTimelineState constraintTimelineState = _constraintTimelines[k];
					if (constraintTimelineState.playState <= 0)
					{
						constraintTimelineState.Update(time);
					}
				}
			}
			if (_fadeState != 0)
			{
				return;
			}
			if (_subFadeState > 0)
			{
				_subFadeState = 0;
				if (_poseTimelines.Count > 0)
				{
					foreach (TimelineState poseTimeline in _poseTimelines)
					{
						if (poseTimeline is BoneTimelineState)
						{
							_boneTimelines.Remove(poseTimeline as BoneTimelineState);
						}
						else if (poseTimeline is SlotTimelineState)
						{
							_slotTimelines.Remove(poseTimeline as SlotTimelineState);
						}
						else if (poseTimeline is ConstraintTimelineState)
						{
							_constraintTimelines.Remove(poseTimeline as ConstraintTimelineState);
						}
						poseTimeline.ReturnToPool();
					}
					_poseTimelines.Clear();
				}
			}
			if (_actionTimeline.playState > 0 && autoFadeOutTime >= 0f)
			{
				FadeOut(autoFadeOutTime);
			}
		}

		public void Play()
		{
			_playheadState = 3;
		}

		public void Stop()
		{
			_playheadState &= 1;
		}

		public void FadeOut(float fadeOutTime, bool pausePlayhead = true)
		{
			if (fadeOutTime < 0f)
			{
				fadeOutTime = 0f;
			}
			if (pausePlayhead)
			{
				_playheadState &= 2;
			}
			if (_fadeState > 0)
			{
				if (fadeOutTime > fadeTotalTime - _fadeTime)
				{
					return;
				}
			}
			else
			{
				_fadeState = 1;
				_subFadeState = -1;
				if (fadeOutTime <= 0f || _fadeProgress <= 0f)
				{
					_fadeProgress = 1E-06f;
				}
				foreach (BoneTimelineState boneTimeline in _boneTimelines)
				{
					boneTimeline.FadeOut();
				}
				foreach (SlotTimelineState slotTimeline in _slotTimelines)
				{
					slotTimeline.FadeOut();
				}
				foreach (ConstraintTimelineState constraintTimeline in _constraintTimelines)
				{
					constraintTimeline.FadeOut();
				}
			}
			displayControl = false;
			fadeTotalTime = ((!(_fadeProgress > 1E-06f)) ? 0f : (fadeOutTime / _fadeProgress));
			_fadeTime = fadeTotalTime * (1f - _fadeProgress);
		}

		public bool ContainsBoneMask(string boneName)
		{
			return _boneMask.Count == 0 || _boneMask.IndexOf(boneName) >= 0;
		}

		public void AddBoneMask(string boneName, bool recursive = true)
		{
			Bone bone = _armature.GetBone(boneName);
			if (bone != null)
			{
				if (_boneMask.IndexOf(boneName) < 0)
				{
					_boneMask.Add(boneName);
				}
				if (recursive)
				{
					foreach (Bone bone2 in _armature.GetBones())
					{
						if (_boneMask.IndexOf(bone2.name) < 0 && bone.Contains(bone2))
						{
							_boneMask.Add(bone2.name);
						}
					}
				}
				_timelineDirty = 1;
			}
		}

		public void RemoveBoneMask(string boneName, bool recursive = true)
		{
			if (_boneMask.Contains(boneName))
			{
				_boneMask.Remove(boneName);
			}
			if (recursive)
			{
				Bone bone = _armature.GetBone(boneName);
				if (bone != null)
				{
					List<Bone> bones = _armature.GetBones();
					if (_boneMask.Count > 0)
					{
						foreach (Bone item in bones)
						{
							if (_boneMask.Contains(item.name) && bone.Contains(item))
							{
								_boneMask.Remove(item.name);
							}
						}
					}
					else
					{
						foreach (Bone item2 in bones)
						{
							if (item2 != bone && !bone.Contains(item2))
							{
								_boneMask.Add(item2.name);
							}
						}
					}
				}
			}
			_timelineDirty = 1;
		}

		public void RemoveAllBoneMask()
		{
			_boneMask.Clear();
			_timelineDirty = 1;
		}
	}
}
