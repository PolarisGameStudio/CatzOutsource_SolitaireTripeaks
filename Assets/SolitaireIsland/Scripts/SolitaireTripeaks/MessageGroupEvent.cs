using System.Collections.Generic;
using TriPeaks.ProtoData.Club;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class MessageGroupEvent : UnityEvent<bool, List<Message>>
	{
	}
}
