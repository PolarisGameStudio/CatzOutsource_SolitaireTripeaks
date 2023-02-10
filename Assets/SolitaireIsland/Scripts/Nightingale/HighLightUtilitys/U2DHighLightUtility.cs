using Nightingale.Inputs;
using Nightingale.Utilitys;
using UnityEngine;

namespace Nightingale.HighLightUtilitys
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class U2DHighLightUtility : SingletonBehaviour<U2DHighLightUtility>
	{
		private void Awake()
		{
		}

		public void StartHighLightGameObject(Transform highLight)
		{
			FindObjectsWithClick.Get().enabled = false;
			base.gameObject.SetActive(value: true);
			base.transform.SetParent(highLight, worldPositionStays: false);
		}

		public void StopHighLightGameObject(bool destroy = false)
		{
			FindObjectsWithClick.Get().enabled = true;
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(null);
		}
	}
}
