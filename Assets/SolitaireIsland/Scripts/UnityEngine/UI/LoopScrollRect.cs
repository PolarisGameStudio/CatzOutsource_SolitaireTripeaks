using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public abstract class LoopScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup, IEventSystemHandler, ILayoutController
	{
		public enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		public enum ScrollbarVisibility
		{
			Permanent,
			AutoHide,
			AutoHideAndExpandViewport
		}

		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}

		[Tooltip("Prefab Source")]
		public LoopScrollPrefabSource prefabSource;

		[Tooltip("Total count, negative means INFINITE mode")]
		public int totalCount;

		[NonSerialized]
		[HideInInspector]
		public LoopScrollDataSource dataSource = LoopScrollSendIndexSource.Instance;

		protected float threshold;

		[Tooltip("Reverse direction for dragging")]
		public bool reverseDirection;

		[Tooltip("Rubber scale for outside")]
		public float rubberScale = 1f;

		protected int itemTypeStart;

		protected int itemTypeEnd;

		protected int directionSign;

		private float m_ContentSpacing = -1f;

		protected GridLayoutGroup m_GridLayout;

		private int m_ContentConstraintCount;

		[SerializeField]
		private RectTransform m_Content;

		[SerializeField]
		private bool m_Horizontal = true;

		[SerializeField]
		private bool m_Vertical = true;

		[SerializeField]
		private MovementType m_MovementType = MovementType.Elastic;

		[SerializeField]
		private float m_Elasticity = 0.1f;

		[SerializeField]
		private bool m_Inertia = true;

		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		[SerializeField]
		private RectTransform m_Viewport;

		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		[SerializeField]
		private ScrollbarVisibility m_HorizontalScrollbarVisibility;

		[SerializeField]
		private ScrollbarVisibility m_VerticalScrollbarVisibility;

		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		[SerializeField]
		private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();

		public UnityEvent OnFullLoad = new UnityEvent();

		public UnityEvent OnFullRefresh = new UnityEvent();

		private Vector2 m_PointerStartLocalCursor = Vector2.zero;

		private Vector2 m_ContentStartPosition = Vector2.zero;

		private RectTransform m_ViewRect;

		private Bounds m_ContentBounds;

		private Bounds m_ViewBounds;

		private Vector2 m_Velocity;

		private bool m_Dragging;

		private Vector2 m_PrevPosition = Vector2.zero;

		private Bounds m_PrevContentBounds;

		private Bounds m_PrevViewBounds;

		[NonSerialized]
		private bool m_HasRebuiltLayout;

		private bool m_HSliderExpand;

		private bool m_VSliderExpand;

		private float m_HSliderHeight;

		private float m_VSliderWidth;

		[NonSerialized]
		private RectTransform m_Rect;

		private RectTransform m_HorizontalScrollbarRect;

		private RectTransform m_VerticalScrollbarRect;

		private DrivenRectTransformTracker m_Tracker;

		private readonly Vector3[] m_Corners = new Vector3[4];

		public object[] objectsToFill
		{
			set
			{
				if (value != null)
				{
					dataSource = new LoopScrollArraySource<object>(value);
				}
				else
				{
					dataSource = LoopScrollSendIndexSource.Instance;
				}
			}
		}

		protected float contentSpacing
		{
			get
			{
				if (m_ContentSpacing >= 0f)
				{
					return m_ContentSpacing;
				}
				m_ContentSpacing = 0f;
				if (content != null)
				{
					HorizontalOrVerticalLayoutGroup component = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
					if (component != null)
					{
						m_ContentSpacing = component.spacing;
					}
					m_GridLayout = content.GetComponent<GridLayoutGroup>();
					if (m_GridLayout != null)
					{
						m_ContentSpacing = Mathf.Abs(GetDimension(m_GridLayout.spacing));
					}
				}
				return m_ContentSpacing;
			}
		}

		protected int contentConstraintCount
		{
			get
			{
				if (m_ContentConstraintCount > 0)
				{
					return m_ContentConstraintCount;
				}
				m_ContentConstraintCount = 1;
				if (content != null)
				{
					GridLayoutGroup component = content.GetComponent<GridLayoutGroup>();
					if (component != null)
					{
						if (component.constraint == GridLayoutGroup.Constraint.Flexible)
						{
							UnityEngine.Debug.LogWarning("[LoopScrollRect] Flexible not supported yet");
						}
						m_ContentConstraintCount = component.constraintCount;
					}
				}
				return m_ContentConstraintCount;
			}
		}

		public RectTransform content
		{
			get
			{
				return m_Content;
			}
			set
			{
				m_Content = value;
			}
		}

		public bool horizontal
		{
			get
			{
				return m_Horizontal;
			}
			set
			{
				m_Horizontal = value;
			}
		}

		public bool vertical
		{
			get
			{
				return m_Vertical;
			}
			set
			{
				m_Vertical = value;
			}
		}

		public MovementType movementType
		{
			get
			{
				return m_MovementType;
			}
			set
			{
				m_MovementType = value;
			}
		}

		public float elasticity
		{
			get
			{
				return m_Elasticity;
			}
			set
			{
				m_Elasticity = value;
			}
		}

		public bool inertia
		{
			get
			{
				return m_Inertia;
			}
			set
			{
				m_Inertia = value;
			}
		}

		public float decelerationRate
		{
			get
			{
				return m_DecelerationRate;
			}
			set
			{
				m_DecelerationRate = value;
			}
		}

		public float scrollSensitivity
		{
			get
			{
				return m_ScrollSensitivity;
			}
			set
			{
				m_ScrollSensitivity = value;
			}
		}

		public RectTransform viewport
		{
			get
			{
				return m_Viewport;
			}
			set
			{
				m_Viewport = value;
				SetDirtyCaching();
			}
		}

		public Scrollbar horizontalScrollbar
		{
			get
			{
				return m_HorizontalScrollbar;
			}
			set
			{
				if ((bool)m_HorizontalScrollbar)
				{
					m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
				}
				m_HorizontalScrollbar = value;
				if ((bool)m_HorizontalScrollbar)
				{
					m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}

		public Scrollbar verticalScrollbar
		{
			get
			{
				return m_VerticalScrollbar;
			}
			set
			{
				if ((bool)m_VerticalScrollbar)
				{
					m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
				}
				m_VerticalScrollbar = value;
				if ((bool)m_VerticalScrollbar)
				{
					m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
				}
				SetDirtyCaching();
			}
		}

		public ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return m_HorizontalScrollbarVisibility;
			}
			set
			{
				m_HorizontalScrollbarVisibility = value;
				SetDirtyCaching();
			}
		}

		public ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return m_VerticalScrollbarVisibility;
			}
			set
			{
				m_VerticalScrollbarVisibility = value;
				SetDirtyCaching();
			}
		}

		public float horizontalScrollbarSpacing
		{
			get
			{
				return m_HorizontalScrollbarSpacing;
			}
			set
			{
				m_HorizontalScrollbarSpacing = value;
				SetDirty();
			}
		}

		public float verticalScrollbarSpacing
		{
			get
			{
				return m_VerticalScrollbarSpacing;
			}
			set
			{
				m_VerticalScrollbarSpacing = value;
				SetDirty();
			}
		}

		public ScrollRectEvent onValueChanged
		{
			get
			{
				return m_OnValueChanged;
			}
			set
			{
				m_OnValueChanged = value;
			}
		}

		protected RectTransform viewRect
		{
			get
			{
				if (m_ViewRect == null)
				{
					m_ViewRect = m_Viewport;
				}
				if (m_ViewRect == null)
				{
					m_ViewRect = (RectTransform)base.transform;
				}
				return m_ViewRect;
			}
		}

		public Vector2 velocity
		{
			get
			{
				return m_Velocity;
			}
			set
			{
				m_Velocity = value;
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
			}
			set
			{
				SetNormalizedPosition(value.x, 0);
				SetNormalizedPosition(value.y, 1);
			}
		}

		public float horizontalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				if (totalCount > 0 && itemTypeEnd > itemTypeStart)
				{
					Vector3 size = m_ContentBounds.size;
					float num = size.x / (float)(itemTypeEnd - itemTypeStart);
					float num2 = num * (float)totalCount;
					Vector3 min = m_ContentBounds.min;
					float num3 = min.x - num * (float)itemTypeStart;
					float num4 = num2;
					Vector3 size2 = m_ViewBounds.size;
					if (num4 <= size2.x)
					{
						Vector3 min2 = m_ViewBounds.min;
						return (min2.x > num3) ? 1 : 0;
					}
					Vector3 min3 = m_ViewBounds.min;
					float num5 = min3.x - num3;
					float num6 = num2;
					Vector3 size3 = m_ViewBounds.size;
					return num5 / (num6 - size3.x);
				}
				return 0.5f;
			}
			set
			{
				SetNormalizedPosition(value, 0);
			}
		}

		public float verticalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				if (totalCount > 0 && itemTypeEnd > itemTypeStart)
				{
					Vector3 size = m_ContentBounds.size;
					float num = size.y / (float)(itemTypeEnd - itemTypeStart);
					float num2 = num * (float)totalCount;
					Vector3 max = m_ContentBounds.max;
					float num3 = max.y + num * (float)itemTypeStart;
					float num4 = num2;
					Vector3 size2 = m_ViewBounds.size;
					if (num4 <= size2.y)
					{
						float num5 = num3;
						Vector3 max2 = m_ViewBounds.max;
						return (num5 > max2.y) ? 1 : 0;
					}
					float num6 = num3;
					Vector3 max3 = m_ViewBounds.max;
					float num7 = num6 - max3.y;
					float num8 = num2;
					Vector3 size3 = m_ViewBounds.size;
					return num7 / (num8 - size3.y);
				}
				return 0.5f;
			}
			set
			{
				SetNormalizedPosition(value, 1);
			}
		}

		private bool hScrollingNeeded
		{
			get
			{
				if (Application.isPlaying)
				{
					Vector3 size = m_ContentBounds.size;
					float x = size.x;
					Vector3 size2 = m_ViewBounds.size;
					return x > size2.x + 0.01f;
				}
				return true;
			}
		}

		private bool vScrollingNeeded
		{
			get
			{
				if (Application.isPlaying)
				{
					Vector3 size = m_ContentBounds.size;
					float y = size.y;
					Vector3 size2 = m_ViewBounds.size;
					return y > size2.y + 0.01f;
				}
				return true;
			}
		}

		public virtual float minWidth => -1f;

		public virtual float preferredWidth => -1f;

		public virtual float flexibleWidth
		{
			get;
			private set;
		}

		public virtual float minHeight => -1f;

		public virtual float preferredHeight => -1f;

		public virtual float flexibleHeight => -1f;

		public virtual int layoutPriority => -1;

		protected LoopScrollRect()
		{
			flexibleWidth = -1f;
		}

		protected abstract float GetSize(RectTransform item);

		protected abstract float GetDimension(Vector2 vector);

		protected abstract Vector2 GetVector(float value);

		protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
		{
			return false;
		}

		public void ClearCells()
		{
			if (Application.isPlaying)
			{
				itemTypeStart = 0;
				itemTypeEnd = 0;
				totalCount = 0;
				objectsToFill = null;
				for (int num = content.childCount - 1; num >= 0; num--)
				{
					prefabSource.ReturnObject(content.GetChild(num));
				}
			}
		}

		public void SrollToCell(int index, float speed)
		{
			if (totalCount >= 0 && (index < 0 || index >= totalCount))
			{
				UnityEngine.Debug.LogWarningFormat("invalid index {0}", index);
				return;
			}
			if (speed <= 0f)
			{
				UnityEngine.Debug.LogWarningFormat("invalid speed {0}", speed);
				return;
			}
			StopAllCoroutines();
			StartCoroutine(ScrollToCellCoroutine(index, speed));
		}

		private IEnumerator ScrollToCellCoroutine(int index, float speed)
		{
			bool needMoving = true;
			while (needMoving)
			{
				yield return null;
				if (m_Dragging)
				{
					continue;
				}
				float num;
				if (index < itemTypeStart)
				{
					num = (0f - Time.deltaTime) * speed;
				}
				else if (index >= itemTypeEnd)
				{
					num = Time.deltaTime * speed;
				}
				else
				{
					m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
					Bounds bounds4Item = GetBounds4Item(index);
					float num2 = 0f;
					if (directionSign == -1)
					{
						float num3;
						if (reverseDirection)
						{
							Vector3 min = m_ViewBounds.min;
							float y = min.y;
							Vector3 min2 = bounds4Item.min;
							num3 = y - min2.y;
						}
						else
						{
							Vector3 max = m_ViewBounds.max;
							float y2 = max.y;
							Vector3 max2 = bounds4Item.max;
							num3 = y2 - max2.y;
						}
						num2 = num3;
					}
					else if (directionSign == 1)
					{
						float num4;
						if (reverseDirection)
						{
							Vector3 max3 = bounds4Item.max;
							float x = max3.x;
							Vector3 max4 = m_ViewBounds.max;
							num4 = x - max4.x;
						}
						else
						{
							Vector3 min3 = bounds4Item.min;
							float x2 = min3.x;
							Vector3 min4 = m_ViewBounds.min;
							num4 = x2 - min4.x;
						}
						num2 = num4;
					}
					if (totalCount >= 0)
					{
						if (num2 > 0f && itemTypeEnd == totalCount && !reverseDirection)
						{
							bounds4Item = GetBounds4Item(totalCount - 1);
							if (directionSign == -1)
							{
								Vector3 min5 = bounds4Item.min;
								float y3 = min5.y;
								Vector3 min6 = m_ViewBounds.min;
								if (y3 > min6.y)
								{
									break;
								}
							}
							if (directionSign == 1)
							{
								Vector3 max5 = bounds4Item.max;
								float x3 = max5.x;
								Vector3 max6 = m_ViewBounds.max;
								if (x3 < max6.x)
								{
									break;
								}
							}
						}
						else if (num2 < 0f && itemTypeStart == 0 && reverseDirection)
						{
							bounds4Item = GetBounds4Item(0);
							if (directionSign == -1)
							{
								Vector3 max7 = bounds4Item.max;
								float y4 = max7.y;
								Vector3 max8 = m_ViewBounds.max;
								if (y4 < max8.y)
								{
									break;
								}
							}
							if (directionSign == 1)
							{
								Vector3 min7 = bounds4Item.min;
								float x4 = min7.x;
								Vector3 min8 = m_ViewBounds.min;
								if (x4 > min8.x)
								{
									break;
								}
							}
						}
					}
					float num5 = Time.deltaTime * speed;
					if (Mathf.Abs(num2) < num5)
					{
						needMoving = false;
						num = num2;
					}
					else
					{
						num = Mathf.Sign(num2) * num5;
					}
				}
				if (num != 0f)
				{
					Vector2 vector = GetVector(num);
					content.anchoredPosition += vector;
					m_PrevPosition += vector;
					m_ContentStartPosition += vector;
				}
			}
			StopMovement();
			UpdatePrevData();
		}

		public void RefreshCells()
		{
			if (!Application.isPlaying || !base.isActiveAndEnabled)
			{
				return;
			}
			itemTypeEnd = itemTypeStart;
			for (int i = 0; i < content.childCount; i++)
			{
				if (itemTypeEnd < totalCount)
				{
					dataSource.ProvideData(content.GetChild(i), itemTypeEnd);
					itemTypeEnd++;
				}
				else
				{
					prefabSource.ReturnObject(content.GetChild(i));
					i--;
				}
			}
		}

		public void RefillCellsFromEnd(int offset = 0)
		{
			if (!Application.isPlaying || prefabSource == null)
			{
				return;
			}
			StopMovement();
			itemTypeEnd = ((!reverseDirection) ? (totalCount - offset) : offset);
			itemTypeStart = itemTypeEnd;
			if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
			{
				UnityEngine.Debug.LogWarning("Grid will become strange since we can't fill items in the last line");
			}
			for (int num = m_Content.childCount - 1; num >= 0; num--)
			{
				prefabSource.ReturnObject(m_Content.GetChild(num));
			}
			float num2 = 0f;
			float num3 = 0f;
			if (directionSign == -1)
			{
				Vector2 size = viewRect.rect.size;
				num2 = size.y;
			}
			else
			{
				Vector2 size2 = viewRect.rect.size;
				num2 = size2.x;
			}
			float num4;
			for (; num2 > num3; num3 += num4)
			{
				num4 = ((!reverseDirection) ? NewItemAtStart() : NewItemAtEnd());
				if (num4 <= 0f)
				{
					break;
				}
			}
			Vector2 anchoredPosition = m_Content.anchoredPosition;
			float num5 = Mathf.Max(0f, num3 - num2);
			if (reverseDirection)
			{
				num5 = 0f - num5;
			}
			if (directionSign == -1)
			{
				anchoredPosition.y = num5;
			}
			else if (directionSign == 1)
			{
				anchoredPosition.x = 0f - num5;
			}
			m_Content.anchoredPosition = anchoredPosition;
		}

		public void RefillCells(int offset = 0)
		{
			if (!Application.isPlaying || prefabSource == null)
			{
				return;
			}
			StopMovement();
			itemTypeStart = ((!reverseDirection) ? offset : (totalCount - offset));
			itemTypeEnd = itemTypeStart;
			if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
			{
				UnityEngine.Debug.LogWarning("Grid will become strange since we can't fill items in the first line");
			}
			for (int num = m_Content.childCount - 1; num >= 0; num--)
			{
				prefabSource.ReturnObject(m_Content.GetChild(num));
			}
			float num2 = 0f;
			float num3 = 0f;
			if (directionSign == -1)
			{
				Vector2 size = viewRect.rect.size;
				num2 = size.y;
			}
			else
			{
				Vector2 size2 = viewRect.rect.size;
				num2 = size2.x;
			}
			float num4;
			for (; num2 > num3; num3 += num4)
			{
				num4 = ((!reverseDirection) ? NewItemAtEnd() : NewItemAtStart());
				if (num4 <= 0f)
				{
					break;
				}
			}
			Vector2 anchoredPosition = m_Content.anchoredPosition;
			if (directionSign == -1)
			{
				anchoredPosition.y = 0f;
			}
			else if (directionSign == 1)
			{
				anchoredPosition.x = 0f;
			}
			m_Content.anchoredPosition = anchoredPosition;
		}

		protected float NewItemAtStart()
		{
			if (totalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				itemTypeStart--;
				RectTransform rectTransform = InstantiateNextItem(itemTypeStart);
				rectTransform.SetAsFirstSibling();
				num = Mathf.Max(GetSize(rectTransform), num);
			}
			threshold = Mathf.Max(threshold, num * 1.5f);
			if (!reverseDirection)
			{
				Vector2 vector = GetVector(num);
				content.anchoredPosition += vector;
				m_PrevPosition += vector;
				m_ContentStartPosition += vector;
			}
			return num;
		}

		protected float DeleteItemAtStart()
		{
			if (((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeEnd >= totalCount - 1) || content.childCount == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				RectTransform rectTransform = content.GetChild(0) as RectTransform;
				num = Mathf.Max(GetSize(rectTransform), num);
				prefabSource.ReturnObject(rectTransform);
				itemTypeStart++;
				if (content.childCount == 0)
				{
					break;
				}
			}
			if (!reverseDirection)
			{
				Vector2 vector = GetVector(num);
				content.anchoredPosition -= vector;
				m_PrevPosition -= vector;
				m_ContentStartPosition -= vector;
			}
			return num;
		}

		protected float NewItemAtEnd()
		{
			if (totalCount >= 0 && itemTypeEnd >= totalCount)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = contentConstraintCount - content.childCount % contentConstraintCount;
			for (int i = 0; i < num2; i++)
			{
				RectTransform item = InstantiateNextItem(itemTypeEnd);
				num = Mathf.Max(GetSize(item), num);
				itemTypeEnd++;
				if (totalCount >= 0 && itemTypeEnd >= totalCount)
				{
					break;
				}
			}
			threshold = Mathf.Max(threshold, num * 1.5f);
			if (reverseDirection)
			{
				Vector2 vector = GetVector(num);
				content.anchoredPosition -= vector;
				m_PrevPosition -= vector;
				m_ContentStartPosition -= vector;
			}
			return num;
		}

		protected float DeleteItemAtEnd()
		{
			if (((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeStart < contentConstraintCount) || content.childCount == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				RectTransform rectTransform = content.GetChild(content.childCount - 1) as RectTransform;
				num = Mathf.Max(GetSize(rectTransform), num);
				prefabSource.ReturnObject(rectTransform);
				itemTypeEnd--;
				if (itemTypeEnd % contentConstraintCount == 0 || content.childCount == 0)
				{
					break;
				}
			}
			if (reverseDirection)
			{
				Vector2 vector = GetVector(num);
				content.anchoredPosition += vector;
				m_PrevPosition += vector;
				m_ContentStartPosition += vector;
			}
			return num;
		}

		private RectTransform InstantiateNextItem(int itemIdx)
		{
			RectTransform component = prefabSource.GetObject().GetComponent<RectTransform>();
			component.transform.SetParent(content, worldPositionStays: false);
			component.gameObject.SetActive(value: true);
			dataSource.ProvideData(component, itemIdx);
			return component;
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				UpdateCachedData();
			}
			if (executing == CanvasUpdate.PostLayout)
			{
				UpdateBounds();
				UpdateScrollbars(Vector2.zero);
				UpdatePrevData();
				m_HasRebuiltLayout = true;
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateCachedData()
		{
			Transform transform = base.transform;
			m_HorizontalScrollbarRect = ((!(m_HorizontalScrollbar == null)) ? (m_HorizontalScrollbar.transform as RectTransform) : null);
			m_VerticalScrollbarRect = ((!(m_VerticalScrollbar == null)) ? (m_VerticalScrollbar.transform as RectTransform) : null);
			bool flag = viewRect.parent == transform;
			bool flag2 = !m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform;
			bool flag3 = !m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform;
			bool flag4 = flag && flag2 && flag3;
			m_HSliderExpand = (flag4 && (bool)m_HorizontalScrollbarRect && horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport);
			m_VSliderExpand = (flag4 && (bool)m_VerticalScrollbarRect && verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport);
			m_HSliderHeight = ((!(m_HorizontalScrollbarRect == null)) ? m_HorizontalScrollbarRect.rect.height : 0f);
			m_VSliderWidth = ((!(m_VerticalScrollbarRect == null)) ? m_VerticalScrollbarRect.rect.width : 0f);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
			}
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		protected override void OnDisable()
		{
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
			}
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
			}
			m_HasRebuiltLayout = false;
			m_Tracker.Clear();
			m_Velocity = Vector2.zero;
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		public override bool IsActive()
		{
			return base.IsActive() && m_Content != null;
		}

		private void EnsureLayoutHasRebuilt()
		{
			if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		public virtual void StopMovement()
		{
			m_Velocity = Vector2.zero;
		}

		public virtual void OnScroll(PointerEventData data)
		{
			if (!IsActive())
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateBounds();
			Vector2 scrollDelta = data.scrollDelta;
			scrollDelta.y *= -1f;
			if (vertical && !horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (horizontal && !vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 anchoredPosition = m_Content.anchoredPosition;
			anchoredPosition += scrollDelta * m_ScrollSensitivity;
			if (m_MovementType == MovementType.Clamped)
			{
				anchoredPosition += CalculateOffset(anchoredPosition - m_Content.anchoredPosition);
			}
			SetContentAnchoredPosition(anchoredPosition);
			UpdateBounds();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				m_Velocity = Vector2.zero;
			}
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left && IsActive())
			{
				UpdateBounds();
				m_PointerStartLocalCursor = Vector2.zero;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
				m_ContentStartPosition = m_Content.anchoredPosition;
				m_Dragging = true;
			}
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				m_Dragging = false;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != 0 || !IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
			{
				return;
			}
			UpdateBounds();
			Vector2 b = localPoint - m_PointerStartLocalCursor;
			Vector2 vector = m_ContentStartPosition + b;
			Vector2 b2 = CalculateOffset(vector - m_Content.anchoredPosition);
			vector += b2;
			if (m_MovementType == MovementType.Elastic)
			{
				if (b2.x != 0f)
				{
					float x = vector.x;
					float x2 = b2.x;
					Vector3 size = m_ViewBounds.size;
					vector.x = x - RubberDelta(x2, size.x) * rubberScale;
				}
				if (b2.y != 0f)
				{
					float y = vector.y;
					float y2 = b2.y;
					Vector3 size2 = m_ViewBounds.size;
					vector.y = y - RubberDelta(y2, size2.y) * rubberScale;
				}
			}
			SetContentAnchoredPosition(vector);
		}

		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!m_Horizontal)
			{
				Vector2 anchoredPosition = m_Content.anchoredPosition;
				position.x = anchoredPosition.x;
			}
			if (!m_Vertical)
			{
				Vector2 anchoredPosition2 = m_Content.anchoredPosition;
				position.y = anchoredPosition2.y;
			}
			if (position != m_Content.anchoredPosition)
			{
				m_Content.anchoredPosition = position;
				UpdateBounds(updateItems: true);
			}
		}

		protected virtual void LateUpdate()
		{
			if (!m_Content)
			{
				return;
			}
			EnsureLayoutHasRebuilt();
			UpdateScrollbarVisibility();
			UpdateBounds();
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Vector2 vector = CalculateOffset(Vector2.zero);
			if (!m_Dragging && (vector != Vector2.zero || m_Velocity != Vector2.zero))
			{
				Vector2 vector2 = m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (m_MovementType == MovementType.Elastic && vector[i] != 0f)
					{
						float currentVelocity = m_Velocity[i];
						vector2[i] = Mathf.SmoothDamp(m_Content.anchoredPosition[i], m_Content.anchoredPosition[i] + vector[i], ref currentVelocity, m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
						m_Velocity[i] = currentVelocity;
					}
					else if (m_Inertia)
					{
						ref Vector2 velocity = ref m_Velocity;
						ref Vector2 reference = ref velocity;
						int index;
						velocity[index = i] = reference[index] * Mathf.Pow(m_DecelerationRate, unscaledDeltaTime);
						if (Mathf.Abs(m_Velocity[i]) < 1f)
						{
							m_Velocity[i] = 0f;
						}
						int index2;
						vector2[index2 = i] = vector2[index2] + m_Velocity[i] * unscaledDeltaTime;
					}
					else
					{
						m_Velocity[i] = 0f;
					}
				}
				if (m_Velocity != Vector2.zero)
				{
					if (m_MovementType == MovementType.Clamped)
					{
						vector = CalculateOffset(vector2 - m_Content.anchoredPosition);
						vector2 += vector;
					}
					SetContentAnchoredPosition(vector2);
				}
			}
			if (m_Dragging && m_Inertia)
			{
				Vector3 b = (m_Content.anchoredPosition - m_PrevPosition) / unscaledDeltaTime;
				m_Velocity = Vector3.Lerp(m_Velocity, b, unscaledDeltaTime * 10f);
			}
			if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
			{
				UpdateScrollbars(vector);
				m_OnValueChanged.Invoke(normalizedPosition);
				UpdatePrevData();
			}
		}

		private void UpdatePrevData()
		{
			if (m_Content == null)
			{
				m_PrevPosition = Vector2.zero;
			}
			else
			{
				m_PrevPosition = m_Content.anchoredPosition;
			}
			m_PrevViewBounds = m_ViewBounds;
			m_PrevContentBounds = m_ContentBounds;
		}

		private void UpdateScrollbars(Vector2 offset)
		{
			if ((bool)m_HorizontalScrollbar)
			{
				Vector3 size = m_ContentBounds.size;
				if (size.x > 0f && totalCount > 0)
				{
					Scrollbar horizontalScrollbar = m_HorizontalScrollbar;
					Vector3 size2 = m_ViewBounds.size;
					float num = size2.x - Mathf.Abs(offset.x);
					Vector3 size3 = m_ContentBounds.size;
					horizontalScrollbar.size = Mathf.Clamp01(num / size3.x * (float)(itemTypeEnd - itemTypeStart) / (float)totalCount);
				}
				else
				{
					m_HorizontalScrollbar.size = 1f;
				}
				m_HorizontalScrollbar.value = horizontalNormalizedPosition;
			}
			if ((bool)m_VerticalScrollbar)
			{
				Vector3 size4 = m_ContentBounds.size;
				if (size4.y > 0f && totalCount > 0)
				{
					Scrollbar verticalScrollbar = m_VerticalScrollbar;
					Vector3 size5 = m_ViewBounds.size;
					float num2 = size5.y - Mathf.Abs(offset.y);
					Vector3 size6 = m_ContentBounds.size;
					verticalScrollbar.size = Mathf.Clamp01(num2 / size6.y * (float)(itemTypeEnd - itemTypeStart) / (float)totalCount);
				}
				else
				{
					m_VerticalScrollbar.size = 1f;
				}
				m_VerticalScrollbar.value = verticalNormalizedPosition;
			}
		}

		private void SetHorizontalNormalizedPosition(float value)
		{
			SetNormalizedPosition(value, 0);
		}

		private void SetVerticalNormalizedPosition(float value)
		{
			SetNormalizedPosition(value, 1);
		}

		private void SetNormalizedPosition(float value, int axis)
		{
			if (totalCount > 0 && itemTypeEnd > itemTypeStart)
			{
				EnsureLayoutHasRebuilt();
				UpdateBounds();
				Vector3 localPosition = m_Content.localPosition;
				float num = localPosition[axis];
				switch (axis)
				{
				case 0:
				{
					Vector3 size3 = m_ContentBounds.size;
					float num9 = size3.x / (float)(itemTypeEnd - itemTypeStart);
					float num10 = num9 * (float)totalCount;
					Vector3 min = m_ContentBounds.min;
					float num11 = min.x - num9 * (float)itemTypeStart;
					float num12 = num;
					Vector3 min2 = m_ViewBounds.min;
					num = num12 + (min2.x - value * (num10 - m_ViewBounds.size[axis]) - num11);
					break;
				}
				case 1:
				{
					Vector3 size = m_ContentBounds.size;
					float num2 = size.y / (float)(itemTypeEnd - itemTypeStart);
					float num3 = num2 * (float)totalCount;
					Vector3 max = m_ContentBounds.max;
					float num4 = max.y + num2 * (float)itemTypeStart;
					float num5 = num;
					float num6 = num4;
					float num7 = num3;
					Vector3 size2 = m_ViewBounds.size;
					float num8 = num6 - value * (num7 - size2.y);
					Vector3 max2 = m_ViewBounds.max;
					num = num5 - (num8 - max2.y);
					break;
				}
				}
				if (Mathf.Abs(localPosition[axis] - num) > 0.01f)
				{
					localPosition[axis] = num;
					m_Content.localPosition = localPosition;
					m_Velocity[axis] = 0f;
					UpdateBounds(updateItems: true);
				}
			}
		}

		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			if (m_HSliderExpand || m_VSliderExpand)
			{
				m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if (m_VSliderExpand && vScrollingNeeded)
			{
				RectTransform viewRect = this.viewRect;
				float x = 0f - (m_VSliderWidth + m_VerticalScrollbarSpacing);
				Vector2 sizeDelta = this.viewRect.sizeDelta;
				viewRect.sizeDelta = new Vector2(x, sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if (m_HSliderExpand && hScrollingNeeded)
			{
				RectTransform viewRect2 = this.viewRect;
				Vector2 sizeDelta2 = this.viewRect.sizeDelta;
				viewRect2.sizeDelta = new Vector2(sizeDelta2.x, 0f - (m_HSliderHeight + m_HorizontalScrollbarSpacing));
				m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				m_ContentBounds = GetBounds();
			}
			if (!m_VSliderExpand || !vScrollingNeeded)
			{
				return;
			}
			Vector2 sizeDelta3 = this.viewRect.sizeDelta;
			if (sizeDelta3.x == 0f)
			{
				Vector2 sizeDelta4 = this.viewRect.sizeDelta;
				if (sizeDelta4.y < 0f)
				{
					RectTransform viewRect3 = this.viewRect;
					float x2 = 0f - (m_VSliderWidth + m_VerticalScrollbarSpacing);
					Vector2 sizeDelta5 = this.viewRect.sizeDelta;
					viewRect3.sizeDelta = new Vector2(x2, sizeDelta5.y);
				}
			}
		}

		public virtual void SetLayoutVertical()
		{
			UpdateScrollbarLayout();
			m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
			m_ContentBounds = GetBounds();
		}

		private void UpdateScrollbarVisibility()
		{
			if ((bool)m_VerticalScrollbar && m_VerticalScrollbarVisibility != 0 && m_VerticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
			{
				m_VerticalScrollbar.gameObject.SetActive(vScrollingNeeded);
			}
			if ((bool)m_HorizontalScrollbar && m_HorizontalScrollbarVisibility != 0 && m_HorizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
			{
				m_HorizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
			}
		}

		private void UpdateScrollbarLayout()
		{
			if (m_VSliderExpand && (bool)m_HorizontalScrollbar)
			{
				m_Tracker.Add(this, m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				RectTransform horizontalScrollbarRect = m_HorizontalScrollbarRect;
				Vector2 anchorMin = m_HorizontalScrollbarRect.anchorMin;
				horizontalScrollbarRect.anchorMin = new Vector2(0f, anchorMin.y);
				RectTransform horizontalScrollbarRect2 = m_HorizontalScrollbarRect;
				Vector2 anchorMax = m_HorizontalScrollbarRect.anchorMax;
				horizontalScrollbarRect2.anchorMax = new Vector2(1f, anchorMax.y);
				RectTransform horizontalScrollbarRect3 = m_HorizontalScrollbarRect;
				Vector2 anchoredPosition = m_HorizontalScrollbarRect.anchoredPosition;
				horizontalScrollbarRect3.anchoredPosition = new Vector2(0f, anchoredPosition.y);
				if (vScrollingNeeded)
				{
					RectTransform horizontalScrollbarRect4 = m_HorizontalScrollbarRect;
					float x = 0f - (m_VSliderWidth + m_VerticalScrollbarSpacing);
					Vector2 sizeDelta = m_HorizontalScrollbarRect.sizeDelta;
					horizontalScrollbarRect4.sizeDelta = new Vector2(x, sizeDelta.y);
				}
				else
				{
					RectTransform horizontalScrollbarRect5 = m_HorizontalScrollbarRect;
					Vector2 sizeDelta2 = m_HorizontalScrollbarRect.sizeDelta;
					horizontalScrollbarRect5.sizeDelta = new Vector2(0f, sizeDelta2.y);
				}
			}
			if (m_HSliderExpand && (bool)m_VerticalScrollbar)
			{
				m_Tracker.Add(this, m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				RectTransform verticalScrollbarRect = m_VerticalScrollbarRect;
				Vector2 anchorMin2 = m_VerticalScrollbarRect.anchorMin;
				verticalScrollbarRect.anchorMin = new Vector2(anchorMin2.x, 0f);
				RectTransform verticalScrollbarRect2 = m_VerticalScrollbarRect;
				Vector2 anchorMax2 = m_VerticalScrollbarRect.anchorMax;
				verticalScrollbarRect2.anchorMax = new Vector2(anchorMax2.x, 1f);
				RectTransform verticalScrollbarRect3 = m_VerticalScrollbarRect;
				Vector2 anchoredPosition2 = m_VerticalScrollbarRect.anchoredPosition;
				verticalScrollbarRect3.anchoredPosition = new Vector2(anchoredPosition2.x, 0f);
				if (hScrollingNeeded)
				{
					RectTransform verticalScrollbarRect4 = m_VerticalScrollbarRect;
					Vector2 sizeDelta3 = m_VerticalScrollbarRect.sizeDelta;
					verticalScrollbarRect4.sizeDelta = new Vector2(sizeDelta3.x, 0f - (m_HSliderHeight + m_HorizontalScrollbarSpacing));
				}
				else
				{
					RectTransform verticalScrollbarRect5 = m_VerticalScrollbarRect;
					Vector2 sizeDelta4 = m_VerticalScrollbarRect.sizeDelta;
					verticalScrollbarRect5.sizeDelta = new Vector2(sizeDelta4.x, 0f);
				}
			}
		}

		private void UpdateBounds(bool updateItems = false)
		{
			m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
			m_ContentBounds = GetBounds();
			if (!(m_Content == null))
			{
				if (Application.isPlaying && updateItems && UpdateItems(m_ViewBounds, m_ContentBounds))
				{
					Canvas.ForceUpdateCanvases();
					m_ContentBounds = GetBounds();
				}
				Vector3 size = m_ContentBounds.size;
				Vector3 center = m_ContentBounds.center;
				Vector3 vector = m_ViewBounds.size - size;
				if (vector.x > 0f)
				{
					float x = center.x;
					float x2 = vector.x;
					Vector2 pivot = m_Content.pivot;
					center.x = x - x2 * (pivot.x - 0.5f);
					Vector3 size2 = m_ViewBounds.size;
					size.x = size2.x;
				}
				if (vector.y > 0f)
				{
					float y = center.y;
					float y2 = vector.y;
					Vector2 pivot2 = m_Content.pivot;
					center.y = y - y2 * (pivot2.y - 0.5f);
					Vector3 size3 = m_ViewBounds.size;
					size.y = size3.y;
				}
				m_ContentBounds.size = size;
				m_ContentBounds.center = center;
			}
		}

		private Bounds GetBounds()
		{
			if (m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = viewRect.worldToLocalMatrix;
			m_Content.GetWorldCorners(m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		private Bounds GetBounds4Item(int index)
		{
			if (m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = viewRect.worldToLocalMatrix;
			int num = index - itemTypeStart;
			if (num < 0 || num >= m_Content.childCount)
			{
				return default(Bounds);
			}
			RectTransform rectTransform = m_Content.GetChild(num) as RectTransform;
			if (rectTransform == null)
			{
				return default(Bounds);
			}
			rectTransform.GetWorldCorners(m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 zero = Vector2.zero;
			if (m_MovementType == MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = m_ContentBounds.min;
			Vector2 vector2 = m_ContentBounds.max;
			if (m_Horizontal)
			{
				vector.x += delta.x;
				vector2.x += delta.x;
				float x = vector.x;
				Vector3 min = m_ViewBounds.min;
				if (x > min.x)
				{
					Vector3 min2 = m_ViewBounds.min;
					zero.x = min2.x - vector.x;
				}
				else
				{
					float x2 = vector2.x;
					Vector3 max = m_ViewBounds.max;
					if (x2 < max.x)
					{
						Vector3 max2 = m_ViewBounds.max;
						zero.x = max2.x - vector2.x;
					}
				}
			}
			if (m_Vertical)
			{
				vector.y += delta.y;
				vector2.y += delta.y;
				float y = vector2.y;
				Vector3 max3 = m_ViewBounds.max;
				if (y < max3.y)
				{
					Vector3 max4 = m_ViewBounds.max;
					zero.y = max4.y - vector2.y;
				}
				else
				{
					float y2 = vector.y;
					Vector3 min3 = m_ViewBounds.min;
					if (y2 > min3.y)
					{
						Vector3 min4 = m_ViewBounds.min;
						zero.y = min4.y - vector.y;
					}
				}
			}
			return zero;
		}

		protected void SetDirty()
		{
			if (IsActive())
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}

		protected void SetDirtyCaching()
		{
			if (IsActive())
			{
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}

        Transform ICanvasElement.transform
        {
            get { return base.transform; }
        }

        bool ICanvasElement.IsDestroyed()
		{
			return IsDestroyed();
		}
	}
}
