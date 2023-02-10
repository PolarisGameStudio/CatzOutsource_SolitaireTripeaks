using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PokerData
	{
		public List<string> purchasings;

		public string currentUsePoker;

		public bool IsTips;

		public PokerData()
		{
			currentUsePoker = string.Empty;
			purchasings = new List<string>();
		}

		public static PokerData Get()
		{
			if (SolitaireTripeaksData.Get().PokerData == null)
			{
				SolitaireTripeaksData.Get().PokerData = new PokerData();
			}
			return SolitaireTripeaksData.Get().PokerData;
		}

		public void OpenPokerTheme()
		{
			IsTips = true;
		}

		public bool IsPokerThemeOpen()
		{
			return PlayData.Get().HasThanLevelData(0, 0, 7);
		}

		public string PutPoker(int index)
		{
			string text = string.Empty;
			if (PokerThemeGroup.Get().pokers.Count > index)
			{
				text = PokerThemeGroup.Get().pokers[index].identifier;
				PutPoker(text);
				currentUsePoker = text;
				PokerThemeGroup.Get().ChangePoker();
			}
			return text;
		}

		public void PutPoker(string identifier)
		{
			if (!purchasings.Contains(identifier))
			{
				purchasings.Add(identifier);
			}
		}
	}
}
