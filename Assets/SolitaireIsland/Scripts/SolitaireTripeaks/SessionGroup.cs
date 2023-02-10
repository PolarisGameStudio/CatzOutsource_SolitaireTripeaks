using Nightingale.Utilitys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SessionGroup : SingletonData<SessionGroup>
	{
		public List<SessionData> Sassions;

		private string GetRemainNumbers()
		{
			string text = string.Empty;
			IEnumerator enumerator = Enum.GetValues(typeof(BoosterType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BoosterType boosterType = (BoosterType)enumerator.Current;
					long total = PackData.Get().GetCommodity(boosterType).GetTotal();
					if (total != 0)
					{
						text += $"{boosterType.ToString()}_{total}|";
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public SessionData GetSassionData()
		{
			if (Sassions == null || Sassions.Count == 0)
			{
				return null;
			}
			return Sassions[Sassions.Count - 1];
		}

		public void SaveSassion()
		{
			try
			{
				SessionData sassionData = GetSassionData();
				if (sassionData != null)
				{
					sassionData.EndTime = DateTime.Now.Ticks;
					sassionData.RemainNumbers = GetRemainNumbers();
					FlushData();
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void NewSession()
		{
			try
			{
				if (Sassions == null)
				{
					Sassions = new List<SessionData>();
				}
				Sassions.Add(new SessionData
				{
					StartTime = DateTime.Now.Ticks,
					SassionId = Guid.NewGuid().ToString()
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}
	}
}
