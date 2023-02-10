using DG.Tweening;
using Nightingale.Extensions;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubsUI : DelayBehaviour
	{
		public InputField Input;

		public Button Button;

		public ClubSearchViewUI SearchUI;

		public ClubPagesViewUI PageUI;

		private void Awake()
		{
			PageUI.gameObject.SetActive(value: true);
			SearchUI.gameObject.SetActive(value: false);
			Input.onEndEdit.AddListener(delegate(string content)
			{
				if (string.IsNullOrEmpty(content))
				{
					PageUI.gameObject.SetActive(value: true);
					SearchUI.gameObject.SetActive(value: false);
				}
			});
			Button.onClick.AddListener(delegate
			{
				if (!string.IsNullOrEmpty(Input.text))
				{
					PageUI.gameObject.SetActive(value: false);
					SearchUI.gameObject.SetActive(value: true);
					SearchUI.SearchClub(Input.text);
					Button.interactable = false;
					Sequence sequence = DOTween.Sequence();
					sequence.AppendInterval(2f);
					sequence.OnComplete(delegate
					{
						Button.interactable = true;
					});
				}
			});
		}
	}
}
