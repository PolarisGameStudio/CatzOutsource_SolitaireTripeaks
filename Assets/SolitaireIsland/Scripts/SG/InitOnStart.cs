using UnityEngine;
using UnityEngine.UI;

namespace SG
{
	[RequireComponent(typeof(LoopScrollRect))]
	[DisallowMultipleComponent]
	public class InitOnStart : MonoBehaviour
	{
		public int totalCount = -1;

		private void Start()
		{
			LoopScrollRect component = GetComponent<LoopScrollRect>();
			component.totalCount = totalCount;
			component.RefillCells();
		}
	}
}
