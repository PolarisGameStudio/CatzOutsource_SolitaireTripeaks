using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.Extensions
{
	public class LoopScrollbarExtension : MonoBehaviour
	{
		public Transform m_itemParent;

		public GameObject m_item;

		public Scrollbar m_sb;

		public int m_itemWidth;

		public int m_itemHeight;

		public int m_fixedColumnCount;

		public int m_fixedColCount;

		private Vector2 m_allItemArea = Vector2.zero;

		private Vector2 m_showArea = Vector2.zero;

		private Vector2 m_firstItemPos = Vector2.zero;

		private Vector2 m_lastItemPos = Vector2.zero;

		private int m_listMaxLength;

		private int m_curShowStartIndex;

		private int m_curShowEndIndex;

		private UpdateLoopScrollbarItem m_updateItem;

		private float m_curSbVal;

		public void Init(int maxLength, UpdateLoopScrollbarItem updateItem)
		{
			m_showArea = GetComponent<RectTransform>().sizeDelta;
			m_item.SetActive(value: false);
			m_firstItemPos.y += m_itemHeight;
			if (m_sb != null)
			{
				m_curSbVal = m_sb.value;
			}
			m_curShowEndIndex = m_fixedColCount;
			m_listMaxLength = maxLength;
			m_updateItem = updateItem;
			for (int i = 0; i < m_fixedColCount + 1; i++)
			{
				Transform item = CreateItem(i);
				m_updateItem(item, i);
			}
		}

		private Transform CreateItem(int index)
		{
			Transform transform = UnityEngine.Object.Instantiate(m_item).transform;
			transform.gameObject.SetActive(value: true);
			transform.SetParent(m_itemParent);
			transform.name = index.ToString();
			int num = index / m_fixedColumnCount;
			int num2 = index % m_fixedColumnCount;
			transform.localPosition = new Vector3(num2 * m_itemWidth, -1 * num * m_itemHeight, 0f);
			m_allItemArea.y = (num + 1) * m_itemHeight;
			m_lastItemPos.y = -1 * (int)m_allItemArea.y;
			return transform;
		}

		public void OnDragSlider(float val)
		{
			if (m_sb != null)
			{
				UpdateListByFloat(m_sb.value * (float)(m_listMaxLength - m_fixedColCount));
			}
		}

		public void UpdateListByFloat(float val)
		{
			UpdateListPos(val);
			if (val > m_curSbVal)
			{
				if (m_curShowEndIndex >= m_listMaxLength - 1)
				{
					return;
				}
				UpdateItemPos(isDown: true);
			}
			else
			{
				if (m_curShowStartIndex <= 0)
				{
					return;
				}
				UpdateItemPos(isDown: false);
			}
			m_curSbVal = val;
		}

		private void UpdateListPos(float val)
		{
			float num = 0f;
			if (m_allItemArea.y > m_showArea.y)
			{
				num = m_allItemArea.y - m_showArea.y;
			}
			m_itemParent.localPosition = new Vector2(0f, num * val);
		}

		private void UpdateItemPos(bool isDown)
		{
			if (isDown)
			{
				for (int i = 0; i < m_itemParent.childCount; i++)
				{
					Transform child = m_itemParent.GetChild(i);
					Vector3 localPosition = child.localPosition;
					float y = localPosition.y;
					Vector3 localPosition2 = m_itemParent.localPosition;
					float num = y + localPosition2.y;
					if (num > (float)m_itemHeight)
					{
						child.localPosition = new Vector3(0f, m_lastItemPos.y, 0f);
						m_lastItemPos.y -= m_itemHeight;
						m_firstItemPos.y -= m_itemHeight;
						m_updateItem(child, m_curShowEndIndex + 1);
						m_curShowStartIndex++;
						m_curShowEndIndex++;
					}
				}
				return;
			}
			for (int num2 = m_itemParent.childCount - 1; num2 >= 0; num2--)
			{
				Transform child2 = m_itemParent.GetChild(num2);
				Vector3 localPosition3 = child2.localPosition;
				float y2 = localPosition3.y;
				Vector3 localPosition4 = m_itemParent.localPosition;
				float num3 = y2 + localPosition4.y;
				if (num3 < -1f * m_showArea.y)
				{
					child2.localPosition = new Vector3(0f, m_firstItemPos.y, 0f);
					m_firstItemPos.y += m_itemHeight;
					m_lastItemPos.y += m_itemHeight;
					m_updateItem(child2, m_curShowStartIndex - 1);
					m_curShowEndIndex--;
					m_curShowStartIndex--;
				}
			}
		}
	}
}
