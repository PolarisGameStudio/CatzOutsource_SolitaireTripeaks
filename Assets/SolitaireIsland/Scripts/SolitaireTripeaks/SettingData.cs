using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SettingData : SingletonData<SettingData>
	{
		public float musicVolume;

		public float soundVolume;

		public SettingData()
		{
			musicVolume = 0.5f;
			soundVolume = 0.5f;
		}
	}
}
