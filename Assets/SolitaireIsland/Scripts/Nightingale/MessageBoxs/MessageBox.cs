using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.MessageBoxs
{
	public class MessageBox : BaseScene
	{
		public Text titleLabel;

		public Text descriptionLabel;

		public Button leftButton;

		public Button rightButton;

		public Text leftButtonLabel;

		public Text rightButtonLabel;

		public void OnStart(MessageBoxU messageBoxU)
		{
			if (messageBoxU != null)
			{
				titleLabel.text = messageBoxU.title;
				descriptionLabel.text = messageBoxU.description;
				leftButtonLabel.text = messageBoxU.leftButtonText;
				rightButtonLabel.text = messageBoxU.rightButtonText;
				leftButton.onClick.AddListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Close();
					messageBoxU.unityAction(0);
				});
				rightButton.onClick.AddListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Close();
					messageBoxU.unityAction(1);
				});
			}
		}

		public static void Show(string title, string description, string leftButtonText, string rightButtonText, UnityAction<int> unityAction)
		{
			MessageBox messageBox = SingletonClass<MySceneManager>.Get().Popup<MessageBox>("MessageBox");
			messageBox.OnStart(new MessageBoxU
			{
				title = title,
				description = description,
				leftButtonText = leftButtonText,
				rightButtonText = rightButtonText,
				unityAction = unityAction
			});
		}

		public static void Show(string title, string description, UnityAction<int> unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Nightingale.json");
			Show(title, description, localizationUtility.GetString("Sure"), localizationUtility.GetString("Cancel"), unityAction);
		}
	}
}
