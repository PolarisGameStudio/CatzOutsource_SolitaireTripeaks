using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class StageIconHelper : SingletonBehaviour<StageIconHelper>
	{
		public Sprite[] Normals;

		public Sprite[] Clans;

		public Sprite GetSprite(int stage, bool isClan = false)
		{
			stage--;
			Sprite[] array = (!isClan) ? Normals : Clans;
			stage = Mathf.Min(stage, array.Length);
			stage = Mathf.Max(stage, 0);
			return array[stage];
		}
	}
}
