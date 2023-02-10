using Nightingale.Utilitys;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Nightingale.SensitiveWords
{
	public class SensitiveWords : SingletonClass<SensitiveWords>
	{
		private static Dictionary<Regex, string> _dirtyWords;

		public SensitiveWords()
		{
			_dirtyWords = new Dictionary<Regex, string>();
			TextAsset textAsset = Resources.Load<TextAsset>("DirtyWords");
			using (MemoryStream stream = new MemoryStream(textAsset.bytes))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						if (!string.IsNullOrEmpty(text))
						{
							Regex key = ToRegexPattern(text);
							_dirtyWords[key] = new string('*', text.Length);
						}
					}
				}
			}
		}

		public string ProfanityFilter(string input)
		{
			if (_dirtyWords == null || _dirtyWords.Count == 0 || input == null)
			{
				return input;
			}
			string text = input.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			foreach (KeyValuePair<Regex, string> dirtyWord in _dirtyWords)
			{
				text = dirtyWord.Key.Replace(text, dirtyWord.Value);
			}
			return text;
		}

		public bool ProfanityCheck(string input)
		{
			if (_dirtyWords == null || _dirtyWords.Count == 0 || string.IsNullOrEmpty(input))
			{
				return false;
			}
			foreach (KeyValuePair<Regex, string> dirtyWord in _dirtyWords)
			{
				if (dirtyWord.Key.IsMatch(input))
				{
					return true;
				}
			}
			return false;
		}

		private Regex ToRegexPattern(string wildcardSearch)
		{
			string text = Regex.Escape(wildcardSearch);
			text = text.Replace("\\*", ".*?");
			text = text.Replace("\\?", ".");
			if (text.StartsWith(".*?"))
			{
				text = text.Substring(3);
				text = "(^\\b)*?" + text;
			}
			text = "\\b" + text + "\\b";
			return new Regex(text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}
	}
}
