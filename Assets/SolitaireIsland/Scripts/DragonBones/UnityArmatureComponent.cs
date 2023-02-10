using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DragonBones
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class UnityArmatureComponent : DragonBoneEventDispatcher, IArmatureProxy, IEventDispatcher<EventObject>
	{
		public const int ORDER_SPACE = 10;

		public UnityDragonBonesData unityData;

		public string armatureName;

		public bool isUGUI;

		public bool debugDraw;

		internal readonly ColorTransform _colorTransform = new ColorTransform();

		public string animationName;

		private bool _disposeProxy = true;

		internal Armature _armature;

		[Tooltip("0 : Loop")]
		[Range(0f, 100f)]
		[SerializeField]
		protected int _playTimes;

		[Range(-2f, 2f)]
		[SerializeField]
		protected float _timeScale = 1f;

		[SerializeField]
		internal SortingMode _sortingMode;

		[SerializeField]
		internal string _sortingLayerName = "Default";

		[SerializeField]
		internal int _sortingOrder;

		[SerializeField]
		internal float _zSpace;

		[SerializeField]
		protected bool _flipX;

		[SerializeField]
		protected bool _flipY;

		[SerializeField]
		protected bool _closeCombineMeshs;

		private bool _hasSortingGroup;

		private Material _debugDrawer;

		internal int _armatureZ;

		internal SortingGroup _sortingGroup;

		public Armature armature => _armature;

		public Animation animation => (_armature == null) ? null : _armature.animation;

		public SortingMode sortingMode
		{
			get
			{
				return _sortingMode;
			}
			set
			{
				if (_sortingMode == value)
				{
					return;
				}
				if (false)
				{
					LogHelper.LogWarning("SortingMode.SortByOrder is userd by Unity 5.6 or highter only.");
					return;
				}
				_sortingMode = value;
				if (_sortingMode == SortingMode.SortByOrder)
				{
					_sortingGroup = GetComponent<SortingGroup>();
					if (_sortingGroup == null)
					{
						_sortingGroup = base.gameObject.AddComponent<SortingGroup>();
					}
				}
				else
				{
					_sortingGroup = GetComponent<SortingGroup>();
					if (_sortingGroup != null)
					{
						UnityEngine.Object.DestroyImmediate(_sortingGroup);
					}
				}
				_UpdateSlotsSorting();
			}
		}

		public string sortingLayerName
		{
			get
			{
				return _sortingLayerName;
			}
			set
			{
				if (_sortingLayerName == value)
				{
				}
				_sortingLayerName = value;
				_UpdateSlotsSorting();
			}
		}

		public int sortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				if (_sortingOrder == value)
				{
				}
				_sortingOrder = value;
				_UpdateSlotsSorting();
			}
		}

		public float zSpace
		{
			get
			{
				return _zSpace;
			}
			set
			{
				if (value < 0f || float.IsNaN(value))
				{
					value = 0f;
				}
				if (_zSpace != value)
				{
					_zSpace = value;
					_UpdateSlotsSorting();
				}
			}
		}

		public ColorTransform color
		{
			get
			{
				return _colorTransform;
			}
			set
			{
				_colorTransform.CopyFrom(value);
				foreach (Slot slot in _armature.GetSlots())
				{
					slot._colorDirty = true;
				}
			}
		}

		public SortingGroup sortingGroup => _sortingGroup;

		public void DBClear()
		{
			if (_armature != null)
			{
				_armature = null;
				if (_disposeProxy)
				{
					try
					{
						GameObject gameObject = base.gameObject;
						UnityFactoryHelper.DestroyUnityObject(base.gameObject);
					}
					catch (Exception)
					{
					}
				}
			}
			unityData = null;
			armatureName = null;
			animationName = null;
			isUGUI = false;
			debugDraw = false;
			_disposeProxy = true;
			_armature = null;
			_colorTransform.Identity();
			_sortingMode = SortingMode.SortByZ;
			_sortingLayerName = "Default";
			_sortingOrder = 0;
			_playTimes = 0;
			_timeScale = 1f;
			_zSpace = 0f;
			_flipX = false;
			_flipY = false;
			_hasSortingGroup = false;
			_debugDrawer = null;
			_armatureZ = 0;
			_closeCombineMeshs = false;
		}

		public void DBInit(Armature armature)
		{
			_armature = armature;
		}

		public void DBUpdate()
		{
		}

		private void CreateLineMaterial()
		{
			if (!_debugDrawer)
			{
				Shader shader = Shader.Find("Hidden/Internal-Colored");
				_debugDrawer = new Material(shader);
				_debugDrawer.hideFlags = HideFlags.HideAndDontSave;
				_debugDrawer.SetInt("_SrcBlend", 5);
				_debugDrawer.SetInt("_DstBlend", 10);
				_debugDrawer.SetInt("_Cull", 0);
				_debugDrawer.SetInt("_ZWrite", 0);
			}
		}

		private void OnRenderObject()
		{
			if (!DragonBones.debugDraw && !debugDraw)
			{
				return;
			}
			Color c = new Color(0f, 1f, 1f, 0.7f);
			Color c2 = new Color(1f, 0f, 1f, 1f);
			CreateLineMaterial();
			_debugDrawer.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(base.transform.localToWorldMatrix);
			List<Bone> bones = _armature.GetBones();
			float num = 0.02f;
			for (int i = 0; i < bones.Count; i++)
			{
				Bone bone = bones[i];
				float num2 = Math.Max(bone.boneData.length, num);
				Vector3 vector = new Vector3(bone.globalTransformMatrix.tx, bone.globalTransformMatrix.ty, 0f);
				Vector3 vector2 = new Vector3(bone.globalTransformMatrix.a * num2, bone.globalTransformMatrix.b * num2, 0f) + vector;
				Vector3 normalized = (vector - vector2).normalized;
				Vector3 v = Quaternion.AngleAxis(90f, Vector3.forward) * normalized * num + vector;
				Vector3 v2 = Quaternion.AngleAxis(-90f, Vector3.forward) * normalized * num + vector;
				Vector3 v3 = vector + normalized * num;
				GL.Begin(1);
				GL.Color(c);
				GL.Vertex(v);
				GL.Vertex(v2);
				GL.End();
				GL.Begin(1);
				GL.Color(c);
				GL.Vertex(v3);
				GL.Vertex(vector2);
				GL.End();
			}
			Point point = new Point();
			List<Slot> slots = _armature.GetSlots();
			for (int j = 0; j < slots.Count; j++)
			{
				UnitySlot unitySlot = slots[j] as UnitySlot;
				BoundingBoxData boundingBoxData = unitySlot.boundingBoxData;
				if (boundingBoxData == null)
				{
					continue;
				}
				Bone parent = unitySlot.parent;
				unitySlot.UpdateTransformAndMatrix();
				unitySlot.UpdateGlobalTransform();
				float tx = unitySlot.globalTransformMatrix.tx;
				float ty = unitySlot.globalTransformMatrix.ty;
				float width = boundingBoxData.width;
				float height = boundingBoxData.height;
				switch (boundingBoxData.type)
				{
				case BoundingBoxType.Rectangle:
				{
					GL.Begin(2);
					GL.Color(c2);
					Vector3 v4 = new Vector3(tx - width * 0.5f, ty + height * 0.5f, 0f);
					Vector3 v5 = new Vector3(tx - width * 0.5f, ty - height * 0.5f, 0f);
					Vector3 v6 = new Vector3(tx + width * 0.5f, ty + height * 0.5f, 0f);
					Vector3 v7 = new Vector3(tx + width * 0.5f, ty - height * 0.5f, 0f);
					GL.Vertex(v4);
					GL.Vertex(v6);
					GL.Vertex(v7);
					GL.Vertex(v5);
					GL.Vertex(v4);
					GL.End();
					break;
				}
				case BoundingBoxType.Polygon:
				{
					List<float> vertices = (boundingBoxData as PolygonBoundingBoxData).vertices;
					GL.Begin(2);
					GL.Color(c2);
					for (int k = 0; k < vertices.Count; k += 2)
					{
						unitySlot.globalTransformMatrix.TransformPoint(vertices[k], vertices[k + 1], point);
						GL.Vertex3(point.x, point.y, 0f);
					}
					unitySlot.globalTransformMatrix.TransformPoint(vertices[0], vertices[1], point);
					GL.Vertex3(point.x, point.y, 0f);
					GL.End();
					break;
				}
				}
			}
			GL.PopMatrix();
		}

		public void Dispose(bool disposeProxy = true)
		{
			_disposeProxy = disposeProxy;
			if (_armature != null)
			{
				_armature.Dispose();
			}
		}

		private void _UpdateSortingGroup()
		{
			_sortingGroup = GetComponent<SortingGroup>();
			if (_sortingGroup != null)
			{
				_sortingMode = SortingMode.SortByOrder;
				_sortingLayerName = _sortingGroup.sortingLayerName;
				_sortingOrder = _sortingGroup.sortingOrder;
				foreach (UnitySlot slot in _armature.GetSlots())
				{
					if (slot.childArmature != null)
					{
						UnityArmatureComponent unityArmatureComponent = slot.childArmature.proxy as UnityArmatureComponent;
						unityArmatureComponent._sortingGroup = unityArmatureComponent.GetComponent<SortingGroup>();
						if (unityArmatureComponent._sortingGroup == null)
						{
							unityArmatureComponent._sortingGroup = unityArmatureComponent.gameObject.AddComponent<SortingGroup>();
						}
						unityArmatureComponent._sortingGroup.sortingLayerName = _sortingLayerName;
						unityArmatureComponent._sortingGroup.sortingOrder = _sortingOrder;
					}
				}
			}
			else
			{
				_sortingMode = SortingMode.SortByZ;
				foreach (UnitySlot slot2 in _armature.GetSlots())
				{
					if (slot2.childArmature != null)
					{
						UnityArmatureComponent unityArmatureComponent2 = slot2.childArmature.proxy as UnityArmatureComponent;
						unityArmatureComponent2._sortingGroup = unityArmatureComponent2.GetComponent<SortingGroup>();
						if (unityArmatureComponent2._sortingGroup != null)
						{
							UnityEngine.Object.DestroyImmediate(unityArmatureComponent2._sortingGroup);
						}
					}
				}
			}
			_UpdateSlotsSorting();
		}

		private void _UpdateSlotsSorting()
		{
			if (_armature != null)
			{
				if (!isUGUI && (bool)_sortingGroup)
				{
					_sortingMode = SortingMode.SortByOrder;
					_sortingGroup.sortingLayerName = _sortingLayerName;
					_sortingGroup.sortingOrder = _sortingOrder;
				}
				foreach (UnitySlot slot in _armature.GetSlots())
				{
					GameObject renderDisplay = slot._renderDisplay;
					if (!(renderDisplay == null))
					{
						UnitySlot unitySlot2 = slot;
						Vector3 localPosition = renderDisplay.transform.localPosition;
						float x = localPosition.x;
						Vector3 localPosition2 = renderDisplay.transform.localPosition;
						unitySlot2._SetZorder(new Vector3(x, localPosition2.y, (float)(-slot._zOrder) * (_zSpace + 0.001f)));
						if (slot.childArmature != null)
						{
							(slot.childArmature.proxy as UnityArmatureComponent)._UpdateSlotsSorting();
						}
					}
				}
			}
		}

		private void Awake()
		{
			if (unityData != null && unityData.dragonBonesJSON != null && unityData.textureAtlas != null)
			{
				DragonBonesData dragonBonesData = UnityFactory.factory.LoadData(unityData, isUGUI);
				if (dragonBonesData != null && !string.IsNullOrEmpty(armatureName))
				{
					UnityFactory.factory.BuildArmatureComponent(armatureName, unityData.dataName, null, null, base.gameObject, isUGUI);
				}
			}
			if (_armature != null)
			{
				if (!isUGUI)
				{
					_sortingGroup = GetComponent<SortingGroup>();
				}
				_UpdateSlotsSorting();
				_armature.flipX = _flipX;
				_armature.flipY = _flipY;
				_armature.animation.timeScale = _timeScale;
				if (!string.IsNullOrEmpty(animationName))
				{
					_armature.animation.Play(animationName, _playTimes);
				}
			}
		}

		private void Start()
		{
			if (_closeCombineMeshs)
			{
				CloseCombineMeshs();
			}
			else
			{
				OpenCombineMeshs();
			}
		}

		private void LateUpdate()
		{
			if (_armature != null)
			{
				_flipX = _armature.flipX;
				_flipY = _armature.flipY;
				bool flag = GetComponent<SortingGroup>() != null;
				if (flag != _hasSortingGroup)
				{
					_hasSortingGroup = flag;
					_UpdateSortingGroup();
				}
			}
		}

		private void OnDestroy()
		{
			if (_armature != null)
			{
				Armature armature = _armature;
				_armature = null;
				armature.Dispose();
				if (!Application.isPlaying)
				{
					UnityFactory.factory._dragonBones.AdvanceTime(0f);
				}
			}
			_disposeProxy = true;
			_armature = null;
		}

		private void OpenCombineMeshs()
		{
			if (!isUGUI)
			{
				UnityCombineMeshs component = base.gameObject.GetComponent<UnityCombineMeshs>();
				if (component == null)
				{
					component = base.gameObject.AddComponent<UnityCombineMeshs>();
				}
				if (_armature != null)
				{
					List<Slot> slots = _armature.GetSlots();
					foreach (Slot item in slots)
					{
						if (item.childArmature != null)
						{
							(item.childArmature.proxy as UnityArmatureComponent).OpenCombineMeshs();
						}
					}
				}
			}
		}

		public void CloseCombineMeshs()
		{
			_closeCombineMeshs = true;
			UnityCombineMeshs component = base.gameObject.GetComponent<UnityCombineMeshs>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			if (_armature != null)
			{
				List<Slot> slots = _armature.GetSlots();
				foreach (Slot item in slots)
				{
					if (item.childArmature != null)
					{
						(item.childArmature.proxy as UnityArmatureComponent).CloseCombineMeshs();
					}
				}
			}
		}
	}
}
