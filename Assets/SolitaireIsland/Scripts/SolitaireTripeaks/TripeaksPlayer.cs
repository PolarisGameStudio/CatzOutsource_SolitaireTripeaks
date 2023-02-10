using Nightingale.Socials;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class TripeaksPlayer : FacebookUser, IDisposable
	{
		private string PlayerId;

		private string Avatar;

		private int Level;

		private Sprite sprite;

		private readonly Dictionary<int, UnityAction<Sprite, AvaterType>> avatarCalls = new Dictionary<int, UnityAction<Sprite, AvaterType>>();

		private readonly Dictionary<int, UnityAction> nickNameCalls = new Dictionary<int, UnityAction>();

		public bool IsSelf
		{
			get;
			private set;
		}

		public TripeaksPlayer()
		{
		}

		public TripeaksPlayer(FacebookUser user)
		{
			id = user.id;
			isPlayer = user.isPlayer;
			name = user.name;
			picture = user.picture;
		}

		public TripeaksPlayer(string facebookId, string avatar)
		{
			id = facebookId;
			Avatar = avatar;
		}

		public bool IsInChapter(int world, int chapter)
		{
			if (Level == 0)
			{
				return false;
			}
			List<ScheduleData> allScheduleDatas = UniverseConfig.Get().GetAllScheduleDatas();
			if (Level - 1 < allScheduleDatas.Count && Level > 0)
			{
				ScheduleData scheduleData = allScheduleDatas[Level - 1];
				int result;
				if (scheduleData.world == world)
				{
					ScheduleData scheduleData2 = allScheduleDatas[Level - 1];
					result = ((scheduleData2.chapter == chapter) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
				return (byte)result != 0;
			}
			return false;
		}

		public bool IsInLevele(ScheduleData schedule)
		{
			if (Level == 0)
			{
				return false;
			}
			return UniverseConfig.Get().GetLevels(schedule) + 1 == Level;
		}

		public void AddAvatarListener(int id, UnityAction<Sprite, AvaterType> unityAction)
		{
			if (unityAction != null)
			{
				RemoveAvatarListener(id);
				avatarCalls.Add(id, unityAction);
				UpdateAvatar();
			}
		}

		public void RemoveAvatarListener(int id)
		{
			if (avatarCalls.ContainsKey(id))
			{
				avatarCalls.Remove(id);
			}
		}

		private void AvatarInvoke(Sprite sprite, AvaterType type)
		{
			if (!(sprite == null))
			{
				foreach (UnityAction<Sprite, AvaterType> value in avatarCalls.Values)
				{
					value?.Invoke(sprite, type);
				}
			}
		}

		public void UpdateAvatar()
		{
			string avatar = GetAvatar();
			string facebookId = GetFacebookId();
			if (!string.IsNullOrEmpty(avatar))
			{
				AvatarInvoke(SingletonClass<AvaterUtility>.Get().GetAvater(avatar), SingletonClass<AvaterUtility>.Get().GetAvaterType(avatar));
			}
			else if (!string.IsNullOrEmpty(facebookId))
			{
				if (sprite != null)
				{
					AvatarInvoke(sprite, AvaterType.Social);
					return;
				}
				AvatarInvoke(SingletonClass<AvaterUtility>.Get().GetAvater(), AvaterType.Normal);
				if (string.IsNullOrEmpty(picture))
				{
					picture = string.Format("https://graph.facebook.com/{0}/picture?access_token={1}&width={2}&height={2}", facebookId, SingletonBehaviour<FacebookMananger>.Get().TokenString, 80);
				}
				TaskHelper.GetMiniDownload().AppendTask(new ImageTask(picture, facebookId)).RemoveAllListeners()
					.AddListener(delegate(object asset, float p)
					{
						if (asset != null)
						{
							sprite = (asset as Sprite);
							AvatarInvoke(sprite, AvaterType.Social);
						}
					});
			}
			else
			{
				AvatarInvoke(SingletonClass<AvaterUtility>.Get().GetAvater(), AvaterType.Normal);
			}
		}

		public void AddNickNameListener(int id, UnityAction unityAction)
		{
			if (unityAction != null)
			{
				RemoveNickNameListener(id);
				nickNameCalls.Add(id, unityAction);
				UpdateNickName();
			}
		}

		public void RemoveNickNameListener(int id)
		{
			if (nickNameCalls.ContainsKey(id))
			{
				nickNameCalls.Remove(id);
			}
		}

		public void UpdateNickName()
		{
			if (string.IsNullOrEmpty(AuxiliaryData.Get().NickName))
			{
				SingletonBehaviour<FacebookMananger>.Get().GetPlayerInfo(delegate(FacebookUser user)
				{
					if (string.IsNullOrEmpty(AuxiliaryData.Get().NickName))
					{
						AuxiliaryData.Get().NickName = user.name;
						SingletonBehaviour<ClubSystemHelper>.Get().Profile(AuxiliaryData.Get().GetNickName(), AuxiliaryData.Get().AvaterFileName);
						NickNameInvoke();
					}
				});
			}
			NickNameInvoke();
		}

		private void NickNameInvoke()
		{
			foreach (UnityAction value in nickNameCalls.Values)
			{
				value?.Invoke();
			}
		}

		public void UpdateBySelf()
		{
			IsSelf = true;
			UpdateNickName();
		}

		public string GetFacebookId()
		{
			if (IsSelf)
			{
				return SingletonBehaviour<FacebookMananger>.Get().UserId;
			}
			return id;
		}

		public string GetNickName()
		{
			if (IsSelf)
			{
				return AuxiliaryData.Get().GetNickName();
			}
			return name;
		}

		public string GetPlayerId()
		{
			if (IsSelf)
			{
				return SolitaireTripeaksData.Get().GetPlayerId();
			}
			return PlayerId;
		}

		public string GetAvatar()
		{
			if (IsSelf)
			{
				return AuxiliaryData.Get().AvaterFileName;
			}
			return Avatar;
		}

		public int GetLevel()
		{
			if (IsSelf)
			{
				return PlayData.Get().GetMax();
			}
			return Level;
		}

		public void UpdateInfo(string avatar, int level)
		{
			Avatar = avatar;
			Level = level;
		}

		public void UpdateInfo(FacebookUser user, string playerId, string avatar, int level)
		{
			id = user.id;
			isPlayer = user.isPlayer;
			name = user.name;
			picture = user.picture;
			PlayerId = playerId;
			Avatar = avatar;
			Level = level;
		}

		public void SetAvatar(string avatar)
		{
			Avatar = avatar;
			if (IsSelf)
			{
				AuxiliaryData.Get().AvaterFileName = avatar;
				UpdateAvatar();
			}
		}

		public void SetNickName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				base.name = name;
				if (IsSelf)
				{
					AuxiliaryData.Get().NickName = name;
					NickNameInvoke();
				}
			}
		}

		public void Dispose()
		{
			avatarCalls.Clear();
			nickNameCalls.Clear();
		}
	}
}
