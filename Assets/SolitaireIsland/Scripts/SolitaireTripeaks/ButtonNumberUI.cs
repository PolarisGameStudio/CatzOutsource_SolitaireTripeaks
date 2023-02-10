using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ButtonNumberUI : MonoBehaviour
	{
		[SerializeField]
		private ButtonType ButtonType;

		private bool isCollect;

		private void Start()
		{
			switch (ButtonType)
			{
			case ButtonType.Achievement:
				AchievementData.Get().Changed.AddListener(UpdateNumberUI);
				UpdateNumberUI();
				break;
			case ButtonType.Inbox:
				SingletonClass<InboxUtility>.Get().InboxNumberChanged.AddListener(UpdateNumberUI);
				SingletonClass<InboxUtility>.Get().UpdateNumber();
				break;
			case ButtonType.FreeCoins:
				InvokeRepeating("UpdateFreeCoinLabel", 1f, 1f);
				UpdateFreeCoinLabel();
				break;
			case ButtonType.Group:
				InvokeRepeating("UpdateGroupUI", 1f, 1f);
				break;
			}
		}

		private void OnDestroy()
		{
			switch (ButtonType)
			{
			case ButtonType.FreeCoins:
				break;
			case ButtonType.Achievement:
				AchievementData.Get().Changed.RemoveListener(UpdateNumberUI);
				break;
			case ButtonType.Inbox:
				SingletonClass<InboxUtility>.Get().InboxNumberChanged.RemoveListener(UpdateNumberUI);
				break;
			}
		}

		private void UpdateGroupUI()
		{
			bool flag = false;
			if (AchievementData.Get().GetNeedTipsAchievementCount() > 0)
			{
				flag = true;
			}
			if (!flag && PokerThemeGroup.Get().UseableCount() > 0)
			{
				flag = true;
			}
			if (!flag && QuestData.Get().GetNumber() > 0)
			{
				flag = true;
			}
			if (isCollect != flag)
			{
				isCollect = flag;
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(base.gameObject, isCollect);
			}
		}

		private void UpdateFreeCoinLabel()
		{
			bool flag = AuxiliaryData.Get().IsCollect();
			if (isCollect != flag)
			{
				isCollect = flag;
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(base.gameObject, isCollect);
			}
		}

		private void UpdateNumberUI()
		{
			SingletonBehaviour<GlobalConfig>.Get().CreateNumber(base.gameObject, 1f, AchievementData.Get().GetNeedTipsAchievementCount());
		}

		private void UpdateNumberUI(int number)
		{
			SingletonBehaviour<GlobalConfig>.Get().CreateNumber(base.gameObject, 1f, number);
		}
	}
}
