using System.Collections.Generic;

namespace DragonBones
{
	public class ArmatureDisplayData : DisplayData
	{
		public bool inheritAnimation;

		public readonly List<ActionData> actions = new List<ActionData>();

		public ArmatureData armature;

		protected override void _OnClear()
		{
			base._OnClear();
			foreach (ActionData action in actions)
			{
				action.ReturnToPool();
			}
			type = DisplayType.Armature;
			inheritAnimation = false;
			actions.Clear();
			armature = null;
		}

		internal void AddAction(ActionData value)
		{
			actions.Add(value);
		}
	}
}
