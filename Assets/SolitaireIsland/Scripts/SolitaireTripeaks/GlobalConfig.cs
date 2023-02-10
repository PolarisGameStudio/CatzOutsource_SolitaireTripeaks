using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
    public class GlobalConfig : SingletonBehaviour<GlobalConfig>
    {
        private AudioSource _backgroundAudioSource;

        private AudioSource _SceneSource;

        public float TimeScale
        {
            get;
            set;
        }

        private void Awake()
        {
            TimeScale = 1f;
        }

        public void SetColor(Transform transform, Color color)
        {
            Image[] componentsInChildren = transform.GetComponentsInChildren<Image>();
            Image[] array = componentsInChildren;
            foreach (Image image in array)
            {
                image.color = color;
            }
        }

        public void BuyDoubleBooster(UnityAction<bool> unityAction = null)
        {
            if (SessionData.Get().UseCommodity(BoosterType.DoubleStar, 1L))
            {
                RankCoinData.Get().AppendDoubleStarByThreeHours();
                MenuUITopLeft.UpdateDoubleStarRemianUI();
                if (unityAction != null)
                {
                    unityAction(arg0: false);
                }
            }
            else
            {
                PurchasingEevet PurchasingSuccess = null;
                PurchasingSuccess = delegate (string transactionID, PurchasingPackage package)
                {
                    if (package.HasBoosters(BoosterType.DoubleStar))
                    {
                        SingletonClass<MySceneManager>.Get().Close();
                        SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingSuccess);
                    }
                    if (unityAction != null)
                    {
                        unityAction(arg0: true);
                    }
                };
                SingletonBehaviour<UnityPurchasingHelper>.Get().Append(PurchasingSuccess);
                LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
                SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("buyDouble_Title"), localizationUtility.GetString("buyDouble_Description"), UnityPurchasingConfig.Get().GetLocalizedPriceString("yy_double_blue_star"), delegate (bool sure)
                {
                    if (sure)
                    {
                        SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(new PurchasingPackage
                        {
                            id = "yy_double_blue_star",
                            Type = "DoubleStar",
                            commoditys = new PurchasingCommodity[1]
                            {
                                new PurchasingCommodity
                                {
                                    boosterType = BoosterType.DoubleStar
                                }
                            }
                        });
                    }
                    else
                    {
                        SingletonClass<MySceneManager>.Get().Close();
                        SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingSuccess);
                        if (unityAction != null)
                        {
                            unityAction(arg0: false);
                        }
                    }
                }, close: true);
            }
        }

        public void PlayBackground()
        {
            if (_backgroundAudioSource == null)
            {
                _backgroundAudioSource = AudioUtility.GetMusic().PlayLoop("Audios/bg_main.mp3");
            }
            else
            {
                if (_backgroundAudioSource.isPlaying)
                {
                    return;
                }
                _backgroundAudioSource.UnPause();
                _backgroundAudioSource.DOPitch(1f, 0.5f);
            }
            if (_SceneSource != null)
            {
                _SceneSource.Stop();
                _SceneSource = null;
            }
        }

        public void PlayBackground(AudioClip clip)
        {
            if (_SceneSource != null)
            {
                AudioSource audioSource = _SceneSource;
                audioSource.DOPitch(0f, 0.5f).OnComplete(delegate
                {
                    audioSource.Stop();
                });
            }
            _SceneSource = AudioUtility.GetMusic().PlayLoop(clip);
            if (_backgroundAudioSource != null)
            {
                _backgroundAudioSource.DOPitch(0f, 0.5f).OnComplete(delegate
                {
                    _backgroundAudioSource.Pause();
                });
            }
        }

        public void ShowLoginFacebook(bool bonus, UnityAction unityAction = null)
        {
            if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
            {
                SingletonClass<MySceneManager>.Get().Popup<FriendsScene>("Scenes/FriendsScene", new JoinEffect()).AddClosedListener(unityAction);
            }
            else if (bonus)
            {
                SingletonClass<MySceneManager>.Get().Popup<FacebookLoginBonusScene>("Scenes/FacebookLoginBonusScene", new JoinEffect()).OnStart(unityAction);
            }
            else
            {
                SingletonBehaviour<FacebookMananger>.Get().Login(unityAction);
            }
        }

        public void ShowLoginOrInvitable(bool bonus, UnityAction unityAction = null)
        {
            if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
            {
                InvitableAllFriends();
            }
            else if (bonus)
            {
                Debug.Log("login fb");
                SingletonClass<MySceneManager>.Get().Popup<FacebookLoginBonusScene>("Scenes/FacebookLoginBonusScene", new JoinEffect()).OnStart(unityAction);
            }
            else
            {
                Debug.Log("login fb");
                SingletonBehaviour<FacebookMananger>.Get().Login(unityAction);
            }
        }

        public void InvitableAllFriends()
        {
            SingletonBehaviour<FacebookMananger>.Get().AppInvite(delegate (int number)
            {
                if (number > 0)
                {
                    AchievementData.Get().DoAchievement(AchievementType.InviteFriend, number);
                    TipPopupNoIconScene.ShowInviteFriends();
                }
            }, LocalizationUtility.Get("Localization_facebook.json").GetString("fb_invite_friend"), null, "Invite", "Pyramid Solitaire");
        }

        public void CreateLightArrow(Button button, JoinEffectDir dir, UnityAction unityAction = null)
        {
            Canvas canvas = button.GetComponent<Graphic>().canvas;
            GameObject MaskObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("Guide"));
            Guide component = MaskObject.GetComponent<Guide>();
            component.Target = (button.transform as RectTransform);
            component.Canvas = canvas;
            component.transform.SetParent(canvas.transform, worldPositionStays: false);
            RectTransform arrowUI = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/ArrowUI")).transform as RectTransform;
            arrowUI.SetParent(canvas.transform, worldPositionStays: false);
            Vector2 vector = canvas.transform.InverseTransformPoint(button.transform.position);
            Vector2 vector2 = vector;
            switch (dir)
            {
                case JoinEffectDir.Left:
                    {
                        arrowUI.localScale = new Vector3(-1f, 1f, 1f);
                        Vector2 a7 = vector;
                        Vector2 sizeDelta17 = arrowUI.sizeDelta;
                        float num9 = (0f - sizeDelta17.x) / 2f;
                        Vector2 sizeDelta18 = (button.transform as RectTransform).sizeDelta;
                        vector = a7 + new Vector2(num9 - sizeDelta18.x / 2f, 0f);
                        Vector2 a8 = vector2;
                        Vector2 sizeDelta19 = arrowUI.sizeDelta;
                        float num10 = (0f - sizeDelta19.x) / 2f;
                        Vector2 sizeDelta20 = (button.transform as RectTransform).sizeDelta;
                        vector2 = a8 + new Vector2(num10 - sizeDelta20.x / 2f, 0f) * 1.1f;
                        break;
                    }
                case JoinEffectDir.Right:
                    {
                        Vector2 a5 = vector;
                        Vector2 sizeDelta13 = arrowUI.sizeDelta;
                        float num7 = sizeDelta13.x / 2f;
                        Vector2 sizeDelta14 = (button.transform as RectTransform).sizeDelta;
                        vector = a5 + new Vector2(num7 + sizeDelta14.x / 2f, 0f);
                        Vector2 a6 = vector2;
                        Vector2 sizeDelta15 = arrowUI.sizeDelta;
                        float num8 = sizeDelta15.x / 2f;
                        Vector2 sizeDelta16 = (button.transform as RectTransform).sizeDelta;
                        vector2 = a6 + new Vector2(num8 + sizeDelta16.x / 2f, 0f) * 1.1f;
                        break;
                    }
                case JoinEffectDir.Top:
                    {
                        arrowUI.localEulerAngles = new Vector3(0f, 0f, 90f);
                        Vector2 a3 = vector;
                        Vector2 sizeDelta9 = arrowUI.sizeDelta;
                        float num5 = sizeDelta9.y / 2f;
                        Vector2 sizeDelta10 = (button.transform as RectTransform).sizeDelta;
                        vector = a3 + new Vector2(0f, num5 + sizeDelta10.y / 2f);
                        Vector2 a4 = vector2;
                        Vector2 sizeDelta11 = arrowUI.sizeDelta;
                        float num6 = sizeDelta11.y / 2f;
                        Vector2 sizeDelta12 = (button.transform as RectTransform).sizeDelta;
                        vector2 = a4 + new Vector2(0f, num6 + sizeDelta12.y / 2f) * 1.1f;
                        break;
                    }
                case JoinEffectDir.LeftBottom:
                    {
                        arrowUI.localEulerAngles = new Vector3(0f, 0f, 225f);
                        Vector2 a = vector;
                        Vector2 sizeDelta = arrowUI.sizeDelta;
                        float num = (0f - sizeDelta.x) / 2f;
                        Vector2 sizeDelta2 = (button.transform as RectTransform).sizeDelta;
                        float x = num - sizeDelta2.x / 2f;
                        Vector2 sizeDelta3 = arrowUI.sizeDelta;
                        float num2 = (0f - sizeDelta3.y) / 2f;
                        Vector2 sizeDelta4 = (button.transform as RectTransform).sizeDelta;
                        vector = a + new Vector2(x, num2 - sizeDelta4.y / 2f);
                        Vector2 a2 = vector2;
                        Vector2 sizeDelta5 = arrowUI.sizeDelta;
                        float num3 = (0f - sizeDelta5.x) / 2f;
                        Vector2 sizeDelta6 = (button.transform as RectTransform).sizeDelta;
                        float x2 = num3 - sizeDelta6.x / 2f;
                        Vector2 sizeDelta7 = arrowUI.sizeDelta;
                        float num4 = (0f - sizeDelta7.y) / 2f;
                        Vector2 sizeDelta8 = (button.transform as RectTransform).sizeDelta;
                        vector2 = a2 + new Vector2(x2, num4 - sizeDelta8.y / 2f) * 1.1f;
                        break;
                    }
            }
            arrowUI.anchoredPosition = vector;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(arrowUI.DOAnchorPos(vector2, 0.5f));
            sequence.Append(arrowUI.DOAnchorPos(vector, 0.5f));
            sequence.SetLoops(-1);
            UnityAction onClick = null;
            onClick = delegate
            {
                button.onClick.RemoveListener(onClick);
                UnityEngine.Object.Destroy(MaskObject);
                UnityEngine.Object.Destroy(arrowUI.gameObject);
                if (unityAction != null)
                {
                    unityAction();
                }
            };
            button.onClick.AddListener(onClick);
        }

        public void CreateNumber(GameObject gameObject, float scale, int number, bool left = false, float pointX = -30f, float pointY = -30f)
        {
            Transform transform = gameObject.transform.Find("Digital");
            LabelUI labelUI = (!(transform == null)) ? transform.GetComponentInChildren<LabelUI>() : null;
            if (labelUI == null && number > 0)
            {
                GameObject gameObject2 = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/DigitalLabelUI"));
                gameObject2.name = "Digital";
                gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
                RectTransform rectTransform = gameObject2.transform as RectTransform;
                if (left)
                {
                    rectTransform.anchoredPosition = new Vector2(20f, -20f);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(pointX, pointY);
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                }
                labelUI = gameObject2.GetComponent<LabelUI>();
                labelUI.transform.localScale = Vector3.zero;
                labelUI.transform.DOScale(scale, 0.2f);
            }
            if (labelUI != null)
            {
                if (number == 0)
                {
                    labelUI.gameObject.SetActive(value: false);
                }
                labelUI.SetString(number.ToString());
            }
        }

        public void CreateExclamationMark(GameObject gameObject, bool number, bool left = false, float scale = 1f, float pointX = -30f, float pointY = -30f)
        {
            Transform transform = gameObject.transform.Find("Digital");
            LabelUI labelUI = (!(transform == null)) ? transform.GetComponentInChildren<LabelUI>() : null;
            if (labelUI == null && number)
            {
                GameObject gameObject2 = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/DigitalLabelUI"));
                gameObject2.name = "Digital";
                gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
                RectTransform rectTransform = gameObject2.transform as RectTransform;
                if (left)
                {
                    rectTransform.anchoredPosition = new Vector2(20f, -20f);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(pointX, pointY);
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                }
                labelUI = gameObject2.GetComponent<LabelUI>();
                labelUI.SetString("!");
                labelUI.transform.localScale = Vector3.zero;
                labelUI.transform.DOScale(new Vector3(scale, scale, scale), 0.2f);
                labelUI.Label.transform.localScale = new Vector3(1.2f, 1f, 1f);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(labelUI.Label.transform.DOLocalMoveY(35f, 0.5f));
                sequence.Append(labelUI.Label.transform.DOShakeRotateZ(15f, 4, 1f));
                sequence.Append(labelUI.Label.transform.DOLocalMoveY(0f, 0.5f));
                sequence.AppendInterval(0.5f);
                sequence.SetLoops(-1);
            }
            if (labelUI != null)
            {
                labelUI.gameObject.SetActive(number);
            }
        }

        public static string GetPathByRuntimePlatform(string path)
        {
            string text = "blast";
            string text2 = "Android";
            switch (Application.platform)
            {
                default:
                    //text2 = "StandaloneWindows";
                    //text = "tripeaks";

                    text2 = "Android";
                    text = "blast";
                    
                    //return path.Replace("remote/", "").Replace("local/", "");
                    
                    break;
                case RuntimePlatform.IPhonePlayer:
                    text2 = "iOS";
                    text = "Tripeaks";
                    
                    //text = "tripeaks";
                    
                    //return path.Replace("remote/", "").Replace("local/", "");
                    
                    break;
                case RuntimePlatform.Android:
                    text2 = "Android";
                    text = "blast";

                    //text = "tripeaks";
                    //text = "Tripeaks";
                    
                    //return path.Replace("remote/", "").Replace("local/", "");
                    break;
#if !UNITY_2018_1_OR_NEWER
                case RuntimePlatform.MetroPlayerX86:
                case RuntimePlatform.MetroPlayerX64:
                case RuntimePlatform.MetroPlayerARM:
#else
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
#endif
                    text2 = "WSAPlayer";
                    text = "tripeaks";
                    break;
                case RuntimePlatform.WebGLPlayer:
                    text2 = "WebGL";
                    text = "tripeaks";
                    break;
            }
            string fullPath = $"{text}/{text2}/{path}";
            //Debug.LogError("@LOG GetPathByRuntimePlatform fullPath:" + fullPath);
            return fullPath;
        }
    }
}