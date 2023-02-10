using Nightingale.Extensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubSearchViewUI : DelayBehaviour
	{
		public LoopScrollRect loopScrollRect;

		public GameObject LoadingGameObject;

		public GameObject NoResultGameObject;

		private void Start()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().AddSearchClubListener(UpdateSearchClub);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveSearchClubListener(UpdateSearchClub);
		}

		public void SearchClub(string filter)
		{
			SingletonBehaviour<ClubSystemHelper>.Get().SearchClub(filter);
			LoadingGameObject.SetActive(value: true);
			NoResultGameObject.SetActive(value: false);
		}

		private void UpdateSearchClub(int page, List<Club> datas)
		{
			NoResultGameObject.SetActive(datas.Count == 0);
			LoadingGameObject.SetActive(value: false);
			loopScrollRect.objectsToFill = datas.ToArray();
			loopScrollRect.totalCount = datas.Count;
			loopScrollRect.RefreshCells();
			if (base.gameObject.activeInHierarchy)
			{
				LoopDelayDo(delegate
				{
					if (loopScrollRect.gameObject.activeInHierarchy)
					{
						loopScrollRect.RefillCells();
						return false;
					}
					return true;
				}, null);
			}
		}
	}
}
