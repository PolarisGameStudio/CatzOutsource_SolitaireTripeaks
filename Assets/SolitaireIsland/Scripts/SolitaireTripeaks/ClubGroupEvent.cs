using System.Collections.Generic;
using TriPeaks.ProtoData.Club;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class ClubGroupEvent : UnityEvent<int, List<Club>>
	{
	}
}
