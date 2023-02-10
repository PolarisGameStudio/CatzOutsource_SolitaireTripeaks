using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[RequireComponent(typeof(Text))]
	public class SuffixText : MonoBehaviour
	{
		private float maxWidth;

		public string suffix = "...";

		private Text Label;

		private void Awake()
		{
			Label = GetComponent<Text>();
			Vector2 sizeDelta = Label.rectTransform.sizeDelta;
			maxWidth = sizeDelta.x;
		}

		private string StripLength(string input, float maxWidth)
		{
			int num = 0;
			Font font = Label.font;
			font.RequestCharactersInTexture(input, Label.fontSize, Label.fontStyle);
			CharacterInfo info = default(CharacterInfo);
			char[] array = input.ToCharArray();
			int num2 = 0;
			char[] array2 = array;
			foreach (char ch in array2)
			{
				font.GetCharacterInfo(ch, out info, Label.fontSize);
				int num3 = num + info.advance;
				if ((float)num3 > maxWidth)
				{
					UnityEngine.Debug.LogFormat("newLength {0},  totalLength {1}: ", num3, num);
					if (!(Mathf.Abs((float)num3 - maxWidth) > Mathf.Abs(maxWidth - (float)num)))
					{
						num = num3;
						num2++;
					}
					break;
				}
				num += info.advance;
				num2++;
			}
			UnityEngine.Debug.LogFormat("totalLength {0} : ", num);
			return input.Substring(0, num2 - 1);
		}

		private int CalculateLengthOfText(string message)
		{
			int num = 0;
			Label.font.RequestCharactersInTexture(message, Label.fontSize, Label.fontStyle);
			CharacterInfo info = default(CharacterInfo);
			char[] array = message.ToCharArray();
			char[] array2 = array;
			foreach (char ch in array2)
			{
				Label.font.GetCharacterInfo(ch, out info, Label.fontSize);
				num += info.advance;
			}
			return num;
		}

		private string StripLengthWithSuffix(string input)
		{
			int num = CalculateLengthOfText(input);
			UnityEngine.Debug.Log("input total length = " + num);
			if ((float)num > maxWidth)
			{
				return StripLength(input, maxWidth - (float)CalculateLengthOfText(suffix)) + suffix;
			}
			return input;
		}

		public void SetText(string text)
		{
			if (Label == null)
			{
				Label = GetComponent<Text>();
			}
			Label.text = StripLengthWithSuffix(text);
		}
	}
}
