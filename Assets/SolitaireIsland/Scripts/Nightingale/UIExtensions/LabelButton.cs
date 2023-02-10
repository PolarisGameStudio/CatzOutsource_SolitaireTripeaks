using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	public class LabelButton : MonoBehaviour
	{
		public Button Button;

		public Text Label;

		private void Awake()
		{
			if (Button == null)
			{
				Button = GetComponent<Button>();
			}
			if (Label == null)
			{
				Label = base.transform.GetComponentInChildren<Text>();
			}
		}
	}
}
