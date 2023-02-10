using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SelfAvaterUI : MonoBehaviour
	{
		public FriendAvaterUI FriendAvaterUI;

		private void Awake()
		{
			FriendAvaterUI.SetUser(SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf());
		}
	}
}
