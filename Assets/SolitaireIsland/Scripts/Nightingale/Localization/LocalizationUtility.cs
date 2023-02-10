using Nightingale.JSONUtilitys;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Nightingale.Localization
{
	public class LocalizationUtility
	{
		private static Dictionary<string, LocalizationUtility> _helper = new Dictionary<string, LocalizationUtility>();

		private Dictionary<string, string>[] dictionarys;

		public LocalizationUtility(string fileName)
		{
			fileName = $"Localization/{fileName}";
			List<object> list = Json.Deserialize(SingletonBehaviour<LoaderUtility>.Get().GetText(fileName)) as List<object>;
			dictionarys = new Dictionary<string, string>[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				dictionarys[i] = new Dictionary<string, string>();
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				foreach (string key in dictionary.Keys)
				{
					dictionarys[i].Add(key, dictionary[key].ToString());
				}
			}
		}

		public static LocalizationUtility Get(string fileName = "Localization.json")
		{
			if (!_helper.ContainsKey(fileName))
			{
				_helper.Add(fileName, new LocalizationUtility(fileName));
			}
			return _helper[fileName];
		}

		public static void Clear(string key)
		{
			if (_helper.ContainsKey(key))
			{
				_helper.Remove(key);
			}
		}

		public string[] GetKeys()
		{
			List<string> list = new List<string>();
			Dictionary<string, string>[] array = dictionarys;
			foreach (Dictionary<string, string> dictionary in array)
			{
				list.Add(dictionary["key"]);
			}
			return list.ToArray();
		}

		public static SystemLanguage GetLanguage()
		{
			SystemLanguage systemLanguage = PlatformUtility.GetLanguage();
			if (PlatformUtility.GetLanguage() == SystemLanguage.ChineseSimplified)
			{
				systemLanguage = SystemLanguage.Chinese;
			}
			if (systemLanguage == SystemLanguage.French || systemLanguage == SystemLanguage.German || systemLanguage == SystemLanguage.English || systemLanguage == SystemLanguage.Spanish)
			{
				return systemLanguage;
			}
			return SystemLanguage.English;
		}

		public static CultureInfo GetCultureInfo()
		{
			switch (GetLanguage())
			{
			default:
				return new CultureInfo("en-US");
			case SystemLanguage.French:
				return new CultureInfo("fr-FR");
			case SystemLanguage.German:
				return new CultureInfo("de-DE");
			case SystemLanguage.Spanish:
				return new CultureInfo("es-es");
			}
		}

		public string GetString(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}
			string key2 = GetLanguage().ToString();
			Dictionary<string, string>[] array = dictionarys;
			foreach (Dictionary<string, string> dictionary in array)
			{
				if (key.Equals(dictionary["key"]))
				{
					if (dictionary.ContainsKey(key2))
					{
						return dictionary[key2];
					}
					return dictionary["English"];
				}
			}
			return key;
		}
	}
}
