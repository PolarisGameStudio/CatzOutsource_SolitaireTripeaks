namespace DragonBones
{
	public class ColorTransform
	{
		public float alphaMultiplier = 1f;

		public float redMultiplier = 1f;

		public float greenMultiplier = 1f;

		public float blueMultiplier = 1f;

		public int alphaOffset;

		public int redOffset;

		public int greenOffset;

		public int blueOffset;

		public void CopyFrom(ColorTransform value)
		{
			alphaMultiplier = value.alphaMultiplier;
			redMultiplier = value.redMultiplier;
			greenMultiplier = value.greenMultiplier;
			blueMultiplier = value.blueMultiplier;
			alphaOffset = value.alphaOffset;
			redOffset = value.redOffset;
			redOffset = value.redOffset;
			greenOffset = value.blueOffset;
		}

		public void Identity()
		{
			alphaMultiplier = (redMultiplier = (greenMultiplier = (blueMultiplier = 1f)));
			alphaOffset = (redOffset = (greenOffset = (blueOffset = 0)));
		}
	}
}
