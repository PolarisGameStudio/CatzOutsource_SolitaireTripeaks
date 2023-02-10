namespace DragonBones
{
	public interface IAnimatable
	{
		WorldClock clock
		{
			get;
			set;
		}

		void AdvanceTime(float passedTime);
	}
}
