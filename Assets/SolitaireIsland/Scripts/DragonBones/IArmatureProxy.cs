namespace DragonBones
{
	public interface IArmatureProxy : IEventDispatcher<EventObject>
	{
		Armature armature
		{
			get;
		}

		Animation animation
		{
			get;
		}

		void DBInit(Armature armature);

		void DBClear();

		void DBUpdate();

		void Dispose(bool disposeProxy);
	}
}
