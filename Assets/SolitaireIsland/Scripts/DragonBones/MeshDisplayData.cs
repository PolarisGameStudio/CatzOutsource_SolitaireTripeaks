namespace DragonBones
{
	public class MeshDisplayData : DisplayData
	{
		public readonly VerticesData vertices = new VerticesData();

		public TextureData texture;

		protected override void _OnClear()
		{
			base._OnClear();
			type = DisplayType.Mesh;
			vertices.Clear();
			texture = null;
		}
	}
}
