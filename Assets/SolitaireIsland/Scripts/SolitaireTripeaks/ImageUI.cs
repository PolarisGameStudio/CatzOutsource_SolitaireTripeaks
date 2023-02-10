using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ImageUI : MonoBehaviour
	{
		public Image _Image;

		public Text _Label;

		public void SetImage(Sprite sprite)
		{
			_Image.sprite = sprite;
			_Image.SetNativeSize();
		}

		public void SetNoNativeSizeImage(Sprite sprite)
		{
			_Image.sprite = sprite;
		}

		public void SetLabel(string content)
		{
			_Label.text = content;
		}
	}
}
