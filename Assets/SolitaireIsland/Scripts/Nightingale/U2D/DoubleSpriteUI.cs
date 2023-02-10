using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.U2D
{
	public class DoubleSpriteUI : MonoBehaviour
	{
		public Sprite NormalSprite;

		public Sprite GraySprite;

		public bool State;

		public bool nativeSize = true;

		public Image Image;

		public void SetState(bool normal)
		{
			State = normal;
			if (Image == null)
			{
				Image = GetComponent<Image>();
			}
			Image.sprite = ((!normal) ? GraySprite : NormalSprite);
			if (nativeSize)
			{
				Image.SetNativeSize();
			}
		}
	}
}
