using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ChatItemUI : DelayBehaviour
	{
		public Text ContenLabel;

		public Text TimeLabel;

		public Text NickNameLabel;

		public FriendAvaterUI FriendAvaterUI;

		public LayoutElement LayoutElement;

		public void ScrollCellContent(Message message)
		{
			base.gameObject.SetActive(value: true);
			if (string.IsNullOrEmpty(message.Template))
			{
				ContenLabel.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				ContenLabel.supportRichText = false;
			}
			else
			{
				if (string.IsNullOrEmpty(message.Content))
				{
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
					if ("#joined".Equals(message.Template))
					{
						message.Content = localizationUtility.GetString("Chat_User_Join");
					}
					else if ("#left".Equals(message.Template))
					{
						message.Content = localizationUtility.GetString("Chat_User_Leave");
					}
					else if ("#removed".Equals(message.Template))
					{
						message.Content = localizationUtility.GetString("Chat_User_Removed");
					}
					else
					{
						string[] array = message.Template.Split('#');
						if (array.Length == 3)
						{
							if (array[1].Equals("gift"))
							{
								message.Content = string.Format(localizationUtility.GetString("Chat_Gift"), ClubStoreConfig.Get().GetTitleByGiftId(array[2]));
							}
							else if (array[1].Equals("goldenbox"))
							{
								string text = string.Empty;
								PurchasingCommodity[] commoditys = SingletonBehaviour<ClubSystemHelper>.Get().GetCommoditys(array[2]);
								if (commoditys.Length == 1)
								{
									text += $" {AppearNodeConfig.Get().GetBoosterQuestTitle(commoditys[0].boosterType)} {AppearNodeConfig.Get().GetBoosterByNumber(commoditys[0].boosterType, commoditys[0].count)}";
								}
								else
								{
									PurchasingCommodity[] commoditys2 = SingletonBehaviour<ClubSystemHelper>.Get().GetCommoditys(array[2]);
									foreach (PurchasingCommodity purchasingCommodity in commoditys2)
									{
										if (purchasingCommodity.boosterType == BoosterType.Rocket || purchasingCommodity.boosterType == BoosterType.Wild || purchasingCommodity.boosterType == BoosterType.DoubleStar)
										{
											text += $" {AppearNodeConfig.Get().GetBoosterQuestTitle(purchasingCommodity.boosterType)} {AppearNodeConfig.Get().GetBoosterByNumber(purchasingCommodity.boosterType, purchasingCommodity.count)}";
										}
									}
								}
								if (text.Length > 1)
								{
									text = text.Substring(1);
								}
								message.Content = string.Format(localizationUtility.GetString("Chat_Get_SuperTreasure"), text);
							}
							else if (array[1].Equals("role"))
							{
								int num = int.Parse(array[2]);
								if (num == 3)
								{
									message.Content = string.Format(localizationUtility.GetString("Demoted_Role_To"), SingletonBehaviour<ClubSystemHelper>.Get().GetRole(num));
								}
								else
								{
									message.Content = string.Format(localizationUtility.GetString("Promote_Role_To"), SingletonBehaviour<ClubSystemHelper>.Get().GetRole(num));
								}
							}
						}
					}
				}
				ContenLabel.color = new Color32(byte.MaxValue, 173, 69, byte.MaxValue);
				ContenLabel.supportRichText = true;
			}
			ContenLabel.text = message.Content;
			TimeLabel.text = SingletonBehaviour<ClubSystemHelper>.Get().GetTimeAgo(message.Ticks);
			NickNameLabel.text = message.Author.PlayerName;
			FriendAvaterUI.SetUser(message.Author.SocialId, message.Author.SocialPlatform, message.Author.Avatar);
			ContentSizeFitter contentSizeFitter = ContenLabel.GetComponent<ContentSizeFitter>();
			contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			DelayDo(delegate
			{
				if (ContenLabel.preferredWidth >= 750f)
				{
					contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
					RectTransform rectTransform = ContenLabel.rectTransform;
					Vector2 sizeDelta = ContenLabel.rectTransform.sizeDelta;
					rectTransform.sizeDelta = new Vector2(750f, sizeDelta.y);
				}
				DelayDo(delegate
				{
					RectTransform rectTransform2 = ContenLabel.transform.parent as RectTransform;
					RectTransform rectTransform3 = rectTransform2;
					Vector2 sizeDelta2 = ContenLabel.rectTransform.sizeDelta;
					float x = sizeDelta2.x + 40f;
					Vector2 sizeDelta3 = ContenLabel.rectTransform.sizeDelta;
					rectTransform3.sizeDelta = new Vector2(x, sizeDelta3.y + 40f);
					LayoutElement.preferredHeight = ContenLabel.preferredHeight + 150f;
				});
			});
		}
	}
}
