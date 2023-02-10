using Nightingale.Extensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubPagesViewUI : DelayBehaviour
	{
		public LoopScrollRect loopScrollRect;

		public GameObject LoadingGameObject;

		private int pageIndex = 1;

		private bool initd;

		private List<Club> clubs = new List<Club>();

		private void Start()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().AddPageClubListener(UpdateSearchClub);
			SingletonBehaviour<ClubSystemHelper>.Get().ClubPageList(pageIndex);
			loopScrollRect.OnFullLoad.AddListener(delegate
			{
				LoadingGameObject.SetActive(value: true);
				SingletonBehaviour<ClubSystemHelper>.Get().ClubPageList(pageIndex);
			});
		}

		private void OnDestroy()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().RemovePageClubListener(UpdateSearchClub);
		}

		private void OnEnable()
		{
			if (!initd)
			{
				loopScrollRect.RefillCells();
			}
		}

		private void UpdateSearchClub(int page, List<Club> datas)
		{
			LoadingGameObject.SetActive(value: false);
			if (datas.Count == 0)
			{
				return;
			}
			datas.RemoveAll((Club d) => clubs.Find((Club c) => d.ClubId == c.ClubId) != null);
			if (datas.Count != 0)
			{
				pageIndex++;
				int count = clubs.Count;
				clubs.AddRange(datas);
				loopScrollRect.objectsToFill = clubs.ToArray();
				loopScrollRect.totalCount = clubs.Count;
				loopScrollRect.RefreshCells();
				if (!initd && base.gameObject.activeInHierarchy)
				{
					LoopDelayDo(delegate
					{
						if (loopScrollRect.gameObject.activeInHierarchy)
						{
							initd = true;
							loopScrollRect.RefillCells();
							return false;
						}
						return true;
					}, null);
				}
			}
		}
	}
}
