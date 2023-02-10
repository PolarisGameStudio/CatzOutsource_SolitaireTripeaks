using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class UIViewGroup : ScrollRect
	{
		public GroupButton[] TabContents;

		public int Index;

		protected override void Start()
		{
			base.Start();
			for (int i = 0; i < TabContents.Length; i++)
			{
				int temp = i;
				TabContents[i].Button.AddListener(delegate
				{
					SetTabIndex(temp);
				});
			}
			SetTabIndex(Index);
		}

		public void SetTabIndex(int index)
		{
			Index = index;
			for (int i = 0; i < TabContents.Length; i++)
			{
				TabContents[i].Button.SetState(i != index);
				base.horizontalNormalizedPosition = TabContents[i].horizontalNormalizedPosition;
			}
		}
	}
}
