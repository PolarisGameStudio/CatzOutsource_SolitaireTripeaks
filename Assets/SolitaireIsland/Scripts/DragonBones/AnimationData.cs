using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class AnimationData : BaseObject
	{
		public uint frameIntOffset;

		public uint frameFloatOffset;

		public uint frameOffset;

		public uint frameCount;

		public uint playTimes;

		public float duration;

		public float scale;

		public float fadeInTime;

		public float cacheFrameRate;

		public string name;

		public readonly List<bool> cachedFrames = new List<bool>();

		public readonly Dictionary<string, List<TimelineData>> boneTimelines = new Dictionary<string, List<TimelineData>>();

		public readonly Dictionary<string, List<TimelineData>> slotTimelines = new Dictionary<string, List<TimelineData>>();

		public readonly Dictionary<string, List<TimelineData>> constraintTimelines = new Dictionary<string, List<TimelineData>>();

		public readonly Dictionary<string, List<int>> boneCachedFrameIndices = new Dictionary<string, List<int>>();

		public readonly Dictionary<string, List<int>> slotCachedFrameIndices = new Dictionary<string, List<int>>();

		public TimelineData actionTimeline;

		public TimelineData zOrderTimeline;

		public ArmatureData parent;

		protected override void _OnClear()
		{
			foreach (KeyValuePair<string, List<TimelineData>> boneTimeline in boneTimelines)
			{
				for (int i = 0; i < boneTimeline.Value.Count; i++)
				{
					boneTimeline.Value[i].ReturnToPool();
				}
			}
			foreach (KeyValuePair<string, List<TimelineData>> slotTimeline in slotTimelines)
			{
				for (int j = 0; j < slotTimeline.Value.Count; j++)
				{
					slotTimeline.Value[j].ReturnToPool();
				}
			}
			foreach (KeyValuePair<string, List<TimelineData>> constraintTimeline in constraintTimelines)
			{
				for (int k = 0; k < constraintTimeline.Value.Count; k++)
				{
					constraintTimeline.Value[k].ReturnToPool();
				}
			}
			if (actionTimeline != null)
			{
				actionTimeline.ReturnToPool();
			}
			if (zOrderTimeline != null)
			{
				zOrderTimeline.ReturnToPool();
			}
			frameIntOffset = 0u;
			frameFloatOffset = 0u;
			frameOffset = 0u;
			frameCount = 0u;
			playTimes = 0u;
			duration = 0f;
			scale = 1f;
			fadeInTime = 0f;
			cacheFrameRate = 0f;
			name = string.Empty;
			boneTimelines.Clear();
			slotTimelines.Clear();
			constraintTimelines.Clear();
			boneCachedFrameIndices.Clear();
			slotCachedFrameIndices.Clear();
			cachedFrames.Clear();
			actionTimeline = null;
			zOrderTimeline = null;
			parent = null;
		}

		public void CacheFrames(float frameRate)
		{
			if (!(cacheFrameRate > 0f))
			{
				cacheFrameRate = Math.Max((float)Math.Ceiling(frameRate * scale), 1f);
				int num = (int)Math.Ceiling(cacheFrameRate * duration) + 1;
				cachedFrames.ResizeList(0, value: false);
				cachedFrames.ResizeList(num, value: false);
				foreach (BoneData sortedBone in parent.sortedBones)
				{
					List<int> list = new List<int>(num);
					int i = 0;
					for (int capacity = list.Capacity; i < capacity; i++)
					{
						list.Add(-1);
					}
					boneCachedFrameIndices[sortedBone.name] = list;
				}
				foreach (SlotData sortedSlot in parent.sortedSlots)
				{
					List<int> list2 = new List<int>(num);
					int j = 0;
					for (int capacity2 = list2.Capacity; j < capacity2; j++)
					{
						list2.Add(-1);
					}
					slotCachedFrameIndices[sortedSlot.name] = list2;
				}
			}
		}

		public void AddBoneTimeline(BoneData bone, TimelineData tiemline)
		{
			if (bone != null && tiemline != null)
			{
				if (!boneTimelines.ContainsKey(bone.name))
				{
					boneTimelines[bone.name] = new List<TimelineData>();
				}
				List<TimelineData> list = boneTimelines[bone.name];
				if (!list.Contains(tiemline))
				{
					list.Add(tiemline);
				}
			}
		}

		public void AddSlotTimeline(SlotData slot, TimelineData timeline)
		{
			if (slot != null && timeline != null)
			{
				if (!slotTimelines.ContainsKey(slot.name))
				{
					slotTimelines[slot.name] = new List<TimelineData>();
				}
				List<TimelineData> list = slotTimelines[slot.name];
				if (!list.Contains(timeline))
				{
					list.Add(timeline);
				}
			}
		}

		public void AddConstraintTimeline(ConstraintData constraint, TimelineData timeline)
		{
			if (constraint != null && timeline != null)
			{
				if (!constraintTimelines.ContainsKey(constraint.name))
				{
					constraintTimelines[constraint.name] = new List<TimelineData>();
				}
				List<TimelineData> list = constraintTimelines[constraint.name];
				if (!list.Contains(timeline))
				{
					list.Add(timeline);
				}
			}
		}

		public List<TimelineData> GetBoneTimelines(string timelineName)
		{
			return (!boneTimelines.ContainsKey(timelineName)) ? null : boneTimelines[timelineName];
		}

		public List<TimelineData> GetSlotTimelines(string timelineName)
		{
			return (!slotTimelines.ContainsKey(timelineName)) ? null : slotTimelines[timelineName];
		}

		public List<TimelineData> GetConstraintTimelines(string timelineName)
		{
			return (!constraintTimelines.ContainsKey(timelineName)) ? null : constraintTimelines[timelineName];
		}

		public List<int> GetBoneCachedFrameIndices(string boneName)
		{
			return (!boneCachedFrameIndices.ContainsKey(boneName)) ? null : boneCachedFrameIndices[boneName];
		}

		public List<int> GetSlotCachedFrameIndices(string slotName)
		{
			return (!slotCachedFrameIndices.ContainsKey(slotName)) ? null : slotCachedFrameIndices[slotName];
		}
	}
}
