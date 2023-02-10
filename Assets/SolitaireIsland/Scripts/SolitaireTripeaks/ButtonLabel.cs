namespace SolitaireTripeaks
{
	public class ButtonLabel
	{
		public string label;

		public string prefab;

		public ButtonLabel(string label)
			: this(label, "ui/Inboxs/inbox_greenButton")
		{
		}

		public ButtonLabel(string label, string prefab)
		{
			this.label = label;
			this.prefab = prefab;
		}
	}
}
