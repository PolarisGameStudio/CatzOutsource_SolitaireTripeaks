using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class MultipleImageUI : MonoBehaviour
	{
		public Image Image;

		public Sprite[] Sprites;

		public void SetImage(int point)
		{
			if (point <= Sprites.Length - 1)
			{
				Image.sprite = Sprites[point];
			}
		}
	}
}
