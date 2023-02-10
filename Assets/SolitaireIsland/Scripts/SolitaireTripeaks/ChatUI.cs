using Nightingale.Extensions;
using TriPeaks.ProtoData.Club;

namespace SolitaireTripeaks
{
	public class ChatUI : DelayBehaviour
	{
		public ChatItemUI LeftChat;

		public ChatItemUI RightChat;

		private void ScrollCellContent(Message message)
		{
			base.gameObject.SetActive(value: true);
			bool flag = SolitaireTripeaksData.Get().GetPlayerId().Equals(message.Author.PlayerId);
			((!flag) ? RightChat : LeftChat).gameObject.SetActive(value: false);
			((!flag) ? LeftChat : RightChat).ScrollCellContent(message);
		}
	}
}
