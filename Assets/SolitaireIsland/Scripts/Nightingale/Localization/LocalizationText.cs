using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.Localization
{
	public class LocalizationText : Text
	{
		[HideInInspector]
		public bool onAwake = true;

		[HideInInspector]
		public string fileName = "Localization.csv";

		[HideInInspector]
		public string key;

		protected override void Awake()
		{
			base.Awake();
			if (onAwake && Application.isPlaying)
			{
				SetText();
			}
		}

		public void SetText(params object[] args)
		{
			text = string.Format(LocalizationUtility.Get(fileName).GetString(key), args);
		}
	}
}
