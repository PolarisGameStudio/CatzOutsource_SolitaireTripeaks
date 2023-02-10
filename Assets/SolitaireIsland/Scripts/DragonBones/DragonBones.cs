using System.Collections.Generic;

namespace DragonBones
{
	public class DragonBones
	{
		public static bool yDown = true;

		public static bool debug;

		public static bool debugDraw;

		public static readonly string VERSION = "5.6.300";

		private readonly WorldClock _clock = new WorldClock();

		private readonly List<EventObject> _events = new List<EventObject>();

		private readonly List<BaseObject> _objects = new List<BaseObject>();

		private IEventDispatcher<EventObject> _eventManager;

		public WorldClock clock => _clock;

		public IEventDispatcher<EventObject> eventManager => _eventManager;

		public DragonBones(IEventDispatcher<EventObject> eventManager)
		{
			_eventManager = eventManager;
		}

		public void AdvanceTime(float passedTime)
		{
			if (_objects.Count > 0)
			{
				for (int i = 0; i < _objects.Count; i++)
				{
					BaseObject baseObject = _objects[i];
					baseObject.ReturnToPool();
				}
				_objects.Clear();
			}
			if (_events.Count > 0)
			{
				for (int j = 0; j < _events.Count; j++)
				{
					EventObject eventObject = _events[j];
					Armature armature = eventObject.armature;
					if (armature._armatureData != null)
					{
						armature.eventDispatcher.DispatchDBEvent(eventObject.type, eventObject);
						if (eventObject.type == "soundEvent")
						{
							_eventManager.DispatchDBEvent(eventObject.type, eventObject);
						}
					}
					BufferObject(eventObject);
				}
				_events.Clear();
			}
			_clock.AdvanceTime(passedTime);
		}

		public void BufferEvent(EventObject value)
		{
			if (!_events.Contains(value))
			{
				_events.Add(value);
			}
		}

		public void BufferObject(BaseObject value)
		{
			if (!_objects.Contains(value))
			{
				_objects.Add(value);
			}
		}

		public static implicit operator bool(DragonBones exists)
		{
			return exists != null;
		}
	}
}
