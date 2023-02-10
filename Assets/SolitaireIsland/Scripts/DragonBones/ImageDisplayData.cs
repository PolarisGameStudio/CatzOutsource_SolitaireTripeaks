namespace DragonBones
{
	public class ImageDisplayData : DisplayData
	{
		public readonly Point pivot = new Point();

		public TextureData texture;

		protected override void _OnClear()
		{
			base._OnClear();
			type = DisplayType.Image;
			pivot.Clear();
			texture = null;
		}
	}
}
