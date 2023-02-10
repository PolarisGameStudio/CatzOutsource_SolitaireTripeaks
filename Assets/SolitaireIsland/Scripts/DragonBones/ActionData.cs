namespace DragonBones
{
	public class ActionData : BaseObject
	{
		public ActionType type;

		public string name;

		public BoneData bone;

		public SlotData slot;

		public UserData data;

		protected override void _OnClear()
		{
			if (data != null)
			{
				data.ReturnToPool();
			}
			type = ActionType.Play;
			name = string.Empty;
			bone = null;
			slot = null;
			data = null;
		}
	}
}
