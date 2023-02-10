using System.Collections.Generic;

namespace DragonBones
{
	public abstract class TextureAtlasData : BaseObject
	{
		public bool autoSearch;

		public uint width;

		public uint height;

		public float scale;

		public string name;

		public string imagePath;

		public readonly Dictionary<string, TextureData> textures = new Dictionary<string, TextureData>();

		public TextureAtlasData()
		{
		}

		protected override void _OnClear()
		{
			foreach (TextureData value in textures.Values)
			{
				value.ReturnToPool();
			}
			autoSearch = false;
			width = 0u;
			height = 0u;
			scale = 1f;
			textures.Clear();
			name = string.Empty;
			imagePath = string.Empty;
		}

		public void CopyFrom(TextureAtlasData value)
		{
			autoSearch = value.autoSearch;
			scale = value.scale;
			width = value.width;
			height = value.height;
			name = value.name;
			imagePath = value.imagePath;
			foreach (TextureData value2 in textures.Values)
			{
				value2.ReturnToPool();
			}
			textures.Clear();
			foreach (KeyValuePair<string, TextureData> texture in value.textures)
			{
				TextureData textureData = CreateTexture();
				textureData.CopyFrom(texture.Value);
				textures[texture.Key] = textureData;
			}
		}

		public abstract TextureData CreateTexture();

		public void AddTexture(TextureData value)
		{
			if (value != null)
			{
				if (textures.ContainsKey(value.name))
				{
					Helper.Assert(condition: false, "Same texture: " + value.name);
					textures[value.name].ReturnToPool();
				}
				value.parent = this;
				textures[value.name] = value;
			}
		}

		public TextureData GetTexture(string name)
		{
			return (!textures.ContainsKey(name)) ? null : textures[name];
		}
	}
}
