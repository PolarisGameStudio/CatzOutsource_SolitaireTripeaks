using System.Collections.Generic;

namespace DragonBones
{
	public class ArmatureData : BaseObject
	{
		public ArmatureType type;

		public uint frameRate;

		public uint cacheFrameRate;

		public float scale;

		public string name;

		public readonly Rectangle aabb = new Rectangle();

		public readonly List<string> animationNames = new List<string>();

		public readonly List<BoneData> sortedBones = new List<BoneData>();

		public readonly List<SlotData> sortedSlots = new List<SlotData>();

		public readonly List<ActionData> defaultActions = new List<ActionData>();

		public readonly List<ActionData> actions = new List<ActionData>();

		public readonly Dictionary<string, BoneData> bones = new Dictionary<string, BoneData>();

		public readonly Dictionary<string, SlotData> slots = new Dictionary<string, SlotData>();

		public readonly Dictionary<string, ConstraintData> constraints = new Dictionary<string, ConstraintData>();

		public readonly Dictionary<string, SkinData> skins = new Dictionary<string, SkinData>();

		public readonly Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();

		public SkinData defaultSkin;

		public AnimationData defaultAnimation;

		public CanvasData canvas;

		public UserData userData;

		public DragonBonesData parent;

		protected override void _OnClear()
		{
			foreach (ActionData defaultAction in defaultActions)
			{
				defaultAction.ReturnToPool();
			}
			foreach (ActionData action in actions)
			{
				action.ReturnToPool();
			}
			foreach (string key in bones.Keys)
			{
				bones[key].ReturnToPool();
			}
			foreach (string key2 in slots.Keys)
			{
				slots[key2].ReturnToPool();
			}
			foreach (string key3 in constraints.Keys)
			{
				constraints[key3].ReturnToPool();
			}
			foreach (string key4 in skins.Keys)
			{
				skins[key4].ReturnToPool();
			}
			foreach (string key5 in animations.Keys)
			{
				animations[key5].ReturnToPool();
			}
			if (canvas != null)
			{
				canvas.ReturnToPool();
			}
			if (userData != null)
			{
				userData.ReturnToPool();
			}
			type = ArmatureType.Armature;
			frameRate = 0u;
			cacheFrameRate = 0u;
			scale = 1f;
			name = string.Empty;
			aabb.Clear();
			animationNames.Clear();
			sortedBones.Clear();
			sortedSlots.Clear();
			defaultActions.Clear();
			actions.Clear();
			bones.Clear();
			slots.Clear();
			constraints.Clear();
			skins.Clear();
			animations.Clear();
			defaultSkin = null;
			defaultAnimation = null;
			canvas = null;
			userData = null;
			parent = null;
		}

		public void SortBones()
		{
			int count = sortedBones.Count;
			if (count <= 0)
			{
				return;
			}
			BoneData[] array = sortedBones.ToArray();
			int num = 0;
			int num2 = 0;
			sortedBones.Clear();
			while (num2 < count)
			{
				BoneData boneData = array[num++];
				if (num >= count)
				{
					num = 0;
				}
				if (!sortedBones.Contains(boneData))
				{
					bool flag = false;
					foreach (ConstraintData value in constraints.Values)
					{
						if (value.root == boneData && !sortedBones.Contains(value.target))
						{
							flag = true;
							break;
						}
					}
					if (!flag && (boneData.parent == null || sortedBones.Contains(boneData.parent)))
					{
						sortedBones.Add(boneData);
						num2++;
					}
				}
			}
		}

		public void CacheFrames(uint frameRate)
		{
			if (cacheFrameRate == 0)
			{
				cacheFrameRate = frameRate;
				foreach (string key in animations.Keys)
				{
					animations[key].CacheFrames((float)(double)cacheFrameRate);
				}
			}
		}

		public int SetCacheFrame(Matrix globalTransformMatrix, Transform transform)
		{
			List<float> cachedFrames = parent.cachedFrames;
			int count = cachedFrames.Count;
			cachedFrames.ResizeList(count + 10, 0f);
			cachedFrames[count] = globalTransformMatrix.a;
			cachedFrames[count + 1] = globalTransformMatrix.b;
			cachedFrames[count + 2] = globalTransformMatrix.c;
			cachedFrames[count + 3] = globalTransformMatrix.d;
			cachedFrames[count + 4] = globalTransformMatrix.tx;
			cachedFrames[count + 5] = globalTransformMatrix.ty;
			cachedFrames[count + 6] = transform.rotation;
			cachedFrames[count + 7] = transform.skew;
			cachedFrames[count + 8] = transform.scaleX;
			cachedFrames[count + 9] = transform.scaleY;
			return count;
		}

		public void GetCacheFrame(Matrix globalTransformMatrix, Transform transform, int arrayOffset)
		{
			List<float> cachedFrames = parent.cachedFrames;
			globalTransformMatrix.a = cachedFrames[arrayOffset];
			globalTransformMatrix.b = cachedFrames[arrayOffset + 1];
			globalTransformMatrix.c = cachedFrames[arrayOffset + 2];
			globalTransformMatrix.d = cachedFrames[arrayOffset + 3];
			globalTransformMatrix.tx = cachedFrames[arrayOffset + 4];
			globalTransformMatrix.ty = cachedFrames[arrayOffset + 5];
			transform.rotation = cachedFrames[arrayOffset + 6];
			transform.skew = cachedFrames[arrayOffset + 7];
			transform.scaleX = cachedFrames[arrayOffset + 8];
			transform.scaleY = cachedFrames[arrayOffset + 9];
			transform.x = globalTransformMatrix.tx;
			transform.y = globalTransformMatrix.ty;
		}

		public void AddBone(BoneData value)
		{
			if (value != null && !string.IsNullOrEmpty(value.name))
			{
				if (bones.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same bone: " + value.name);
					bones[value.name].ReturnToPool();
				}
				bones[value.name] = value;
				sortedBones.Add(value);
			}
		}

		public void AddSlot(SlotData value)
		{
			if (value != null && !string.IsNullOrEmpty(value.name))
			{
				if (slots.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same slot: " + value.name);
					slots[value.name].ReturnToPool();
				}
				slots[value.name] = value;
				sortedSlots.Add(value);
			}
		}

		public void AddConstraint(ConstraintData value)
		{
			if (value != null && !string.IsNullOrEmpty(value.name))
			{
				if (constraints.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same constraint: " + value.name);
					slots[value.name].ReturnToPool();
				}
				constraints[value.name] = value;
			}
		}

		public void AddSkin(SkinData value)
		{
			if (value != null && !string.IsNullOrEmpty(value.name))
			{
				if (skins.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same slot: " + value.name);
					skins[value.name].ReturnToPool();
				}
				value.parent = this;
				skins[value.name] = value;
				if (defaultSkin == null)
				{
					defaultSkin = value;
				}
				if (value.name == "default")
				{
					defaultSkin = value;
				}
			}
		}

		public void AddAnimation(AnimationData value)
		{
			if (value != null && !string.IsNullOrEmpty(value.name))
			{
				if (animations.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same animation: " + value.name);
					animations[value.name].ReturnToPool();
				}
				value.parent = this;
				animations[value.name] = value;
				animationNames.Add(value.name);
				if (defaultAnimation == null)
				{
					defaultAnimation = value;
				}
			}
		}

		internal void AddAction(ActionData value, bool isDefault)
		{
			if (isDefault)
			{
				defaultActions.Add(value);
			}
			else
			{
				actions.Add(value);
			}
		}

		public BoneData GetBone(string boneName)
		{
			return (string.IsNullOrEmpty(boneName) || !bones.ContainsKey(boneName)) ? null : bones[boneName];
		}

		public SlotData GetSlot(string slotName)
		{
			return (string.IsNullOrEmpty(slotName) || !slots.ContainsKey(slotName)) ? null : slots[slotName];
		}

		public ConstraintData GetConstraint(string constraintName)
		{
			return (!constraints.ContainsKey(constraintName)) ? null : constraints[constraintName];
		}

		public SkinData GetSkin(string skinName)
		{
			return string.IsNullOrEmpty(skinName) ? defaultSkin : ((!skins.ContainsKey(skinName)) ? null : skins[skinName]);
		}

		public MeshDisplayData GetMesh(string skinName, string slotName, string meshName)
		{
			SkinData skin = GetSkin(skinName);
			if (skin == null)
			{
				return null;
			}
			return skin.GetDisplay(slotName, meshName) as MeshDisplayData;
		}

		public AnimationData GetAnimation(string animationName)
		{
			return string.IsNullOrEmpty(animationName) ? defaultAnimation : ((!animations.ContainsKey(animationName)) ? null : animations[animationName]);
		}
	}
}
