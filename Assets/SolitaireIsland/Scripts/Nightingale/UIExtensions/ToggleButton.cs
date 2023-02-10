using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	public class ToggleButton : MonoBehaviour
	{
		public Sprite SpriteOn;

		public Sprite SpriteOff;

		private Image Image;

		public UnityEvent onChanged = new UnityEvent();

		public GameObject _selectTransform;

		public bool IsOn
		{
			get;
			set;
		}

		private void Awake()
		{
			if (_selectTransform == null)
			{
				_selectTransform = base.gameObject;
			}
			Image = GetComponent<Image>();
			EventTrigger eventTrigger = _selectTransform.AddComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				SetState(!IsOn);
				onChanged.Invoke();
			});
			eventTrigger.triggers.Add(entry);
		}

		public void SetState(bool state)
		{
			IsOn = state;
			Image.sprite = ((!state) ? SpriteOff : SpriteOn);
		}
	}
}
