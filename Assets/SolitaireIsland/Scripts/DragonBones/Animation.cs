using System.Collections.Generic;

namespace DragonBones
{
	public class Animation : BaseObject
	{
		public float timeScale;

		private bool _lockUpdate;

		private bool _animationDirty;

		private float _inheritTimeScale;

		private readonly List<string> _animationNames = new List<string>();

		private readonly List<AnimationState> _animationStates = new List<AnimationState>();

		private readonly Dictionary<string, AnimationData> _animations = new Dictionary<string, AnimationData>();

		private Armature _armature;

		private AnimationConfig _animationConfig;

		private AnimationState _lastAnimationState;

		public bool isPlaying
		{
			get
			{
				foreach (AnimationState animationState in _animationStates)
				{
					if (animationState.isPlaying)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isCompleted
		{
			get
			{
				foreach (AnimationState animationState in _animationStates)
				{
					if (!animationState.isCompleted)
					{
						return false;
					}
				}
				return _animationStates.Count > 0;
			}
		}

		public string lastAnimationName => (_lastAnimationState == null) ? string.Empty : _lastAnimationState.name;

		public List<string> animationNames => _animationNames;

		public Dictionary<string, AnimationData> animations
		{
			get
			{
				return _animations;
			}
			set
			{
				if (_animations != value)
				{
					_animationNames.Clear();
					_animations.Clear();
					foreach (KeyValuePair<string, AnimationData> item in value)
					{
						_animationNames.Add(item.Key);
						_animations[item.Key] = value[item.Key];
					}
				}
			}
		}

		public AnimationConfig animationConfig
		{
			get
			{
				_animationConfig.Clear();
				return _animationConfig;
			}
		}

		public AnimationState lastAnimationState => _lastAnimationState;

		protected override void _OnClear()
		{
			foreach (AnimationState animationState in _animationStates)
			{
				animationState.ReturnToPool();
			}
			if (_animationConfig != null)
			{
				_animationConfig.ReturnToPool();
			}
			timeScale = 1f;
			_lockUpdate = false;
			_animationDirty = false;
			_inheritTimeScale = 1f;
			_animationNames.Clear();
			_animationStates.Clear();
			_animations.Clear();
			_armature = null;
			_animationConfig = null;
			_lastAnimationState = null;
		}

		private void _FadeOut(AnimationConfig animationConfig)
		{
			switch (animationConfig.fadeOutMode)
			{
			case AnimationFadeOutMode.None:
			case AnimationFadeOutMode.Single:
				break;
			case AnimationFadeOutMode.SameLayer:
				foreach (AnimationState animationState in _animationStates)
				{
					if (animationState._parent == null && animationState.layer == animationConfig.layer)
					{
						animationState.FadeOut(animationConfig.fadeOutTime, animationConfig.pauseFadeOut);
					}
				}
				break;
			case AnimationFadeOutMode.SameGroup:
				foreach (AnimationState animationState2 in _animationStates)
				{
					if (animationState2._parent == null && animationState2.group == animationConfig.group)
					{
						animationState2.FadeOut(animationConfig.fadeOutTime, animationConfig.pauseFadeOut);
					}
				}
				break;
			case AnimationFadeOutMode.SameLayerAndGroup:
				foreach (AnimationState animationState3 in _animationStates)
				{
					if (animationState3._parent == null && animationState3.layer == animationConfig.layer && animationState3.group == animationConfig.group)
					{
						animationState3.FadeOut(animationConfig.fadeOutTime, animationConfig.pauseFadeOut);
					}
				}
				break;
			case AnimationFadeOutMode.All:
				foreach (AnimationState animationState4 in _animationStates)
				{
					if (animationState4._parent == null)
					{
						animationState4.FadeOut(animationConfig.fadeOutTime, animationConfig.pauseFadeOut);
					}
				}
				break;
			}
		}

		internal void Init(Armature armature)
		{
			if (_armature == null)
			{
				_armature = armature;
				_animationConfig = BaseObject.BorrowObject<AnimationConfig>();
			}
		}

		internal void AdvanceTime(float passedTime)
		{
			if (passedTime < 0f)
			{
				passedTime = 0f - passedTime;
			}
			if (_armature.inheritAnimation && _armature._parent != null)
			{
				_inheritTimeScale = _armature._parent._armature.animation._inheritTimeScale * timeScale;
			}
			else
			{
				_inheritTimeScale = timeScale;
			}
			if (_inheritTimeScale != 1f)
			{
				passedTime *= _inheritTimeScale;
			}
			int count = _animationStates.Count;
			if (count == 1)
			{
				AnimationState animationState = _animationStates[0];
				if (animationState._fadeState > 0 && animationState._subFadeState > 0)
				{
					_armature._dragonBones.BufferObject(animationState);
					_animationStates.Clear();
					_lastAnimationState = null;
					return;
				}
				AnimationData animationData = animationState._animationData;
				float cacheFrameRate = animationData.cacheFrameRate;
				if (_animationDirty && cacheFrameRate > 0f)
				{
					_animationDirty = false;
					foreach (Bone bone in _armature.GetBones())
					{
						bone._cachedFrameIndices = animationData.GetBoneCachedFrameIndices(bone.name);
					}
					foreach (Slot slot in _armature.GetSlots())
					{
						List<DisplayData> rawDisplayDatas = slot.rawDisplayDatas;
						if (rawDisplayDatas != null && rawDisplayDatas.Count > 0)
						{
							DisplayData displayData = rawDisplayDatas[0];
							if (displayData != null && displayData.parent == _armature.armatureData.defaultSkin)
							{
								slot._cachedFrameIndices = animationData.GetSlotCachedFrameIndices(slot.name);
								continue;
							}
						}
						slot._cachedFrameIndices = null;
					}
				}
				animationState.AdvanceTime(passedTime, cacheFrameRate);
			}
			else if (count > 1)
			{
				int i = 0;
				int num = 0;
				for (; i < count; i++)
				{
					AnimationState animationState2 = _animationStates[i];
					if (animationState2._fadeState > 0 && animationState2._subFadeState > 0)
					{
						num++;
						_armature._dragonBones.BufferObject(animationState2);
						_animationDirty = true;
						if (_lastAnimationState == animationState2)
						{
							_lastAnimationState = null;
						}
					}
					else
					{
						if (num > 0)
						{
							_animationStates[i - num] = animationState2;
						}
						animationState2.AdvanceTime(passedTime, 0f);
					}
					if (i == count - 1 && num > 0)
					{
						_animationStates.ResizeList(_animationStates.Count - num);
						if (_lastAnimationState == null && _animationStates.Count > 0)
						{
							_lastAnimationState = _animationStates[_animationStates.Count - 1];
						}
					}
				}
				_armature._cacheFrameIndex = -1;
			}
			else
			{
				_armature._cacheFrameIndex = -1;
			}
		}

		public void Reset()
		{
			foreach (AnimationState animationState in _animationStates)
			{
				animationState.ReturnToPool();
			}
			_animationDirty = false;
			_animationConfig.Clear();
			_animationStates.Clear();
			_lastAnimationState = null;
		}

		public void Stop(string animationName = null)
		{
			if (animationName != null)
			{
				GetState(animationName)?.Stop();
			}
			else
			{
				foreach (AnimationState animationState in _animationStates)
				{
					animationState.Stop();
				}
			}
		}

		public AnimationState PlayConfig(AnimationConfig animationConfig)
		{
			string animation = animationConfig.animation;
			if (!_animations.ContainsKey(animation))
			{
				Helper.Assert(condition: false, "Non-existent animation.\nDragonBones name: " + _armature.armatureData.parent.name + "Armature name: " + _armature.name + "Animation name: " + animation);
				return null;
			}
			AnimationData animationData = _animations[animation];
			if (animationConfig.fadeOutMode == AnimationFadeOutMode.Single)
			{
				foreach (AnimationState animationState2 in _animationStates)
				{
					if (animationState2._animationData == animationData)
					{
						return animationState2;
					}
				}
			}
			if (_animationStates.Count == 0)
			{
				animationConfig.fadeInTime = 0f;
			}
			else if (animationConfig.fadeInTime < 0f)
			{
				animationConfig.fadeInTime = animationData.fadeInTime;
			}
			if (animationConfig.fadeOutTime < 0f)
			{
				animationConfig.fadeOutTime = animationConfig.fadeInTime;
			}
			if (animationConfig.timeScale <= -100f)
			{
				animationConfig.timeScale = 1f / animationData.scale;
			}
			if (animationData.frameCount > 1)
			{
				if (animationConfig.position < 0f)
				{
					animationConfig.position %= animationData.duration;
					animationConfig.position = animationData.duration - animationConfig.position;
				}
				else if (animationConfig.position == animationData.duration)
				{
					animationConfig.position -= 1E-06f;
				}
				else if (animationConfig.position > animationData.duration)
				{
					animationConfig.position %= animationData.duration;
				}
				if (animationConfig.duration > 0f && animationConfig.position + animationConfig.duration > animationData.duration)
				{
					animationConfig.duration = animationData.duration - animationConfig.position;
				}
				if (animationConfig.playTimes < 0)
				{
					animationConfig.playTimes = (int)animationData.playTimes;
				}
			}
			else
			{
				animationConfig.playTimes = 1;
				animationConfig.position = 0f;
				if ((double)animationConfig.duration > 0.0)
				{
					animationConfig.duration = 0f;
				}
			}
			if (animationConfig.duration == 0f)
			{
				animationConfig.duration = -1f;
			}
			_FadeOut(animationConfig);
			AnimationState animationState = BaseObject.BorrowObject<AnimationState>();
			animationState.Init(_armature, animationData, animationConfig);
			_animationDirty = true;
			_armature._cacheFrameIndex = -1;
			if (_animationStates.Count > 0)
			{
				bool flag = false;
				int i = 0;
				for (int count = _animationStates.Count; i < count; i++)
				{
					if (animationState.layer > _animationStates[i].layer)
					{
						flag = true;
						_animationStates.Insert(i, animationState);
						break;
					}
					if (i != count - 1 && animationState.layer > _animationStates[i + 1].layer)
					{
						flag = true;
						_animationStates.Insert(i + 1, animationState);
						break;
					}
				}
				if (!flag)
				{
					_animationStates.Add(animationState);
				}
			}
			else
			{
				_animationStates.Add(animationState);
			}
			foreach (Slot slot in _armature.GetSlots())
			{
				Armature childArmature = slot.childArmature;
				if (childArmature != null && childArmature.inheritAnimation && childArmature.animation.HasAnimation(animation) && childArmature.animation.GetState(animation) == null)
				{
					childArmature.animation.FadeIn(animation);
				}
			}
			if (!_lockUpdate && animationConfig.fadeInTime <= 0f)
			{
				_armature.AdvanceTime(0f);
			}
			_lastAnimationState = animationState;
			return animationState;
		}

		public AnimationState Play(string animationName = null, int playTimes = -1)
		{
			_animationConfig.Clear();
			_animationConfig.resetToPose = true;
			_animationConfig.playTimes = playTimes;
			_animationConfig.fadeInTime = 0f;
			_animationConfig.animation = ((animationName == null) ? string.Empty : animationName);
			if (animationName != null && animationName.Length > 0)
			{
				PlayConfig(_animationConfig);
			}
			else if (_lastAnimationState == null)
			{
				AnimationData defaultAnimation = _armature.armatureData.defaultAnimation;
				if (defaultAnimation != null)
				{
					_animationConfig.animation = defaultAnimation.name;
					PlayConfig(_animationConfig);
				}
			}
			else if (!_lastAnimationState.isPlaying && !_lastAnimationState.isCompleted)
			{
				_lastAnimationState.Play();
			}
			else
			{
				_animationConfig.animation = _lastAnimationState.name;
				PlayConfig(_animationConfig);
			}
			return _lastAnimationState;
		}

		public AnimationState FadeIn(string animationName, float fadeInTime = -1f, int playTimes = -1, int layer = 0, string group = null, AnimationFadeOutMode fadeOutMode = AnimationFadeOutMode.SameLayerAndGroup)
		{
			_animationConfig.Clear();
			_animationConfig.fadeOutMode = fadeOutMode;
			_animationConfig.playTimes = playTimes;
			_animationConfig.layer = layer;
			_animationConfig.fadeInTime = fadeInTime;
			_animationConfig.animation = animationName;
			_animationConfig.group = ((group == null) ? string.Empty : group);
			return PlayConfig(_animationConfig);
		}

		public AnimationState GotoAndPlayByTime(string animationName, float time = 0f, int playTimes = -1)
		{
			_animationConfig.Clear();
			_animationConfig.resetToPose = true;
			_animationConfig.playTimes = playTimes;
			_animationConfig.position = time;
			_animationConfig.fadeInTime = 0f;
			_animationConfig.animation = animationName;
			return PlayConfig(_animationConfig);
		}

		public AnimationState GotoAndPlayByFrame(string animationName, uint frame = 0u, int playTimes = -1)
		{
			_animationConfig.Clear();
			_animationConfig.resetToPose = true;
			_animationConfig.playTimes = playTimes;
			_animationConfig.fadeInTime = 0f;
			_animationConfig.animation = animationName;
			AnimationData animationData = (!_animations.ContainsKey(animationName)) ? null : _animations[animationName];
			if (animationData != null)
			{
				_animationConfig.position = animationData.duration * (float)(double)frame / (float)(double)animationData.frameCount;
			}
			return PlayConfig(_animationConfig);
		}

		public AnimationState GotoAndPlayByProgress(string animationName, float progress = 0f, int playTimes = -1)
		{
			_animationConfig.Clear();
			_animationConfig.resetToPose = true;
			_animationConfig.playTimes = playTimes;
			_animationConfig.fadeInTime = 0f;
			_animationConfig.animation = animationName;
			AnimationData animationData = (!_animations.ContainsKey(animationName)) ? null : _animations[animationName];
			if (animationData != null)
			{
				_animationConfig.position = animationData.duration * ((!(progress > 0f)) ? 0f : progress);
			}
			return PlayConfig(_animationConfig);
		}

		public AnimationState GotoAndStopByTime(string animationName, float time = 0f)
		{
			AnimationState animationState = GotoAndPlayByTime(animationName, time, 1);
			animationState?.Stop();
			return animationState;
		}

		public AnimationState GotoAndStopByFrame(string animationName, uint frame = 0u)
		{
			AnimationState animationState = GotoAndPlayByFrame(animationName, frame, 1);
			animationState?.Stop();
			return animationState;
		}

		public AnimationState GotoAndStopByProgress(string animationName, float progress = 0f)
		{
			AnimationState animationState = GotoAndPlayByProgress(animationName, progress, 1);
			animationState?.Stop();
			return animationState;
		}

		public AnimationState GetState(string animationName)
		{
			int num = _animationStates.Count;
			while (num-- > 0)
			{
				AnimationState animationState = _animationStates[num];
				if (animationState.name == animationName)
				{
					return animationState;
				}
			}
			return null;
		}

		public bool HasAnimation(string animationName)
		{
			return _animations.ContainsKey(animationName);
		}

		public List<AnimationState> GetStates()
		{
			return _animationStates;
		}
	}
}
