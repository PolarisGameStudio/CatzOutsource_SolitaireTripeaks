using System;
using System.Collections.Generic;

namespace DragonBones
{
	public abstract class Slot : TransformObject
	{
		public string displayController;

		protected bool _displayDirty;

		protected bool _zOrderDirty;

		protected bool _visibleDirty;

		protected bool _blendModeDirty;

		internal bool _colorDirty;

		internal bool _transformDirty;

		protected bool _visible;

		internal BlendMode _blendMode;

		protected int _displayIndex;

		protected int _animationDisplayIndex;

		internal int _zOrder;

		protected int _cachedFrameIndex;

		internal float _pivotX;

		internal float _pivotY;

		protected readonly Matrix _localMatrix = new Matrix();

		internal readonly ColorTransform _colorTransform = new ColorTransform();

		internal readonly List<DisplayData> _displayDatas = new List<DisplayData>();

		protected readonly List<object> _displayList = new List<object>();

		internal SlotData _slotData;

		protected List<DisplayData> _rawDisplayDatas;

		protected DisplayData _displayData;

		protected BoundingBoxData _boundingBoxData;

		protected TextureData _textureData;

		public DeformVertices _deformVertices;

		protected object _rawDisplay;

		protected object _meshDisplay;

		protected object _display;

		protected Armature _childArmature;

		protected Bone _parent;

		internal List<int> _cachedFrameIndices = new List<int>();

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
					_UpdateVisible();
				}
			}
		}

		public int displayIndex
		{
			get
			{
				return _displayIndex;
			}
			set
			{
				if (_SetDisplayIndex(value))
				{
					Update(-1);
				}
			}
		}

		public string name => _slotData.name;

		public List<object> displayList
		{
			get
			{
				return new List<object>(_displayList.ToArray());
			}
			set
			{
				object[] array = _displayList.ToArray();
				List<object> list = new List<object>();
				if (_SetDisplayList(value))
				{
					Update(-1);
				}
				object[] array2 = array;
				foreach (object obj in array2)
				{
					if (obj != null && obj != _rawDisplay && obj != _meshDisplay && _displayList.IndexOf(obj) < 0 && list.IndexOf(obj) < 0)
					{
						list.Add(obj);
					}
				}
				foreach (object item in list)
				{
					if (!(item is Armature))
					{
						_DisposeDisplay(item, isRelease: true);
					}
				}
			}
		}

		public SlotData slotData => _slotData;

		public List<DisplayData> rawDisplayDatas
		{
			get
			{
				return _rawDisplayDatas;
			}
			set
			{
				if (_rawDisplayDatas == value)
				{
					return;
				}
				_displayDirty = true;
				_rawDisplayDatas = value;
				if (_rawDisplayDatas != null)
				{
					_displayDatas.ResizeList(_rawDisplayDatas.Count);
					int i = 0;
					for (int count = _displayDatas.Count; i < count; i++)
					{
						DisplayData displayData = _rawDisplayDatas[i];
						if (displayData == null)
						{
							displayData = _GetDefaultRawDisplayData(i);
						}
						_displayDatas[i] = displayData;
					}
				}
				else
				{
					_displayDatas.Clear();
				}
			}
		}

		public BoundingBoxData boundingBoxData => _boundingBoxData;

		public object rawDisplay => _rawDisplay;

		public object meshDisplay => _meshDisplay;

		public object display
		{
			get
			{
				return _display;
			}
			set
			{
				if (_display == value)
				{
					return;
				}
				int count = _displayList.Count;
				if (_displayIndex < 0 && count == 0)
				{
					_displayIndex = 0;
				}
				if (_displayIndex >= 0)
				{
					List<object> displayList = this.displayList;
					if (count <= _displayIndex)
					{
						displayList.ResizeList(_displayIndex + 1);
					}
					displayList[_displayIndex] = value;
					this.displayList = displayList;
				}
			}
		}

		public Armature childArmature
		{
			get
			{
				return _childArmature;
			}
			set
			{
				if (_childArmature != value)
				{
					display = value;
				}
			}
		}

		public Bone parent => _parent;

		public Slot()
		{
		}

		protected override void _OnClear()
		{
			base._OnClear();
			List<object> list = new List<object>();
			int i = 0;
			for (int count = _displayList.Count; i < count; i++)
			{
				object obj = _displayList[i];
				if (obj != _rawDisplay && obj != _meshDisplay && !list.Contains(obj))
				{
					list.Add(obj);
				}
			}
			int j = 0;
			for (int count2 = list.Count; j < count2; j++)
			{
				object obj2 = list[j];
				if (obj2 is Armature)
				{
					(obj2 as Armature).Dispose();
				}
				else
				{
					_DisposeDisplay(obj2, isRelease: true);
				}
			}
			if (_deformVertices != null)
			{
				_deformVertices.ReturnToPool();
			}
			if (_meshDisplay != null && _meshDisplay != _rawDisplay)
			{
				_DisposeDisplay(_meshDisplay, isRelease: false);
			}
			if (_rawDisplay != null)
			{
				_DisposeDisplay(_rawDisplay, isRelease: false);
			}
			displayController = null;
			_displayDirty = false;
			_zOrderDirty = false;
			_blendModeDirty = false;
			_colorDirty = false;
			_transformDirty = false;
			_visible = true;
			_blendMode = BlendMode.Normal;
			_displayIndex = -1;
			_animationDisplayIndex = -1;
			_zOrder = 0;
			_cachedFrameIndex = -1;
			_pivotX = 0f;
			_pivotY = 0f;
			_localMatrix.Identity();
			_colorTransform.Identity();
			_displayList.Clear();
			_displayDatas.Clear();
			_slotData = null;
			_rawDisplayDatas = null;
			_displayData = null;
			_boundingBoxData = null;
			_textureData = null;
			_deformVertices = null;
			_rawDisplay = null;
			_meshDisplay = null;
			_display = null;
			_childArmature = null;
			_parent = null;
			_cachedFrameIndices = null;
		}

		protected abstract void _InitDisplay(object value, bool isRetain);

		protected abstract void _DisposeDisplay(object value, bool isRelease);

		protected abstract void _OnUpdateDisplay();

		protected abstract void _AddDisplay();

		protected abstract void _ReplaceDisplay(object value);

		protected abstract void _RemoveDisplay();

		protected abstract void _UpdateZOrder();

		internal abstract void _UpdateVisible();

		internal abstract void _UpdateBlendMode();

		protected abstract void _UpdateColor();

		protected abstract void _UpdateFrame();

		protected abstract void _UpdateMesh();

		protected abstract void _UpdateTransform();

		protected abstract void _IdentityTransform();

		protected DisplayData _GetDefaultRawDisplayData(int displayIndex)
		{
			SkinData defaultSkin = _armature._armatureData.defaultSkin;
			if (defaultSkin != null)
			{
				List<DisplayData> displays = defaultSkin.GetDisplays(_slotData.name);
				if (displays != null)
				{
					return (displayIndex >= displays.Count) ? null : displays[displayIndex];
				}
			}
			return null;
		}

		protected void _UpdateDisplayData()
		{
			DisplayData displayData = _displayData;
			VerticesData verticesData = (_deformVertices == null) ? null : _deformVertices.verticesData;
			TextureData textureData = _textureData;
			DisplayData displayData2 = null;
			VerticesData verticesData2 = null;
			_displayData = null;
			_boundingBoxData = null;
			_textureData = null;
			if (_displayIndex >= 0)
			{
				if (_rawDisplayDatas != null)
				{
					displayData2 = ((_displayIndex >= _rawDisplayDatas.Count) ? null : _rawDisplayDatas[_displayIndex]);
				}
				if (displayData2 == null)
				{
					displayData2 = _GetDefaultRawDisplayData(_displayIndex);
				}
				if (_displayIndex < _displayDatas.Count)
				{
					_displayData = _displayDatas[_displayIndex];
				}
			}
			if (_displayData != null)
			{
				if (_displayData.type == DisplayType.Mesh)
				{
					verticesData2 = (_displayData as MeshDisplayData).vertices;
				}
				else if (_displayData.type == DisplayType.Path)
				{
					verticesData2 = (_displayData as PathDisplayData).vertices;
				}
				else if (displayData2 != null)
				{
					if (displayData2.type == DisplayType.Mesh)
					{
						verticesData2 = (displayData2 as MeshDisplayData).vertices;
					}
					else if (displayData2.type == DisplayType.Path)
					{
						verticesData2 = (displayData2 as PathDisplayData).vertices;
					}
				}
				if (_displayData.type == DisplayType.BoundingBox)
				{
					_boundingBoxData = (_displayData as BoundingBoxDisplayData).boundingBox;
				}
				else if (displayData2 != null && displayData2.type == DisplayType.BoundingBox)
				{
					_boundingBoxData = (displayData2 as BoundingBoxDisplayData).boundingBox;
				}
				if (_displayData.type == DisplayType.Image)
				{
					_textureData = (_displayData as ImageDisplayData).texture;
				}
				else if (_displayData.type == DisplayType.Mesh)
				{
					_textureData = (_displayData as MeshDisplayData).texture;
				}
			}
			if (_displayData == displayData && verticesData2 == verticesData && _textureData == textureData)
			{
				return;
			}
			if (verticesData2 == null && _textureData != null)
			{
				ImageDisplayData imageDisplayData = _displayData as ImageDisplayData;
				float num = _textureData.parent.scale * _armature._armatureData.scale;
				Rectangle frame = _textureData.frame;
				_pivotX = imageDisplayData.pivot.x;
				_pivotY = imageDisplayData.pivot.y;
				Rectangle rectangle = (frame == null) ? _textureData.region : frame;
				float num2 = rectangle.width;
				float num3 = rectangle.height;
				if (_textureData.rotated && frame == null)
				{
					num2 = rectangle.height;
					num3 = rectangle.width;
				}
				_pivotX *= num2 * num;
				_pivotY *= num3 * num;
				if (frame != null)
				{
					_pivotX += frame.x * num;
					_pivotY += frame.y * num;
				}
				if (_displayData != null && displayData2 != null && _displayData != displayData2)
				{
					displayData2.transform.ToMatrix(TransformObject._helpMatrix);
					TransformObject._helpMatrix.Invert();
					TransformObject._helpMatrix.TransformPoint(0f, 0f, TransformObject._helpPoint);
					_pivotX -= TransformObject._helpPoint.x;
					_pivotY -= TransformObject._helpPoint.y;
					_displayData.transform.ToMatrix(TransformObject._helpMatrix);
					TransformObject._helpMatrix.Invert();
					TransformObject._helpMatrix.TransformPoint(0f, 0f, TransformObject._helpPoint);
					_pivotX += TransformObject._helpPoint.x;
					_pivotY += TransformObject._helpPoint.y;
				}
				if (!DragonBones.yDown)
				{
					_pivotY = ((!_textureData.rotated) ? _textureData.region.height : _textureData.region.width) * num - _pivotY;
				}
			}
			else
			{
				_pivotX = 0f;
				_pivotY = 0f;
			}
			if (displayData2 != null)
			{
				origin = displayData2.transform;
			}
			else if (_displayData != null)
			{
				origin = _displayData.transform;
			}
			else
			{
				origin = null;
			}
			if (verticesData2 != verticesData)
			{
				if (_deformVertices == null)
				{
					_deformVertices = BaseObject.BorrowObject<DeformVertices>();
				}
				_deformVertices.init(verticesData2, _armature);
			}
			else if (_deformVertices != null && _textureData != textureData)
			{
				_deformVertices.verticesDirty = true;
			}
			_displayDirty = true;
			_transformDirty = true;
		}

		protected void _UpdateDisplay()
		{
			object obj = (_display == null) ? _rawDisplay : _display;
			Armature childArmature = _childArmature;
			if (_displayIndex >= 0 && _displayIndex < _displayList.Count)
			{
				_display = _displayList[_displayIndex];
				if (_display != null && _display is Armature)
				{
					_childArmature = (_display as Armature);
					_display = _childArmature.display;
				}
				else
				{
					_childArmature = null;
				}
			}
			else
			{
				_display = null;
				_childArmature = null;
			}
			object obj2 = (_display == null) ? _rawDisplay : _display;
			if (obj2 != obj)
			{
				_OnUpdateDisplay();
				_ReplaceDisplay(obj);
				_transformDirty = true;
				_visibleDirty = true;
				_blendModeDirty = true;
				_colorDirty = true;
			}
			if (obj2 == _rawDisplay || obj2 == _meshDisplay)
			{
				_UpdateFrame();
			}
			if (_childArmature == childArmature)
			{
				return;
			}
			if (childArmature != null)
			{
				childArmature._parent = null;
				childArmature.clock = null;
				if (childArmature.inheritAnimation)
				{
					childArmature.animation.Reset();
				}
			}
			if (_childArmature == null)
			{
				return;
			}
			_childArmature._parent = this;
			_childArmature.clock = _armature.clock;
			if (!_childArmature.inheritAnimation)
			{
				return;
			}
			if (_childArmature.cacheFrameRate == 0)
			{
				uint cacheFrameRate = _armature.cacheFrameRate;
				if (cacheFrameRate != 0)
				{
					_childArmature.cacheFrameRate = cacheFrameRate;
				}
			}
			List<ActionData> list = null;
			if (_displayData != null && _displayData.type == DisplayType.Armature)
			{
				list = (_displayData as ArmatureDisplayData).actions;
			}
			else if (_displayIndex >= 0 && _rawDisplayDatas != null)
			{
				DisplayData displayData = (_displayIndex >= _rawDisplayDatas.Count) ? null : _rawDisplayDatas[_displayIndex];
				if (displayData == null)
				{
					displayData = _GetDefaultRawDisplayData(_displayIndex);
				}
				if (displayData != null && displayData.type == DisplayType.Armature)
				{
					list = (displayData as ArmatureDisplayData).actions;
				}
			}
			if (list != null && list.Count > 0)
			{
				foreach (ActionData item in list)
				{
					EventObject eventObject = BaseObject.BorrowObject<EventObject>();
					EventObject.ActionDataToInstance(item, eventObject, _armature);
					eventObject.slot = this;
					_armature._BufferAction(eventObject, append: false);
				}
			}
			else
			{
				_childArmature.animation.Play();
			}
		}

		protected void _UpdateGlobalTransformMatrix(bool isCache)
		{
			globalTransformMatrix.CopyFrom(_localMatrix);
			globalTransformMatrix.Concat(_parent.globalTransformMatrix);
			if (isCache)
			{
				global.FromMatrix(globalTransformMatrix);
			}
			else
			{
				_globalDirty = true;
			}
		}

		internal bool _SetDisplayIndex(int value, bool isAnimation = false)
		{
			if (isAnimation)
			{
				if (_animationDisplayIndex == value)
				{
					return false;
				}
				_animationDisplayIndex = value;
			}
			if (_displayIndex == value)
			{
				return false;
			}
			_displayIndex = value;
			_displayDirty = true;
			_UpdateDisplayData();
			return _displayDirty;
		}

		internal bool _SetZorder(int value)
		{
			if (_zOrder == value)
			{
			}
			_zOrder = value;
			_zOrderDirty = true;
			return _zOrderDirty;
		}

		internal bool _SetColor(ColorTransform value)
		{
			_colorTransform.CopyFrom(value);
			_colorDirty = true;
			return _colorDirty;
		}

		internal bool _SetDisplayList(List<object> value)
		{
			if (value != null && value.Count > 0)
			{
				if (_displayList.Count != value.Count)
				{
					_displayList.ResizeList(value.Count);
				}
				int i = 0;
				for (int count = value.Count; i < count; i++)
				{
					object obj = value[i];
					if (obj != null && obj != _rawDisplay && obj != _meshDisplay && !(obj is Armature) && _displayList.IndexOf(obj) < 0)
					{
						_InitDisplay(obj, isRetain: true);
					}
					_displayList[i] = obj;
				}
			}
			else if (_displayList.Count > 0)
			{
				_displayList.Clear();
			}
			if (_displayIndex >= 0 && _displayIndex < _displayList.Count)
			{
				_displayDirty = (_display != _displayList[_displayIndex]);
			}
			else
			{
				_displayDirty = (_display != null);
			}
			_UpdateDisplayData();
			return _displayDirty;
		}

		internal virtual void Init(SlotData slotData, Armature armatureValue, object rawDisplay, object meshDisplay)
		{
			if (_slotData == null)
			{
				_slotData = slotData;
				_visibleDirty = true;
				_blendModeDirty = true;
				_colorDirty = true;
				_blendMode = _slotData.blendMode;
				_zOrder = _slotData.zOrder;
				_colorTransform.CopyFrom(_slotData.color);
				_rawDisplay = rawDisplay;
				_meshDisplay = meshDisplay;
				_armature = armatureValue;
				Bone bone = _armature.GetBone(_slotData.parent.name);
				if (bone != null)
				{
					_parent = bone;
				}
				_armature._AddSlot(this);
				_InitDisplay(_rawDisplay, isRetain: false);
				if (_rawDisplay != _meshDisplay)
				{
					_InitDisplay(_meshDisplay, isRetain: false);
				}
				_OnUpdateDisplay();
				_AddDisplay();
			}
		}

		internal void Update(int cacheFrameIndex)
		{
			if (_displayDirty)
			{
				_displayDirty = false;
				_UpdateDisplay();
				if (_transformDirty)
				{
					if (origin != null)
					{
						global.CopyFrom(origin).Add(offset).ToMatrix(_localMatrix);
					}
					else
					{
						global.CopyFrom(offset).ToMatrix(_localMatrix);
					}
				}
			}
			if (_zOrderDirty)
			{
				_zOrderDirty = false;
				_UpdateZOrder();
			}
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
				else if (_transformDirty || _parent._childrenTransformDirty)
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
			else if (_transformDirty || _parent._childrenTransformDirty)
			{
				cacheFrameIndex = -1;
				_transformDirty = true;
				_cachedFrameIndex = -1;
			}
			if (_display == null)
			{
				return;
			}
			if (_visibleDirty)
			{
				_visibleDirty = false;
				_UpdateVisible();
			}
			if (_blendModeDirty)
			{
				_blendModeDirty = false;
				_UpdateBlendMode();
			}
			if (_colorDirty)
			{
				_colorDirty = false;
				_UpdateColor();
			}
			if (_deformVertices != null && _deformVertices.verticesData != null && _display == _meshDisplay)
			{
				bool flag = _deformVertices.verticesData.weight != null;
				if (_deformVertices.verticesDirty || (flag && _deformVertices.isBonesUpdate()))
				{
					_deformVertices.verticesDirty = false;
					_UpdateMesh();
				}
				if (flag)
				{
					return;
				}
			}
			if (!_transformDirty)
			{
				return;
			}
			_transformDirty = false;
			if (_cachedFrameIndex < 0)
			{
				bool flag2 = cacheFrameIndex >= 0;
				_UpdateGlobalTransformMatrix(flag2);
				if (flag2 && _cachedFrameIndices != null)
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
			_UpdateTransform();
		}

		public void UpdateTransformAndMatrix()
		{
			if (_transformDirty)
			{
				_transformDirty = false;
				_UpdateGlobalTransformMatrix(isCache: false);
			}
		}

		internal void ReplaceDisplayData(DisplayData value, int displayIndex = -1)
		{
			if (displayIndex < 0)
			{
				displayIndex = ((_displayIndex >= 0) ? _displayIndex : 0);
			}
			if (_displayDatas.Count <= displayIndex)
			{
				_displayDatas.ResizeList(displayIndex + 1);
				int i = 0;
				for (int count = _displayDatas.Count; i < count; i++)
				{
					_displayDatas[i] = null;
				}
			}
			_displayDatas[displayIndex] = value;
		}

		public bool ContainsPoint(float x, float y)
		{
			if (_boundingBoxData == null)
			{
				return false;
			}
			UpdateTransformAndMatrix();
			TransformObject._helpMatrix.CopyFrom(globalTransformMatrix);
			TransformObject._helpMatrix.Invert();
			TransformObject._helpMatrix.TransformPoint(x, y, TransformObject._helpPoint);
			return _boundingBoxData.ContainsPoint(TransformObject._helpPoint.x, TransformObject._helpPoint.y);
		}

		public int IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			if (_boundingBoxData == null)
			{
				return 0;
			}
			UpdateTransformAndMatrix();
			TransformObject._helpMatrix.CopyFrom(globalTransformMatrix);
			TransformObject._helpMatrix.Invert();
			TransformObject._helpMatrix.TransformPoint(xA, yA, TransformObject._helpPoint);
			xA = TransformObject._helpPoint.x;
			yA = TransformObject._helpPoint.y;
			TransformObject._helpMatrix.TransformPoint(xB, yB, TransformObject._helpPoint);
			xB = TransformObject._helpPoint.x;
			yB = TransformObject._helpPoint.y;
			int num = _boundingBoxData.IntersectsSegment(xA, yA, xB, yB, intersectionPointA, intersectionPointB, normalRadians);
			if (num > 0)
			{
				if (num == 1 || num == 2)
				{
					if (intersectionPointA != null)
					{
						globalTransformMatrix.TransformPoint(intersectionPointA.x, intersectionPointA.y, intersectionPointA);
						if (intersectionPointB != null)
						{
							intersectionPointB.x = intersectionPointA.x;
							intersectionPointB.y = intersectionPointA.y;
						}
					}
					else if (intersectionPointB != null)
					{
						globalTransformMatrix.TransformPoint(intersectionPointB.x, intersectionPointB.y, intersectionPointB);
					}
				}
				else
				{
					if (intersectionPointA != null)
					{
						globalTransformMatrix.TransformPoint(intersectionPointA.x, intersectionPointA.y, intersectionPointA);
					}
					if (intersectionPointB != null)
					{
						globalTransformMatrix.TransformPoint(intersectionPointB.x, intersectionPointB.y, intersectionPointB);
					}
				}
				if (normalRadians != null)
				{
					globalTransformMatrix.TransformPoint((float)Math.Cos(normalRadians.x), (float)Math.Sin(normalRadians.x), TransformObject._helpPoint, delta: true);
					normalRadians.x = (float)Math.Atan2(TransformObject._helpPoint.y, TransformObject._helpPoint.x);
					globalTransformMatrix.TransformPoint((float)Math.Cos(normalRadians.y), (float)Math.Sin(normalRadians.y), TransformObject._helpPoint, delta: true);
					normalRadians.y = (float)Math.Atan2(TransformObject._helpPoint.y, TransformObject._helpPoint.x);
				}
			}
			return num;
		}

		public void InvalidUpdate()
		{
			_displayDirty = true;
			_transformDirty = true;
		}
	}
}
