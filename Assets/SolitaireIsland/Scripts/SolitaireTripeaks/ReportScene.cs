using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ReportScene : BaseScene
	{
		public Button SubmitButton;

		public InputField DescriptionInputField;

		private void Start()
		{
			SubmitButton.interactable = false;
			SingletonBehaviour<GlobalConfig>.Get().SetColor(SubmitButton.transform, (!SubmitButton.interactable) ? Color.gray : Color.white);
			DescriptionInputField.onValueChanged.AddListener(delegate(string text)
			{
				SubmitButton.interactable = !string.IsNullOrEmpty(text);
				SingletonBehaviour<GlobalConfig>.Get().SetColor(SubmitButton.transform, (!SubmitButton.interactable) ? Color.gray : Color.white);
			});
			SubmitButton.onClick.AddListener(delegate
			{
				if (!string.IsNullOrEmpty(DescriptionInputField.text))
				{
					SingletonClass<MySceneManager>.Get().Close();
					SingletonBehaviour<MessageUtility>.Get().SendMessage("Admin", "Support", string.Empty, JsonUtility.ToJson(new MessageContent
					{
						title = "Support",
						description = DescriptionInputField.text
					}.Clone()));
				}
			});
		}
	}
}
