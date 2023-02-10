using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ClubStoreScene : SoundScene
	{
		public Transform ContentTransform;

		private void Awake()
		{
			base.IsStay = true;
			List<ClubStoreItemUI> list = ContentTransform.GetComponentsInChildren<ClubStoreItemUI>().ToList();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].SetInfo(ClubStoreConfig.Get().GetConfig(i));
			}
		}
	}
}
