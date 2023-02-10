using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[AddComponentMenu("UI/LinkImageText", 10)]
	public class LinkImageText : Text, IPointerClickHandler, IEventSystemHandler
	{
		[Serializable]
		public class HrefClickEvent : UnityEvent<string>
		{
		}

		private class HrefInfo
		{
			public int startIndex;

			public int endIndex;

			public string name;

			public readonly List<Rect> boxes = new List<Rect>();
		}

		private string m_OutputText;

		protected readonly List<Image> m_ImagesPool = new List<Image>();

		private readonly List<int> m_ImagesVertexIndex = new List<int>();

		private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

		protected static readonly StringBuilder s_TextBuilder = new StringBuilder();

		[SerializeField]
		private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

		private static readonly Regex s_ImageRegex = new Regex("<quad name=(.+?) size=(\\d*\\.?\\d+%?) width=(\\d*\\.?\\d+%?) />", RegexOptions.Singleline);

		private static readonly Regex s_HrefRegex = new Regex("<a href=([^>\\n\\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

		public HrefClickEvent onHrefClick
		{
			get
			{
				return m_OnHrefClick;
			}
			set
			{
				m_OnHrefClick = value;
			}
		}

		public override void SetVerticesDirty()
		{
			base.SetVerticesDirty();
			UpdateQuadImage();
		}

		protected void UpdateQuadImage()
		{
			m_OutputText = GetOutputText(text);
			m_ImagesVertexIndex.Clear();
			IEnumerator enumerator = s_ImageRegex.Matches(m_OutputText).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					int index = match.Index;
					int item = index * 4 + 3;
					m_ImagesVertexIndex.Add(item);
					m_ImagesPool.RemoveAll((Image image) => image == null);
					if (m_ImagesPool.Count == 0)
					{
						GetComponentsInChildren(m_ImagesPool);
					}
					if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
					{
						GameObject gameObject = DefaultControls.CreateImage(default(DefaultControls.Resources));
						gameObject.layer = base.gameObject.layer;
						RectTransform rectTransform = gameObject.transform as RectTransform;
						if ((bool)rectTransform)
						{
							rectTransform.SetParent(base.rectTransform);
							rectTransform.localPosition = Vector3.zero;
							rectTransform.localRotation = Quaternion.identity;
							rectTransform.localScale = Vector3.one;
						}
						m_ImagesPool.Add(gameObject.GetComponent<Image>());
					}
					string value = match.Groups[1].Value;
					float num = float.Parse(match.Groups[2].Value);
					float y = num;
					Image image2 = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
					if (image2.sprite == null || image2.sprite.name != value)
					{
						image2.sprite = LinkImageConfig.GetDefault().GetSprite(value);
					}
					if (image2.sprite != null)
					{
						y = num / image2.sprite.rect.width * image2.sprite.rect.height;
					}
					image2.rectTransform.sizeDelta = new Vector2(num, y);
					image2.enabled = true;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			for (int i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
			{
				if ((bool)m_ImagesPool[i])
				{
					m_ImagesPool[i].enabled = false;
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			string text = m_Text;
			m_Text = m_OutputText;
			base.OnPopulateMesh(toFill);
			m_Text = text;
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < m_ImagesVertexIndex.Count; i++)
			{
				int num = m_ImagesVertexIndex[i];
				RectTransform rectTransform = m_ImagesPool[i].rectTransform;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				if (num < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref vertex, num);
					rectTransform.anchoredPosition = new Vector2(vertex.position.x + sizeDelta.x / 2f, vertex.position.y + sizeDelta.y / 2f);
					toFill.PopulateUIVertex(ref vertex, num - 3);
					Vector3 position = vertex.position;
					int num2 = num;
					int num3 = num - 3;
					while (num2 > num3)
					{
						toFill.PopulateUIVertex(ref vertex, num);
						vertex.position = position;
						toFill.SetUIVertex(vertex, num2);
						num2--;
					}
				}
			}
			if (m_ImagesVertexIndex.Count != 0)
			{
				m_ImagesVertexIndex.Clear();
			}
			foreach (HrefInfo hrefInfo in m_HrefInfos)
			{
				hrefInfo.boxes.Clear();
				if (hrefInfo.startIndex < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref vertex, hrefInfo.startIndex);
					Vector3 position2 = vertex.position;
					Bounds bounds = new Bounds(position2, Vector3.zero);
					int j = hrefInfo.startIndex;
					for (int endIndex = hrefInfo.endIndex; j < endIndex && j < toFill.currentVertCount; j++)
					{
						toFill.PopulateUIVertex(ref vertex, j);
						position2 = vertex.position;
						float x = position2.x;
						Vector3 min = bounds.min;
						if (x < min.x)
						{
							hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
							bounds = new Bounds(position2, Vector3.zero);
						}
						else
						{
							bounds.Encapsulate(position2);
						}
					}
					hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
				}
			}
		}

		protected virtual string GetOutputText(string outputText)
		{
			s_TextBuilder.Length = 0;
			m_HrefInfos.Clear();
			int num = 0;
			IEnumerator enumerator = s_HrefRegex.Matches(outputText).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					s_TextBuilder.Append(outputText.Substring(num, match.Index - num));
					s_TextBuilder.Append("<color=blue>");
					Group group = match.Groups[1];
					HrefInfo hrefInfo = new HrefInfo();
					hrefInfo.startIndex = s_TextBuilder.Length * 4;
					hrefInfo.endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
					hrefInfo.name = group.Value;
					HrefInfo item = hrefInfo;
					m_HrefInfos.Add(item);
					s_TextBuilder.Append(match.Groups[2].Value);
					s_TextBuilder.Append("</color>");
					num = match.Index + match.Length;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			s_TextBuilder.Append(outputText.Substring(num, outputText.Length - num));
			return s_TextBuilder.ToString();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
			foreach (HrefInfo hrefInfo in m_HrefInfos)
			{
				List<Rect> boxes = hrefInfo.boxes;
				for (int i = 0; i < boxes.Count; i++)
				{
					if (boxes[i].Contains(localPoint))
					{
						m_OnHrefClick.Invoke(hrefInfo.name);
						return;
					}
				}
			}
		}
	}
}
