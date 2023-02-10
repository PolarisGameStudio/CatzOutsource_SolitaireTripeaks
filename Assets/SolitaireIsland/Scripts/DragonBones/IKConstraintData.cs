namespace DragonBones
{
	public class IKConstraintData : ConstraintData
	{
		public bool scaleEnabled;

		public bool bendPositive;

		public float weight;

		protected override void _OnClear()
		{
			base._OnClear();
			scaleEnabled = false;
			bendPositive = false;
			weight = 1f;
		}
	}
}
