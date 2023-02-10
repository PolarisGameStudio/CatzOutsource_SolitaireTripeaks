using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class WorldClock : IAnimatable
	{
		public float time;

		public float timeScale = 1f;

		private float _systemTime;

		private readonly List<IAnimatable> _animatebles = new List<IAnimatable>();

		private WorldClock _clock;

		[Obsolete("")]
		public WorldClock clock
		{
			get
			{
				return _clock;
			}
			set
			{
				if (_clock != value)
				{
					if (_clock != null)
					{
						_clock.Remove(this);
					}
					_clock = value;
					if (_clock != null)
					{
						_clock.Add(this);
					}
				}
			}
		}

		public WorldClock(float time = -1f)
		{
			this.time = time;
			_systemTime = (float)DateTime.Now.Ticks * 0.01f * 0.001f;
		}

		public void AdvanceTime(float passedTime)
		{
			if (float.IsNaN(passedTime))
			{
				passedTime = 0f;
			}
			float num = (float)DateTime.Now.Ticks * 0.01f * 0.001f;
			if (passedTime < 0f)
			{
				passedTime = num - _systemTime;
			}
			_systemTime = num;
			if (timeScale != 1f)
			{
				passedTime *= timeScale;
			}
			if (passedTime == 0f)
			{
				return;
			}
			if (passedTime < 0f)
			{
				time -= passedTime;
			}
			else
			{
				time += passedTime;
			}
			int i = 0;
			int num2 = 0;
			int count;
			for (count = _animatebles.Count; i < count; i++)
			{
				IAnimatable animatable = _animatebles[i];
				if (animatable != null)
				{
					if (num2 > 0)
					{
						_animatebles[i - num2] = animatable;
						_animatebles[i] = null;
					}
					animatable.AdvanceTime(passedTime);
				}
				else
				{
					num2++;
				}
			}
			if (num2 <= 0)
			{
				return;
			}
			for (count = _animatebles.Count; i < count; i++)
			{
				IAnimatable animatable2 = _animatebles[i];
				if (animatable2 != null)
				{
					_animatebles[i - num2] = animatable2;
				}
				else
				{
					num2++;
				}
			}
			_animatebles.ResizeList(count - num2);
		}

		public bool Contains(IAnimatable value)
		{
			if (value == this)
			{
				return false;
			}
			IAnimatable animatable = value;
			while (animatable != this && animatable != null)
			{
				animatable = animatable.clock;
			}
			return animatable == this;
		}

		public void Add(IAnimatable value)
		{
			if (value != null && !_animatebles.Contains(value))
			{
				_animatebles.Add(value);
				value.clock = this;
			}
		}

		public void Remove(IAnimatable value)
		{
			int num = _animatebles.IndexOf(value);
			if (num >= 0)
			{
				_animatebles[num] = null;
				value.clock = null;
			}
		}

		public void Clear()
		{
			int i = 0;
			for (int count = _animatebles.Count; i < count; i++)
			{
				IAnimatable animatable = _animatebles[i];
				_animatebles[i] = null;
				if (animatable != null)
				{
					animatable.clock = null;
				}
			}
		}
	}
}
