using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class RankRewardData
	{
		public string id;

		public int rank;

		public SegmentType CurrentStage;

		public SegmentType NextStage;

		public bool reward;

		public bool syn;

		public bool upload;

		public List<PurchasingCommodity> commoditys;
	}
}
