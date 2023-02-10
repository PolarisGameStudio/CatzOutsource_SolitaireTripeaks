using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Localization
{
	public class FontConfig : MonoBehaviour
	{
		public List<FontData> fonts;

		private static FontConfig config;

		public static FontConfig GetDefault()
		{
			if (config == null)
			{
				config = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<FontConfig>("FontConfig");
			}
			return config;
		}

		public FontData Finder(bool force)
		{
			SystemLanguage language = LocalizationUtility.GetLanguage();
			FontData fontData = fonts.Find((FontData e) => e.Language == language);
			if (fontData == null && force)
			{
				fontData = fonts[0];
			}
			return fontData;
		}
	}
}
