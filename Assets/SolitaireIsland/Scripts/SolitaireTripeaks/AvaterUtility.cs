using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class AvaterUtility : SingletonClass<AvaterUtility>
	{
		public Sprite GetAvater(string avaterName = "7")
		{
			Sprite sprite = null;
			try
			{
				if (!string.IsNullOrEmpty(avaterName))
				{
					sprite = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>(typeof(AchievementScene).Name, $"AvaterSprites/{avaterName}");
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			if (sprite == null)
			{
				sprite = ((GetAvaterType() != AvaterType.Master) ? SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("AvaterSprites/7") : SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("AvaterSprites/22"));
			}
			return sprite;
		}

		public AvaterType GetAvaterType(string avaterName = "7")
		{
			List<string> list = new List<string>();
			list.Add("18");
			list.Add("19");
			list.Add("20");
			list.Add("21");
			list.Add("22");
			list.Add("23");
			list.Add("24");
			list.Add("25");
			list.Add("26");
			list.Add("27");
			list.Add("28");
			list.Add("29");
			list.Add("30");
			list.Add("31");
			list.Add("32");
			list.Add("34");
			list.Add("36");
			list.Add("38");
			list.Add("40");
			List<string> list2 = list;
			if (list2.Contains(avaterName))
			{
				return AvaterType.Master;
			}
			string[] array = avaterName.Split('_');
			if (array.Length >= 2 && array[0].Equals("master"))
			{
				return AvaterType.Master;
			}
			return AvaterType.Normal;
		}
	}
}
