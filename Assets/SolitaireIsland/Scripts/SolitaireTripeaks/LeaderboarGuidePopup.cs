using Nightingale.Utilitys;
using System;
using System.Collections;
using System.Collections.Generic;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class LeaderboarGuidePopup : SoundScene
	{
		public RectTransform contentTransform;

		public LeaderboarGuidePopup OnStart(bool isClan, SegmentType segment, List<RewardItem> rewards)
		{
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeaderboarGuidePopup).Name, "UI/LeaderBoardRewardItemUI");
			GameObject selected = null;
			IEnumerator enumerator = Enum.GetValues(typeof(SegmentType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SegmentType segmentType = (SegmentType)enumerator.Current;
					LeaderBoardRewardItemUI component = UnityEngine.Object.Instantiate(asset).GetComponent<LeaderBoardRewardItemUI>();
					component.transform.SetParent(contentTransform, worldPositionStays: false);
					component.SetInfo(isClan, (int)segmentType, (int)segment, rewards);
					if (segmentType == segment)
					{
						selected = component.transform.gameObject;
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
			DelayDo(delegate
			{
				if (selected != null)
				{
					CenterToSelected(selected);
				}
			});
			return this;
		}

		private void CenterToSelected(GameObject selected)
		{
			RectTransform component = selected.GetComponent<RectTransform>();
			RectTransform rectTransform = contentTransform.parent.parent as RectTransform;
			Vector3 a = rectTransform.position + (Vector3)rectTransform.rect.center;
			Vector3 position = component.position;
			Vector3 b = a - position;
			b.z = 0f;
			Vector3 position2 = contentTransform.position + b;
			if (position2.x > 0f)
			{
				position2.x = 0f;
			}
			contentTransform.position = position2;
		}
	}
}
