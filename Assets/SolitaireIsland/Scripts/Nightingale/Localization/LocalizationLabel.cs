using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.Localization
{
	public class LocalizationLabel : MonoBehaviour
	{
		[HideInInspector]
		public bool onAwake = true;

		[HideInInspector]
		public string fileName = "Localization.csv";

		[HideInInspector]
		public string key;

		private Text Label;

		private void Awake()
		{
			if (onAwake && Application.isPlaying)
			{
				SetText();
			}
		}

		public void SetText(params object[] args)
		{
			if (Label == null)
			{
				Label = GetComponent<Text>();
			}
			Label.text = string.Format(LocalizationUtility.Get(fileName).GetString(key), args);
		}
	}
}
