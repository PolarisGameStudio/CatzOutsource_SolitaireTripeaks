using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nightingale.Inputs
{
	public class FindObjectsWithClick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private List<RaiseEventHandlerData> delegates = new List<RaiseEventHandlerData>();

		private static FindObjectsWithClick instance;

		public bool IsRunning = true;

		public static FindObjectsWithClick Get()
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<FindObjectsWithClick>();
			}
			return instance;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (IsRunning)
			{
				RaycastHit[] array = Physics.RaycastAll(Camera.main.ScreenToWorldPoint(eventData.position), Vector3.forward);
				if (array.Length > 0)
				{
					Transform[] transforms = (from e in array
						select e.transform).ToArray();
					foreach (RaiseEventHandlerData @delegate in delegates)
					{
						if (@delegate.handler(transforms))
						{
							break;
						}
					}
				}
			}
		}

		public void Append(RaiseEventHandler handler, int order = 0)
		{
			RaiseEventHandlerData raiseEventHandlerData = delegates.Find((RaiseEventHandlerData e) => e.handler == handler);
			if (raiseEventHandlerData == null)
			{
				delegates.Add(new RaiseEventHandlerData
				{
					handler = handler,
					order = order
				});
                //@TODO
                var @sort_descending = from e in delegates
                             orderby e.order descending
                             select e;
			}
		}

		public void Remove(RaiseEventHandler handler)
		{
			delegates.RemoveAll((RaiseEventHandlerData e) => e.handler == handler);
		}

		public void Clear()
		{
			delegates.Clear();
		}
	}
}
