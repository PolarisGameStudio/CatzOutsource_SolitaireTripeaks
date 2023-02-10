using UnityEngine;

namespace Nightingale.UIExtensions
{
	public class TabGroup : MonoBehaviour
	{
		public TabContent[] TabContents;

		public int Index;

		private void Start()
		{
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
				TabContents[i].Transform.gameObject.SetActive(i == index);
			}
		}

		public void SetVisable(bool visable)
		{
			TabContent[] tabContents = TabContents;
			foreach (TabContent tabContent in tabContents)
			{
				tabContent.Button.gameObject.SetActive(visable);
			}
		}

		public void ShowOnce(int index)
		{
			Index = index;
			for (int i = 0; i < TabContents.Length; i++)
			{
				TabContents[i].Button.SetState(i != index);
				TabContents[i].Button.gameObject.SetActive(i == index);
				TabContents[i].Transform.gameObject.SetActive(i == index);
			}
		}
	}
}
