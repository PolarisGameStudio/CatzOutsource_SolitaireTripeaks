using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class DowningProgressUI : MonoBehaviour
	{
		public Image ProgressImage;

		public void SetProgress(float progress)
		{
			ProgressImage.fillAmount = progress;
		}
	}
}
