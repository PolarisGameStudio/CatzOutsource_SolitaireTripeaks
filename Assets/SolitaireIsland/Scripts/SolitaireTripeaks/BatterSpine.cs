using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class BatterSpine : BaseSpine
	{
		public BatterSpineItem[] batterSpineItems;

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			for (int i = 0; i < batterSpineItems.Length; i++)
			{
				batterSpineItems[i].UpdateOrderLayer(zIndex + index + i + 1);
			}
		}

		public override void PlayActivation(UnityAction unityAction)
		{
			for (int i = 0; i < batterSpineItems.Length; i++)
			{
				batterSpineItems[i].PlayActivation();
			}
			DelayDo(new WaitForSeconds(1f), unityAction);
		}

		public override void PlayDestroy(UnityAction unityAction)
		{
			for (int i = 0; i < batterSpineItems.Length; i++)
			{
				batterSpineItems[i].PlayDestroy();
			}
			DelayDo(new WaitForSeconds(1f), unityAction);
		}

		public override void PlayIndex(float index)
		{
			for (int i = 0; i < batterSpineItems.Length; i++)
			{
				if (index <= (float)i)
				{
					batterSpineItems[i].PlayDestroy();
				}
				else
				{
					batterSpineItems[i].PlayActivation();
				}
			}
		}

		public override void UpdateColor(bool white)
		{
			BatterSpineItem[] array = batterSpineItems;
			foreach (BatterSpineItem batterSpineItem in array)
			{
				batterSpineItem.UpdateColor(white);
			}
		}
	}
}
