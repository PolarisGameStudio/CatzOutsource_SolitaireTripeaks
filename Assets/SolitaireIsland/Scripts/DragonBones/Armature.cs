using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DragonBones
{
	public class Armature : BaseObject, IAnimatable
	{
		public bool inheritAnimation;

		public object userData;

		private bool _lockUpdate;

		private bool _slotsDirty;

		private bool _zOrderDirty;

		private bool _flipX;

		private bool _flipY;

		internal int _cacheFrameIndex;

		private readonly List<Bone> _bones = new List<Bone>();

		private readonly List<Slot> _slots = new List<Slot>();

		internal readonly List<Constraint> _constraints = new List<Constraint>();

		private readonly List<EventObject> _actions = new List<EventObject>();

		public ArmatureData _armatureData;

		private Animation _animation;

		private IArmatureProxy _proxy;

		private object _display;

		internal TextureAtlasData _replaceTextureAtlasData;

		private object _replacedTexture;

		internal DragonBones _dragonBones;

		private WorldClock _clock;

		internal Slot _parent;

		[CompilerGenerated]
		private static Comparison<Slot> _003C_003Ef__mg_0024cache0;

		public bool flipX
		{
			get
			{
				return _flipX;
			}
			set
			{
				if (_flipX != value)
				{
					_flipX = value;
					InvalidUpdate();
				}
			}
		}

		public bool flipY
		{
			get
			{
				return _flipY;
			}
			set
			{
				if (_flipY != value)
				{
					_flipY = value;
					InvalidUpdate();
				}
			}
		}

		public uint cacheFrameRate
		{
			get
			{
				return _armatureData.cacheFrameRate;
			}
			set
			{
				if (_armatureData.cacheFrameRate != value)
				{
					_armatureData.CacheFrames(value);
					foreach (Slot slot in _slots)
					{
						Armature childArmature = slot.childArmature;
						if (childArmature != null)
						{
							childArmature.cacheFrameRate = value;
						}
					}
				}
			}
		}

		public string name => _armatureData.name;

		public ArmatureData armatureData => _armatureData;

		public Animation animation => _animation;

		public IArmatureProxy proxy => _proxy;

		public IEventDispatcher<EventObject> eventDispatcher => _proxy;

		public object display => _display;

		public object replacedTexture
		{
			get
			{
				return _replacedTexture;
			}
			set
			{
				if (_replacedTexture != value)
				{
					if (_replaceTextureAtlasData != null)
					{
						_replaceTextureAtlasData.ReturnToPool();
						_replaceTextureAtlasData = null;
					}
					_replacedTexture = value;
					foreach (Slot slot in _slots)
					{
						slot.InvalidUpdate();
						slot.Update(-1);
					}
				}
			}
		}

		public WorldClock clock
		{
			get
			{
				return _clock;
			}
			set
			{
				if (_clock != value)
				{
					if (_clock != null)
					{
						_clock.Remove(this);
					}
					_clock = value;
					if (_clock != null)
					{
						_clock.Add(this);
					}
					foreach (Slot slot in _slots)
					{
						Armature childArmature = slot.childArmature;
						if (childArmature != null)
						{
							childArmature.clock = _clock;
						}
					}
				}
			}
		}

		public Slot parent => _parent;

		private static int _OnSortSlots(Slot a, Slot b)
		{
			if (a._zOrder > b._zOrder)
			{
				return 1;
			}
			if (a._zOrder < b._zOrder)
			{
				return -1;
			}
			return 0;
		}

		protected override void _OnClear()
		{
			if (_clock != null)
			{
				_clock.Remove(this);
			}
			foreach (Bone bone in _bones)
			{
				bone.ReturnToPool();
			}
			foreach (Slot slot in _slots)
			{
				slot.ReturnToPool();
			}
			foreach (Constraint constraint in _constraints)
			{
				constraint.ReturnToPool();
			}
			if (_animation != null)
			{
				_animation.ReturnToPool();
			}
			if (_proxy != null)
			{
				_proxy.DBClear();
			}
			if (_replaceTextureAtlasData != null)
			{
				_replaceTextureAtlasData.ReturnToPool();
			}
			inheritAnimation = true;
			userData = null;
			_lockUpdate = false;
			_slotsDirty = false;
			_zOrderDirty = false;
			_flipX = false;
			_flipY = false;
			_cacheFrameIndex = -1;
			_bones.Clear();
			_slots.Clear();
			_constraints.Clear();
			_actions.Clear();
			_armatureData = null;
			_animation = null;
			_proxy = null;
			_display = null;
			_replaceTextureAtlasData = null;
			_replacedTexture = null;
			_dragonBones = null;
			_clock = null;
			_parent = null;
		}

		internal void _SortZOrder(short[] slotIndices, int offset)
		{
			List<SlotData> sortedSlots = _armatureData.sortedSlots;
			bool flag = slotIndices == null;
			if (!_zOrderDirty && flag)
			{
				return;
			}
			int i = 0;
			for (int count = sortedSlots.Count; i < count; i++)
			{
				int num = (!flag) ? slotIndices[offset + i] : i;
				if (num >= 0 && num < count)
				{
					SlotData slotData = sortedSlots[num];
					GetSlot(slotData.name)?._SetZorder(i);
				}
			}
			_slotsDirty = true;
			_zOrderDirty = !flag;
		}

		internal void _AddBone(Bone value)
		{
			if (!_bones.Contains(value))
			{
				_bones.Add(value);
			}
		}

		internal void _AddSlot(Slot value)
		{
			if (!_slots.Contains(value))
			{
				_slotsDirty = true;
				_slots.Add(value);
			}
		}

		internal void _AddConstraint(Constraint value)
		{
			if (!_constraints.Contains(value))
			{
				_constraints.Add(value);
			}
		}

		internal void _BufferAction(EventObject action, bool append)
		{
			if (!_actions.Contains(action))
			{
				if (append)
				{
					_actions.Add(action);
				}
				else
				{
					_actions.Insert(0, action);
				}
			}
		}

		public void Dispose()
		{
			if (_armatureData != null)
			{
				_lockUpdate = true;
				if (_dragonBones != null)
				{
					_dragonBones.BufferObject(this);
				}
			}
		}

		internal void Init(ArmatureData armatureData, IArmatureProxy proxy, object display, DragonBones dragonBones)
		{
			if (_armatureData == null)
			{
				_armatureData = armatureData;
				_animation = BaseObject.BorrowObject<Animation>();
				_proxy = proxy;
				_display = display;
				_dragonBones = dragonBones;
				_proxy.DBInit(this);
				_animation.Init(this);
				_animation.animations = _armatureData.animations;
			}
		}

		public void AdvanceTime(float passedTime)
		{
			if (_lockUpdate)
			{
				return;
			}
			if (_armatureData == null)
			{
				Helper.Assert(condition: false, "The armature has been disposed.");
				return;
			}
			if (_armatureData.parent == null)
			{
				Helper.Assert(condition: false, "The armature data has been disposed.\nPlease make sure dispose armature before call factory.clear().");
				return;
			}
			int cacheFrameIndex = _cacheFrameIndex;
			_animation.AdvanceTime(passedTime);
			if (_slotsDirty)
			{
				_slotsDirty = false;
				_slots.Sort(_OnSortSlots);
			}
			if (_cacheFrameIndex < 0 || _cacheFrameIndex != cacheFrameIndex)
			{
				int num = 0;
				int num2 = 0;
				num = 0;
				for (num2 = _bones.Count; num < num2; num++)
				{
					_bones[num].Update(_cacheFrameIndex);
				}
				num = 0;
				for (num2 = _slots.Count; num < num2; num++)
				{
					_slots[num].Update(_cacheFrameIndex);
				}
			}
			if (_actions.Count > 0)
			{
				_lockUpdate = true;
				foreach (EventObject action in _actions)
				{
					ActionData actionData = action.actionData;
					if (actionData != null && actionData.type == ActionType.Play)
					{
						if (action.slot != null)
						{
							action.slot.childArmature?.animation.FadeIn(actionData.name);
						}
						else if (action.bone != null)
						{
							foreach (Slot slot in GetSlots())
							{
								if (slot.parent == action.bone)
								{
									slot.childArmature?.animation.FadeIn(actionData.name);
								}
							}
						}
						else
						{
							_animation.FadeIn(actionData.name);
						}
					}
					action.ReturnToPool();
				}
				_actions.Clear();
				_lockUpdate = false;
			}
			_proxy.DBUpdate();
		}

		public void InvalidUpdate(string boneName = null, bool updateSlot = false)
		{
			if (!string.IsNullOrEmpty(boneName))
			{
				Bone bone = GetBone(boneName);
				if (bone != null)
				{
					bone.InvalidUpdate();
					if (updateSlot)
					{
						foreach (Slot slot in _slots)
						{
							if (slot.parent == bone)
							{
								slot.InvalidUpdate();
							}
						}
					}
				}
			}
			else
			{
				foreach (Bone bone2 in _bones)
				{
					bone2.InvalidUpdate();
				}
				if (updateSlot)
				{
					foreach (Slot slot2 in _slots)
					{
						slot2.InvalidUpdate();
					}
				}
			}
		}

		public Slot ContainsPoint(float x, float y)
		{
			foreach (Slot slot in _slots)
			{
				if (slot.ContainsPoint(x, y))
				{
					return slot;
				}
			}
			return null;
		}

		public Slot IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			bool flag = xA == xB;
			float num = 0f;
			float num2 = 0f;
			float x = 0f;
			float y = 0f;
			float x2 = 0f;
			float y2 = 0f;
			float x3 = 0f;
			float y3 = 0f;
			Slot slot = null;
			Slot slot2 = null;
			foreach (Slot slot3 in _slots)
			{
				int num3 = slot3.IntersectsSegment(xA, yA, xB, yB, intersectionPointA, intersectionPointB, normalRadians);
				if (num3 > 0)
				{
					if (intersectionPointA == null && intersectionPointB == null)
					{
						slot = slot3;
						break;
					}
					if (intersectionPointA != null)
					{
						float num4 = (!flag) ? (intersectionPointA.x - xA) : (intersectionPointA.y - yA);
						if (num4 < 0f)
						{
							num4 = 0f - num4;
						}
						if (slot == null || num4 < num)
						{
							num = num4;
							x = intersectionPointA.x;
							y = intersectionPointA.y;
							slot = slot3;
							if (normalRadians != null)
							{
								x3 = normalRadians.x;
							}
						}
					}
					if (intersectionPointB != null)
					{
						float num5 = intersectionPointB.x - xA;
						if (num5 < 0f)
						{
							num5 = 0f - num5;
						}
						if (slot2 == null || num5 > num2)
						{
							num2 = num5;
							x2 = intersectionPointB.x;
							y2 = intersectionPointB.y;
							slot2 = slot3;
							if (normalRadians != null)
							{
								y3 = normalRadians.y;
							}
						}
					}
				}
			}
			if (slot != null && intersectionPointA != null)
			{
				intersectionPointA.x = x;
				intersectionPointA.y = y;
				if (normalRadians != null)
				{
					normalRadians.x = x3;
				}
			}
			if (slot2 != null && intersectionPointB != null)
			{
				intersectionPointB.x = x2;
				intersectionPointB.y = y2;
				if (normalRadians != null)
				{
					normalRadians.y = y3;
				}
			}
			return slot;
		}

		public Bone GetBone(string name)
		{
			foreach (Bone bone in _bones)
			{
				if (bone.name == name)
				{
					return bone;
				}
			}
			return null;
		}

		public Bone GetBoneByDisplay(object display)
		{
			return GetSlotByDisplay(display)?.parent;
		}

		public Slot GetSlot(string name)
		{
			foreach (Slot slot in _slots)
			{
				if (slot.name == name)
				{
					return slot;
				}
			}
			return null;
		}

		public Slot GetSlotByDisplay(object display)
		{
			if (display != null)
			{
				foreach (Slot slot in _slots)
				{
					if (slot.display == display)
					{
						return slot;
					}
				}
			}
			return null;
		}

		public List<Bone> GetBones()
		{
			return _bones;
		}

		public List<Slot> GetSlots()
		{
			return _slots;
		}
	}
}
