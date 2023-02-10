namespace DragonBones
{
	public class BoundingBoxDisplayData : DisplayData
	{
		public BoundingBoxData boundingBox;

		protected override void _OnClear()
		{
			base._OnClear();
			if (boundingBox != null)
			{
				boundingBox.ReturnToPool();
			}
			type = DisplayType.BoundingBox;
			boundingBox = null;
		}
	}
}
