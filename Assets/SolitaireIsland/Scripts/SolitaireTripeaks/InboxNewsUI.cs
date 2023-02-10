using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
    public class InboxNewsUI : MonoBehaviour
    {
        public Transform buttonTransform;

        public Transform PrefabTransform;

        public Image Icon;

        public Text TitleLabel;

        public Text DescriptionLabel;

        public ImageUI RewardImageUI;

        public NewsConfig Config { get; private set; }

        public void SetNewsConfig(NewsConfig newsConfig)
        {
            Config = newsConfig;
            TitleLabel.text = newsConfig.title;
            DescriptionLabel.text = newsConfig.description;
            Icon.sprite = newsConfig.GetIcon();
            RewardImageUI.gameObject.SetActive(newsConfig.rewardSprite != null);
            RewardImageUI.SetImage(newsConfig.rewardSprite);
            if (newsConfig.rewardCount >= 1000)
            {
                RewardImageUI.SetLabel($"x{newsConfig.rewardCount / 1000}K");
            }
            else if (newsConfig.rewardCount > 0)
            {
                RewardImageUI.SetLabel($"x{newsConfig.rewardCount}");
            }
            else
            {
                RewardImageUI.SetLabel(string.Empty);
            }

            for (int i = 0; i < buttonTransform.childCount; i++)
            {
                buttonTransform.GetChild(i).gameObject.SetActive(value: false);
            }

            if (newsConfig.buttons != null)
            {
                for (int j = 0; j < newsConfig.buttons.Count; j++)
                {
                    int ind = j;
                    ButtonLabel buttonLabel = newsConfig.buttons[ind];
                    GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get()
                        .GetAsset<GameObject>(typeof(InboxScene).Name, buttonLabel.prefab));
                    Button component = gameObject.GetComponent<Button>();
                    component.GetComponentInChildren<Text>().text = buttonLabel.label;
                    gameObject.transform.SetParent(buttonTransform, worldPositionStays: false);
                    component.onClick.RemoveAllListeners();
                    component.onClick.AddListener(delegate { newsConfig.Run(this, ind); });
                }
            }

            RectTransform rectTransform = DescriptionLabel.transform as RectTransform;
            int num = 719;
            if (newsConfig.buttons != null)
            {
                if (newsConfig.buttons.Count == 1)
                {
                    num = 719;
                }
                else if (newsConfig.buttons.Count == 2)
                {
                    num = 570;
                }
                else if (newsConfig.buttons.Count >= 3)
                {
                    num = 500;
                }
            }

            RectTransform rectTransform2 = rectTransform;
            float x = num;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            rectTransform2.sizeDelta = new Vector2(x, sizeDelta.y);
            for (int k = 0; k < PrefabTransform.childCount; k++)
            {
                PrefabTransform.GetChild(k).gameObject.SetActive(value: false);
            }

            if (newsConfig.prefabs != null)
            {
                foreach (PrefabLabel prefab in newsConfig.prefabs)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get()
                        .GetAsset<GameObject>(prefab.prefab));
                    gameObject2.transform.SetParent(PrefabTransform, worldPositionStays: false);
                    gameObject2.transform.localPosition = prefab.position;
                }
            }

            if (!string.IsNullOrEmpty(newsConfig.identifier))
            {
                SingletonData<DeviceFileData>.Get().Append(newsConfig.identifier);
                SingletonClass<InboxUtility>.Get().UpdateNumber();
            }
        }

        public void UpdateUI()
        {
            SetNewsConfig(Config);
        }

        public void RemoveAllListeners()
        {
            Button[] componentsInChildren = buttonTransform.GetComponentsInChildren<Button>();
            Button[] array = componentsInChildren;
            foreach (Button button in array)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        public void DestroyUI()
        {
            RemoveAllListeners();
            base.transform.DOScaleX(0f, 0.2f).OnComplete(delegate { UnityEngine.Object.Destroy(base.gameObject); });
        }

        public void ShowWatchVideo()
        {
            IronSourceManager.Instance.ShowRewardedAds(WatchAdComeleted);
        }

        private void WatchAdComeleted()
        {
            DestroyUI();
            AuxiliaryData.Get().PutDailyCompleted("InboxWatchVideoAd");
            AuxiliaryData.Get().WatchVideoCount++;
            AuxiliaryData.Get().WatchVideoTotal++;
            SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Video, 1000L, changed: false);
            TipPopupIconNumberScene.ShowVideoRewardCoins(1000);
            SingletonClass<InboxUtility>.Get().UpdateNumber();
        }
    }
}