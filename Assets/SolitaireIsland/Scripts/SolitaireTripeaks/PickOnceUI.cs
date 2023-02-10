using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PickOnceUI : MonoBehaviour
	{
		public Button NextButton;

		public Button PreButton;

		public Text Label;

		public string[] labels;

		private int index;

		private void Awake()
		{
			NextButton.onClick.AddListener(delegate
			{
				index++;
				UpdateIndex();
			});
			PreButton.onClick.AddListener(delegate
			{
				index--;
				UpdateIndex();
			});
			UpdateIndex();
		}

		private void UpdateIndex()
		{
			NextButton.interactable = (index < labels.Length - 1);
			SetColor(NextButton.transform, (!NextButton.interactable) ? Color.gray : Color.white);
			PreButton.interactable = (index > 0);
			SetColor(PreButton.transform, (!PreButton.interactable) ? Color.gray : Color.white);
			Label.text = labels[index];
		}

		private void SetColor(Transform transform, Color color)
		{
			Image[] componentsInChildren = transform.GetComponentsInChildren<Image>();
			Image[] array = componentsInChildren;
			foreach (Image image in array)
			{
				image.color = color;
			}
		}

		public void PutLabels(string[] labels, int index = 0)
		{
			this.labels = labels;
			SetIndex(index);
		}

		public void SetIndex(int dex)
		{
			index = dex;
			index = Math.Min(labels.Length, index);
			index = Math.Max(0, index);
			UpdateIndex();
		}

		public string GetString()
		{
			return labels[index];
		}

		public int GetIndex()
		{
			return index;
		}
	}
}
