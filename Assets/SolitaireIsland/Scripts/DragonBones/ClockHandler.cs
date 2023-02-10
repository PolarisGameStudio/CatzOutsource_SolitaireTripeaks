using UnityEngine;

namespace DragonBones
{
	internal class ClockHandler : MonoBehaviour
	{
		private void Update()
		{
			UnityFactory.factory._dragonBones.AdvanceTime(Time.deltaTime);
		}
	}
}
