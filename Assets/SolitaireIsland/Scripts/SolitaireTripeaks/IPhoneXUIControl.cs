using UnityEngine;

namespace SolitaireTripeaks
{
	public class IPhoneXUIControl : MonoBehaviour
	{
		public Vector3 LandscapeLeftPosition;

		public Vector3 LandscapeRightPosition;

		private ScreenOrientation LastScreenOrientation;

		private void Start()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string text = SystemInfo.deviceModel.ToString();
				if (text.Equals("iPhone10,3") || text.Equals("iPhone10,6"))
				{
					return;
				}
			}
			UnityEngine.Object.Destroy(this);
		}

		private void Update()
		{
			if (Screen.orientation != LastScreenOrientation)
			{
				RectTransform rectTransform = base.transform as RectTransform;
				LastScreenOrientation = Screen.orientation;
				switch (Screen.orientation)
				{
				case ScreenOrientation.LandscapeLeft:
					rectTransform.anchoredPosition = LandscapeLeftPosition;
					break;
				case ScreenOrientation.LandscapeRight:
					rectTransform.anchoredPosition = LandscapeRightPosition;
					break;
				}
			}
		}
	}
}
