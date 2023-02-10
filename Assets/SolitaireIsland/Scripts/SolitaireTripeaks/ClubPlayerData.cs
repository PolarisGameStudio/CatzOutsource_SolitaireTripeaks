using Nightingale.Utilitys;
using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ClubPlayerData : SingletonData<ClubPlayerData>
	{
		public List<GiftData> giftDatas;

		public void AppendGift(GiftData giftData)
		{
			if (giftDatas == null)
			{
				giftDatas = new List<GiftData>();
			}
			if (!giftDatas.Contains(giftData))
			{
				giftDatas.Add(giftData);
				FlushData();
			}
		}

		public void RemoveGift(GiftData giftData)
		{
			if (giftDatas != null && giftDatas.Contains(giftData))
			{
				giftDatas.Remove(giftData);
				FlushData();
			}
		}

		public List<GiftData> GetGiftDatas()
		{
			if (giftDatas == null)
			{
				return new List<GiftData>();
			}
			return giftDatas;
		}
	}
}
