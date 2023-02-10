using System.Collections.Generic;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class TopPlayerEvent : UnityEvent<List<TopPlayer>>
	{
	}
}
