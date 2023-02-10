using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[RequireComponent(typeof(Button))]
	public class FacebookFriendUI : MonoBehaviour
	{
		public Image _AvatarImage;

		public Text _NickNameLabel;

		public Image _StateImage;

		public Sprite _SelectedSprite;

		public Sprite _NormalSprite;

		public Sprite _DisableSprite;

		public Sprite defaultAvater;

		public Button button;

		public FriendAvaterUI FriendAvaterUI;

		private TripeaksPlayerInView facebookUserInView;

		private void ScrollCellContent(TripeaksPlayerInView facebookUserInView)
		{
			_AvatarImage.sprite = defaultAvater;
			FriendAvaterUI.SetUser(facebookUserInView.player);
			_NickNameLabel.text = facebookUserInView.player.name;
			this.facebookUserInView = facebookUserInView;
			UpdateState();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate
			{
				facebookUserInView.IsSelect = !facebookUserInView.IsSelect;
				UpdateState();
				Object.FindObjectOfType<FriendViewUI>().UpdateViewContent();
			});
		}

		public void UpdateState()
		{
			if (facebookUserInView != null)
			{
				if (facebookUserInView.IsWait)
				{
					button.interactable = false;
					_StateImage.sprite = _DisableSprite;
				}
				else if (facebookUserInView.IsSelect)
				{
					button.interactable = true;
					_StateImage.sprite = _SelectedSprite;
				}
				else
				{
					button.interactable = true;
					_StateImage.sprite = _NormalSprite;
				}
				_StateImage.SetNativeSize();
			}
		}
	}
}
