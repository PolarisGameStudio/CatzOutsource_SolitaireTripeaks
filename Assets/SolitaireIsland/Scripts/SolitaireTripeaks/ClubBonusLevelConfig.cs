using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ClubBonusLevelConfig
	{
		public string Name;

		public List<PurchasingCommodity> commoditys;

		public ClubSkill Skill;

		public int SkillNumber;

		public float SkillRatio;
	}
}
