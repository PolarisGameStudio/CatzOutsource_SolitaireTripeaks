using System.Collections.Generic;

namespace DragonBones
{
	public class AnimationConfig : BaseObject
	{
		public bool pauseFadeOut;

		public AnimationFadeOutMode fadeOutMode;

		public TweenType fadeOutTweenType;

		public float fadeOutTime;

		public bool pauseFadeIn;

		public bool actionEnabled;

		public bool additiveBlending;

		public bool displayControl;

		public bool resetToPose;

		public TweenType fadeInTweenType;

		public int playTimes;

		public int layer;

		public float position;

		public float duration;

		public float timeScale;

		public float weight;

		public float fadeInTime;

		public float autoFadeOutTime;

		public string name;

		public string animation;

		public string group;

		public readonly List<string> boneMask = new List<string>();

		protected override void _OnClear()
		{
			pauseFadeOut = true;
			fadeOutMode = AnimationFadeOutMode.All;
			fadeOutTweenType = TweenType.Line;
			fadeOutTime = -1f;
			actionEnabled = true;
			additiveBlending = false;
			displayControl = true;
			pauseFadeIn = true;
			resetToPose = true;
			fadeInTweenType = TweenType.Line;
			playTimes = -1;
			layer = 0;
			position = 0f;
			duration = -1f;
			timeScale = -100f;
			weight = 1f;
			fadeInTime = -1f;
			autoFadeOutTime = -1f;
			name = string.Empty;
			animation = string.Empty;
			group = string.Empty;
			boneMask.Clear();
		}

		public void Clear()
		{
			_OnClear();
		}

		public void CopyFrom(AnimationConfig value)
		{
			pauseFadeOut = value.pauseFadeOut;
			fadeOutMode = value.fadeOutMode;
			autoFadeOutTime = value.autoFadeOutTime;
			fadeOutTweenType = value.fadeOutTweenType;
			actionEnabled = value.actionEnabled;
			additiveBlending = value.additiveBlending;
			displayControl = value.displayControl;
			pauseFadeIn = value.pauseFadeIn;
			resetToPose = value.resetToPose;
			playTimes = value.playTimes;
			layer = value.layer;
			position = value.position;
			duration = value.duration;
			timeScale = value.timeScale;
			fadeInTime = value.fadeInTime;
			fadeOutTime = value.fadeOutTime;
			fadeInTweenType = value.fadeInTweenType;
			weight = value.weight;
			name = value.name;
			animation = value.animation;
			group = value.group;
			boneMask.ResizeList(value.boneMask.Count);
			int i = 0;
			for (int count = boneMask.Count; i < count; i++)
			{
				boneMask[i] = value.boneMask[i];
			}
		}

		public bool ContainsBoneMask(string boneName)
		{
			return boneMask.Count == 0 || boneMask.Contains(boneName);
		}

		public void AddBoneMask(Armature armature, string boneName, bool recursive = false)
		{
			Bone bone = armature.GetBone(boneName);
			if (bone == null)
			{
				return;
			}
			if (!boneMask.Contains(boneName))
			{
				boneMask.Add(boneName);
			}
			if (!recursive)
			{
				return;
			}
			List<Bone> bones = armature.GetBones();
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				Bone bone2 = bones[i];
				if (!boneMask.Contains(bone2.name) && bone.Contains(bone2))
				{
					boneMask.Add(bone2.name);
				}
			}
		}

		public void RemoveBoneMask(Armature armature, string name, bool recursive = true)
		{
			if (boneMask.Contains(name))
			{
				boneMask.Remove(name);
			}
			if (!recursive)
			{
				return;
			}
			Bone bone = armature.GetBone(name);
			if (bone == null)
			{
				return;
			}
			List<Bone> bones = armature.GetBones();
			if (boneMask.Count > 0)
			{
				int i = 0;
				for (int count = bones.Count; i < count; i++)
				{
					Bone bone2 = bones[i];
					if (boneMask.Contains(bone2.name) && bone.Contains(bone2))
					{
						boneMask.Remove(bone2.name);
					}
				}
				return;
			}
			int j = 0;
			for (int count2 = bones.Count; j < count2; j++)
			{
				Bone bone3 = bones[j];
				if (bone3 != bone && !bone.Contains(bone3))
				{
					boneMask.Add(bone3.name);
				}
			}
		}
	}
}
