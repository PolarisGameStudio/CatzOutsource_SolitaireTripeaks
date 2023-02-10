using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class PokerThemeScene : SoundScene
	{
		public RectTransform contentTransform;

		public RectTransform viewPointTransform;

		private void Awake()
		{
			base.IsStay = true;
			PokerThemeGroup.Get().CalcLock();
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(InboxScene).Name, "UI/PokerThemeUI");
			GameObject selected = null;
			PokerThemeConfig[] array = (from c in PokerThemeGroup.Get().pokers
				orderby c.order
				select c).ToArray();
			PokerThemeConfig[] array2 = array;
			foreach (PokerThemeConfig pokerThemeConfig in array2)
			{
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.SetParent(contentTransform, worldPositionStays: false);
				PokerThemeUI component = gameObject.GetComponent<PokerThemeUI>();
				if (component.SetPokerThemeConfig(pokerThemeConfig))
				{
					selected = component.gameObject;
				}
			}
			DelayDo(delegate
			{
				CenterToSelected(selected);
			});
		}

		private void CenterToSelected(GameObject selected)
		{
			if (!(selected == null))
			{
				RectTransform component = selected.GetComponent<RectTransform>();
				Vector3 a = viewPointTransform.position + (Vector3)viewPointTransform.rect.center;
				Vector3 position = component.position;
				Vector3 b = a - position;
				b.z = 0f;
				Vector3 position2 = contentTransform.position + b;
				contentTransform.position = position2;
			}
		}
	}
}
