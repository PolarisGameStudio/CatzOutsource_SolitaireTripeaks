using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class NumberUI : MonoBehaviour
	{
		public Image image;

		public Text label;

		public void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
		}

		public void SetNumber(int number)
		{
			label.text = number + string.Empty;
			base.gameObject.SetActive(number > 0);
		}

		public void SetNumber(long number)
		{
			label.text = number + string.Empty;
			base.gameObject.SetActive(number > 0);
		}

		public void SetNumber(string format, long number)
		{
			label.text = string.Format(format, number);
			base.gameObject.SetActive(number > 0);
		}
	}
}
