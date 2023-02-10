using Nightingale.Localization;
using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
    [Serializable]
    public class PokerThemeConfig
    {
        public string identifier;

        public string fileName;

        public string name;

        public string thumbnail;

        public string themeType;

        public int order;

        public ScheduleData scheduleData;

        public string purchasId;

        public string waspurchasId;

        public int Index
        {
            get;
            set;
        }

        public bool IsCanUse()
        {
            if (PokerData.Get().purchasings.Contains(identifier))
            {
                return true;
            }
            switch (GetThemeType())
            {
                default:
                    return true;
                case ThemeType.Buy:
                    return false;
                case ThemeType.Chapter:
                    {
                        ChapterData chapter = PlayData.Get().GetChapter(scheduleData.world, scheduleData.chapter);
                        ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(scheduleData.world, scheduleData.chapter);
                        if (chapter != null && chapterConfig != null && chapter.lvs.Count == chapterConfig.LevelCount)
                        {
                            return true;
                        }
                        return false;
                    }
            }
        }

        public ThemeType GetThemeType()
        {
            return EnumUtility.GetEnumType(themeType, ThemeType.None);
        }

        public string GetDescription()
        {
            ThemeType themeType = GetThemeType();
            if (themeType == ThemeType.None || themeType == ThemeType.Buy || themeType != ThemeType.Chapter)
            {
                return "None";
            }
            ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(scheduleData.world, scheduleData.chapter);
            if (chapterConfig == null)
            {
                return "---";
            }
            return string.Format(LocalizationUtility.Get("Localization_poker.json").GetString("theme_lock_desc"), chapterConfig.name, name);
        }
    }
}
