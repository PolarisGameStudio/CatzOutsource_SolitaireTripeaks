using Nightingale.Localization;
using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AchievementInfo
	{
		public string identifier;

		public int CurrentCount;

		public bool IsTips;

		private AchievementConfig Config;

		private IAchievementTarget IAchievementTarget;

		public AchievementConfig GetConfig()
		{
			return Config;
		}

		public void DoAchievement(ScheduleData schedule)
		{
			if (!IsComplete() && IsActive())
			{
				if (IAchievementTarget == null)
				{
					IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
				}
				IAchievementTarget.DOAchievement(this, schedule);
			}
		}

		public void DoAchievement(int schedule)
		{
			if (!IsComplete() && IsActive())
			{
				if (IAchievementTarget == null)
				{
					IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
				}
				IAchievementTarget.DOAchievement(this, new ScheduleData(schedule, schedule, schedule));
			}
		}

		public string GetDescription()
		{
			if (IAchievementTarget == null)
			{
				IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
			}
			string text = IAchievementTarget.GetDescription(this);
			if (!string.IsNullOrEmpty(GetConfig().Date))
			{
				string arg = DateTime.Parse(GetConfig().Date).ToString("ddd, d MMM yyyy", LocalizationUtility.GetCultureInfo());
				arg = string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("On Day"), arg);
				text = $"{arg}: {text}";
			}
			return text;
		}

		public string GetTitle()
		{
			if (IAchievementTarget == null)
			{
				IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
			}
			return IAchievementTarget.GetTitle(this);
		}

		public void PutConfig(AchievementConfig config)
		{
			identifier = config.identifier;
			Config = config;
		}

		public float GetProgress()
		{
			if (IAchievementTarget == null)
			{
				IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
			}
			return (float)IAchievementTarget.GetCurrent(this) / (float)IAchievementTarget.GetTotal(this);
		}

		public string GetProgressString()
		{
			if (IAchievementTarget == null)
			{
				IAchievementTarget = (IAchievementTarget)Activator.CreateInstance(EnumUtility.GetStringType(Config.achievementType));
			}
			return $"{IAchievementTarget.GetCurrent(this)}/{IAchievementTarget.GetTotal(this)}";
		}

		public bool IsComplete()
		{
			return CurrentCount >= GetConfig().NeedCount;
		}

		public bool IsSpecial()
		{
			return !string.IsNullOrEmpty(Config.Date);
		}

		public bool IsActive()
		{
			if (!string.IsNullOrEmpty(Config.Date))
			{
				return DateTime.Parse(Config.Date).Date.Equals(DateTime.Now.Date);
			}
			return true;
		}
	}
}
