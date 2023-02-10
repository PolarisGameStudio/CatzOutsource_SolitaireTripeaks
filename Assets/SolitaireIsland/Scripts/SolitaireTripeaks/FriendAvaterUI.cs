using DG.Tweening;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class FriendAvaterUI : MonoBehaviour
	{
		public Image AvaterImage;

		public Sprite AvatarFrameSprite;

		public bool MasterBig;

		private Image AvatarFrameImage;

		public bool IsAnimation;

		private TripeaksPlayer player;

		private void OnDestroy()
		{
			if (player != null)
			{
				player.RemoveAvatarListener(GetInstanceID());
			}
		}

		private void UpdateAvatar(Sprite sprite, AvaterType avater)
		{
			if (sprite == null)
			{
				sprite = SingletonClass<AvaterUtility>.Get().GetAvater();
			}
			try
			{
				if (AvaterImage == null)
				{
					AvaterImage = GetComponent<Image>();
				}
				AvaterImage.sprite = sprite;
				ButtonLightMask buttonLightMask = AvaterImage.gameObject.GetComponent<ButtonLightMask>();
				if (MasterBig)
				{
					AvaterImage.transform.localScale = Vector3.one;
				}
				switch (avater)
				{
				case AvaterType.Social:
					if (AvatarFrameImage == null)
					{
						GameObject gameObject = new GameObject("FrameImage");
						gameObject.transform.SetParent(AvaterImage.transform, worldPositionStays: false);
						AvatarFrameImage = gameObject.AddComponent<Image>();
						AvatarFrameImage.rectTransform.sizeDelta = AvaterImage.rectTransform.sizeDelta * 1.08f;
					}
					AvatarFrameImage.sprite = AvatarFrameSprite;
					AvatarFrameImage.gameObject.SetActive(value: true);
					if (buttonLightMask != null)
					{
						buttonLightMask.SetActive(active: false);
					}
					break;
				case AvaterType.Master:
					if (MasterBig)
					{
						AvaterImage.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
					}
					if (AvatarFrameImage != null)
					{
						AvatarFrameImage.gameObject.SetActive(value: false);
					}
					if (buttonLightMask == null)
					{
						buttonLightMask = AvaterImage.gameObject.AddComponent<ButtonLightMask>();
						buttonLightMask.Speed = 0.8f;
					}
					buttonLightMask.SetActive(active: true);
					break;
				case AvaterType.Normal:
					if (AvatarFrameImage != null)
					{
						AvatarFrameImage.gameObject.SetActive(value: false);
					}
					if (buttonLightMask != null)
					{
						buttonLightMask.SetActive(active: false);
					}
					break;
				}
				if (IsAnimation)
				{
					base.transform.localScale = Vector3.zero;
					base.transform.DOScale(1f, 0.4f);
					base.transform.DORotate(Vector3.forward * 360f * 2f, 0.4f, RotateMode.FastBeyond360);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void SetUser(TripeaksPlayer user)
		{
			if (player != null)
			{
				player.RemoveAvatarListener(GetInstanceID());
			}
			player = user;
			player.AddAvatarListener(GetInstanceID(), UpdateAvatar);
		}

		public void SetUser(FacebookUser facebookUser)
		{
			SetUser(SingletonBehaviour<TripeaksPlayerHelper>.Get().CreatePlayer(facebookUser));
		}

		public void SetUser(string socialId, int socialPlatform, string avatarId)
		{
			SetUser(SingletonBehaviour<TripeaksPlayerHelper>.Get().CreatePlayer(socialId, avatarId));
		}

		public void SetUser(string avatarId)
		{
			UpdateAvatar(SingletonClass<AvaterUtility>.Get().GetAvater(avatarId), SingletonClass<AvaterUtility>.Get().GetAvaterType(avatarId));
		}
	}
}
