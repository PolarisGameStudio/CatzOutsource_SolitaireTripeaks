using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class InboxMessageUI : MonoBehaviour
	{
		public Text _Label;

		public Text _ButtonLabel;

		public Text _AmountText;

		public GameObject RewardGameObject;

		public Transform AvaterTransform;

		public Button button;

		private FacebookRequestData[] requestDatas;

		public void SetAsk(FacebookRequestData[] requestDatas)
		{
			this.requestDatas = requestDatas;
			base.transform.SetAsFirstSibling();
			LocalizationUtility utility = LocalizationUtility.Get("Localization_inbox.json");
			for (int i = 0; i < requestDatas.Length && i <= 5; i++)
			{
				CreateAvater(requestDatas[i]);
			}
			RewardGameObject.SetActive(value: false);
			if (requestDatas.Length > 1)
			{
				_Label.text = string.Format(utility.GetString("messages_friends_ask_help_desc"), requestDatas[0].fromUser.name, requestDatas.Length - 1);
			}
			else
			{
				_Label.text = string.Format(utility.GetString("messages_friend_ask_help_desc"), requestDatas[0].fromUser.name);
			}
			_ButtonLabel.text = utility.GetString("messages_btn_help");
			button.onClick.AddListener(delegate
			{
				string[] to = (from e in requestDatas
					select e.fromUser.id).ToArray();
				SingletonBehaviour<FacebookMananger>.Get().AppRequest(AskedRequestCompleted, utility.GetString("Send coins in facebook"), to, "Asked", "Pyramid Solitaire");
			});
		}

		private void AskedRequestCompleted(int number)
		{
			if (number > 0)
			{
				AchievementData.Get().DoAchievement(AchievementType.HelpFriend, requestDatas.Length);
				DestoryAnimation();
				FacebookRequestData[] array = requestDatas;
				foreach (FacebookRequestData data in array)
				{
					SingletonClass<InboxUtility>.Get().ClearAppRequest(data);
				}
			}
		}

		private void InvitedRequestCompleted(int number)
		{
			if (number > 0)
			{
				DestoryAnimation();
				FacebookRequestData[] array = requestDatas;
				foreach (FacebookRequestData data in array)
				{
					SingletonClass<InboxUtility>.Get().ClearAppRequest(data);
				}
			}
		}

		public void SetInvites(FacebookRequestData[] requestDatas)
		{
			this.requestDatas = requestDatas;
			for (int i = 0; i < requestDatas.Length && i <= 5; i++)
			{
				CreateAvater(requestDatas[i]);
			}
			base.transform.SetAsFirstSibling();
			LocalizationUtility utility = LocalizationUtility.Get("Localization_inbox.json");
			RewardGameObject.SetActive(value: false);
			_Label.text = string.Format(utility.GetString("messages_invite_you_desc"));
			_ButtonLabel.text = utility.GetString("invite_you_button");
			button.onClick.AddListener(delegate
			{
				string[] to = (from e in requestDatas
					select e.fromUser.id).ToArray();
				SingletonBehaviour<FacebookMananger>.Get().AppRequest(InvitedRequestCompleted, utility.GetString("thank invite in facebook"), to, "Invited", "Pyramid Solitaire");
			});
		}

		public void SetInvited(FacebookRequestData requestData)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_inbox.json");
			CreateAvater(requestData);
			RewardGameObject.SetActive(value: true);
			_AmountText.text = 15000.ToString();
			_Label.text = string.Format(localizationUtility.GetString("rewards_invited_friend_gift_desc"), requestData.fromUser.name);
			_ButtonLabel.text = localizationUtility.GetString("btn_collect");
			button.onClick.AddListener(delegate
			{
				SingletonBehaviour<EffectUtility>.Get().CreateCoinEffect(button.transform.position);
				SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Task, 15000L);
				AuxiliaryData.Get().InvitableFriended.Add(requestData.fromUser.id);
				DestoryAnimation();
				SingletonClass<InboxUtility>.Get().ClearAppRequest(requestData);
			});
		}

		public void SetAsked(FacebookRequestData[] requestDatas)
		{
			LocalizationUtility utility = LocalizationUtility.Get("Localization_inbox.json");
			int remain = 25 - AuxiliaryData.Get()._FacebookRewardNumber;
			for (int i = 0; i < requestDatas.Length && i <= 5; i++)
			{
				CreateAvater(requestDatas[i]);
			}
			RewardGameObject.SetActive(value: true);
			_AmountText.text = $"{3000 * requestDatas.Length}";
			if (requestDatas.Length > 1)
			{
				_Label.text = string.Format(utility.GetString("rewards_friends_gift_desc"), requestDatas[0].fromUser.name, requestDatas.Length - 1);
			}
			else
			{
				_Label.text = string.Format(utility.GetString("rewards_friend_gift_desc"), requestDatas[0].fromUser.name);
			}
			_ButtonLabel.text = utility.GetString("btn_collect");
			button.onClick.AddListener(delegate
			{
				if (remain <= 0 || remain < requestDatas.Length)
				{
					utility = LocalizationUtility.Get("Localization_popup.json");
					SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(utility.GetString("Facebook_Rewards_Limit_Title"), string.Format(utility.GetString("Facebook_Rewards_Limit_description"), 25), utility.GetString("btn_ok"), delegate
					{
						SingletonClass<MySceneManager>.Get().Close();
					});
				}
				if (remain > 0)
				{
					remain = Mathf.Min(remain, requestDatas.Length);
					AuxiliaryData.Get()._FacebookRewardNumber += remain;
					SingletonBehaviour<EffectUtility>.Get().CreateCoinEffect(button.transform.position);
					SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Task, remain * 3000);
					DestoryAnimation();
					for (int j = 0; j < remain; j++)
					{
						if (j < requestDatas.Length)
						{
							SingletonClass<InboxUtility>.Get().ClearAppRequest(requestDatas[j]);
						}
					}
				}
			});
		}

		private void DestoryAnimation()
		{
			button.onClick.RemoveAllListeners();
			base.transform.DOScaleX(0f, 0.2f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}

		private void CreateAvater(FacebookRequestData facebookRequestData)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(InboxScene).Name, "UI/Inboxs/InboxFbFriendSmallAvater"));
			gameObject.transform.SetParent(AvaterTransform, worldPositionStays: false);
			gameObject.GetComponent<FriendAvaterUI>().SetUser(facebookRequestData.fromUser);
		}
	}
}
