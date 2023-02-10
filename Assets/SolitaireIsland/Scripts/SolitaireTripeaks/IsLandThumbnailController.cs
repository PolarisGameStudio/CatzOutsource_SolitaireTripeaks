using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class IsLandThumbnailController : MonoBehaviour
	{
		public bool IsVisable
		{
			get;
			private set;
		}

		public void SetLock(bool isLock)
		{
			IsVisable = true;
			Transform transform = base.transform.Find("line");
			if (transform != null)
			{
				IslandLineEffect component = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/Island_Line")).GetComponent<IslandLineEffect>();
				component.transform.SetParent(transform, worldPositionStays: false);
				component.SetLock(isLock);
			}
		}
	}
}
