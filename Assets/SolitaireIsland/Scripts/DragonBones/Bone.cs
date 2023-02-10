using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class Bone : TransformObject
	{
		internal OffsetMode offsetMode;

		internal readonly Transform animationPose = new Transform();

		internal bool _transformDirty;

		internal bool _childrenTransformDirty;

		private bool _localDirty;

		internal bool _hasConstraint;

		private bool _visible;

		private int _cachedFrameIndex;

		internal readonly BlendState _blendState = new BlendState();

		internal BoneData _boneData;

		protected Bone _parent;

		internal List<int> _cachedFrameIndices = new List<int>();

		public BoneData boneData => _boneData;

		public bool visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if (_visible != value)
				{
					_visible = value;
					foreach (Slot slot in _armature.GetSlots())
					{
						if (slot.parent == this)
						{
							slot._UpdateVisible();
						}
					}
				}
			}
		}

		public string name => _boneData.name;

		public Bone parent => _parent;

		[Obsolete("")]
		public Slot slot
		{
			get
			{
				foreach (Slot slot in _armature.GetSlots())
				{
					if (slot.parent == this)
					{
						return slot;
					}
				}
				return null;
			}
		}

		protected override void _OnClear()
		{
			base._OnClear();
			offsetMode = OffsetMode.Additive;
			animationPose.Identity();
			_transformDirty = false;
			_childrenTransformDirty = false;
			_localDirty = true;
			_hasConstraint = false;
			_visible = true;
			_cachedFrameIndex = -1;
			_blendState.Clear();
			_boneData = null;
			_parent = null;
			_cachedFrameIndices = null;
		}

		private void _UpdateGlobalTransformMatrix(bool isCache)
		{
			BoneData boneData = _boneData;
			Bone parent = _parent;
			bool flipX = _armature.flipX;
			bool flag = _armature.flipY == DragonBones.yDown;
			float num = 0f;
			Transform global = base.global;
			bool flag2 = parent != null;
			Matrix globalTransformMatrix = base.globalTransformMatrix;
			if (offsetMode == OffsetMode.Additive)
			{
				if (origin != null)
				{
					global.x = origin.x + offset.x + animationPose.x;
					global.y = origin.y + offset.y + animationPose.y;
					global.skew = origin.skew + offset.skew + animationPose.skew;
					global.rotation = origin.rotation + offset.rotation + animationPose.rotation;
					global.scaleX = origin.scaleX * offset.scaleX * animationPose.scaleX;
					global.scaleY = origin.scaleY * offset.scaleY * animationPose.scaleY;
				}
				else
				{
					global.CopyFrom(offset).Add(animationPose);
				}
			}
			else if (offsetMode == OffsetMode.None)
			{
				if (origin != null)
				{
					global.CopyFrom(origin).Add(animationPose);
				}
				else
				{
					global.CopyFrom(animationPose);
				}
			}
			else
			{
				flag2 = false;
				global.CopyFrom(offset);
			}
			if (flag2)
			{
				Matrix globalTransformMatrix2 = parent.globalTransformMatrix;
				if (boneData.inheritScale)
				{
					if (!boneData.inheritRotation)
					{
						parent.UpdateGlobalTransform();
						num = (global.rotation = ((flipX && flag) ? (global.rotation - (parent.global.rotation + Transform.PI)) : (flipX ? (global.rotation + parent.global.rotation + Transform.PI) : ((!flag) ? (global.rotation - parent.global.rotation) : (global.rotation + parent.global.rotation)))));
					}
					global.ToMatrix(globalTransformMatrix);
					globalTransformMatrix.Concat(globalTransformMatrix2);
					if (_boneData.inheritTranslation)
					{
						global.x = globalTransformMatrix.tx;
						global.y = globalTransformMatrix.ty;
					}
					else
					{
						globalTransformMatrix.tx = global.x;
						globalTransformMatrix.ty = global.y;
					}
					if (isCache)
					{
						global.FromMatrix(globalTransformMatrix);
					}
					else
					{
						_globalDirty = true;
					}
					return;
				}
				if (boneData.inheritTranslation)
				{
					float x = global.x;
					float y = global.y;
					global.x = globalTransformMatrix2.a * x + globalTransformMatrix2.c * y + globalTransformMatrix2.tx;
					global.y = globalTransformMatrix2.b * x + globalTransformMatrix2.d * y + globalTransformMatrix2.ty;
				}
				else
				{
					if (flipX)
					{
						global.x = 0f - global.x;
					}
					if (flag)
					{
						global.y = 0f - global.y;
					}
				}
				if (boneData.inheritRotation)
				{
					parent.UpdateGlobalTransform();
					num = ((!((double)parent.global.scaleX < 0.0)) ? (global.rotation + parent.global.rotation) : (global.rotation + parent.global.rotation + Transform.PI));
					if ((double)(globalTransformMatrix2.a * globalTransformMatrix2.d - globalTransformMatrix2.b * globalTransformMatrix2.c) < 0.0)
					{
						num -= global.rotation * 2f;
						if (flipX != flag || boneData.inheritReflection)
						{
							global.skew += Transform.PI;
						}
					}
					global.rotation = num;
				}
				else if (flipX || flag)
				{
					if (flipX && flag)
					{
						num = global.rotation + Transform.PI;
					}
					else
					{
						num = ((!flipX) ? (0f - global.rotation) : (Transform.PI - global.rotation));
						global.skew += Transform.PI;
					}
					global.rotation = num;
				}
				global.ToMatrix(globalTransformMatrix);
				return;
			}
			if (flipX || flag)
			{
				if (flipX)
				{
					global.x = 0f - global.x;
				}
				if (flag)
				{
					global.y = 0f - global.y;
				}
				if (flipX && flag)
				{
					num = global.rotation + Transform.PI;
				}
				else
				{
					num = ((!flipX) ? (0f - global.rotation) : (Transform.PI - global.rotation));
					global.skew += Transform.PI;
				}
				global.rotation = num;
			}
			global.ToMatrix(globalTransformMatrix);
		}

		internal void Init(BoneData boneData, Armature armatureValue)
		{
			if (_boneData == null)
			{
				_boneData = boneData;
				_armature = armatureValue;
				if (_boneData.parent != null)
				{
					_parent = _armature.GetBone(_boneData.parent.name);
				}
				_armature._AddBone(this);
				origin = _boneData.transform;
			}
		}

		internal void Update(int cacheFrameIndex)
		{
			_blendState.dirty = false;
			if (cacheFrameIndex >= 0 && _cachedFrameIndices != null)
			{
				int num = _cachedFrameIndices[cacheFrameIndex];
				if (num >= 0 && _cachedFrameIndex == num)
				{
					_transformDirty = false;
				}
				else if (num >= 0)
				{
					_transformDirty = true;
					_cachedFrameIndex = num;
				}
				else
				{
					if (_hasConstraint)
					{
						foreach (Constraint constraint in _armature._constraints)
						{
							if (constraint._root == this)
							{
								constraint.Update();
							}
						}
					}
					if (_transformDirty || (_parent != null && _parent._childrenTransformDirty))
					{
						_transformDirty = true;
						_cachedFrameIndex = -1;
					}
					else if (_cachedFrameIndex >= 0)
					{
						_transformDirty = false;
						_cachedFrameIndices[cacheFrameIndex] = _cachedFrameIndex;
					}
					else
					{
						_transformDirty = true;
						_cachedFrameIndex = -1;
					}
				}
			}
			else
			{
				if (_hasConstraint)
				{
					foreach (Constraint constraint2 in _armature._constraints)
					{
						if (constraint2._root == this)
						{
							constraint2.Update();
						}
					}
				}
				if (_transformDirty || (_parent != null && _parent._childrenTransformDirty))
				{
					cacheFrameIndex = -1;
					_transformDirty = true;
					_cachedFrameIndex = -1;
				}
			}
			if (_transformDirty)
			{
				_transformDirty = false;
				_childrenTransformDirty = true;
				if (_cachedFrameIndex < 0)
				{
					bool flag = cacheFrameIndex >= 0;
					if (_localDirty)
					{
						_UpdateGlobalTransformMatrix(flag);
					}
					if (flag && _cachedFrameIndices != null)
					{
						int num2 = _armature._armatureData.SetCacheFrame(globalTransformMatrix, global);
						_cachedFrameIndices[cacheFrameIndex] = num2;
						_cachedFrameIndex = num2;
					}
				}
				else
				{
					_armature._armatureData.GetCacheFrame(globalTransformMatrix, global, _cachedFrameIndex);
				}
			}
			else if (_childrenTransformDirty)
			{
				_childrenTransformDirty = false;
			}
			_localDirty = true;
		}

		internal void UpdateByConstraint()
		{
			if (_localDirty)
			{
				_localDirty = false;
				if (_transformDirty || (_parent != null && _parent._childrenTransformDirty))
				{
					_UpdateGlobalTransformMatrix(isCache: true);
				}
				_transformDirty = true;
			}
		}

		public void InvalidUpdate()
		{
			_transformDirty = true;
		}

		public bool Contains(Bone value)
		{
			if (value == this)
			{
				return false;
			}
			Bone bone = value;
			while (bone != this && bone != null)
			{
				bone = bone.parent;
			}
			return bone == this;
		}
	}
}
