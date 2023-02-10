using System;
using UnityEngine;

namespace DragonBones
{
	[Serializable]
	public class UnityDragonBonesData : ScriptableObject
	{
		[Serializable]
		public class TextureAtlas
		{
			public TextAsset textureAtlasJSON;

			public Texture2D texture;

			public Material material;

			public Material uiMaterial;
		}

		public string dataName;

		public TextAsset dragonBonesJSON;

		public TextureAtlas[] textureAtlas;

		public void RemoveFromFactory(bool disposeData = true)
		{
			UnityFactory.factory.RemoveDragonBonesData(dataName, disposeData);
			if (this.textureAtlas == null)
			{
				return;
			}
			TextureAtlas[] array = this.textureAtlas;
			foreach (TextureAtlas textureAtlas in array)
			{
				if (textureAtlas != null && textureAtlas.texture != null)
				{
					UnityFactory.factory.RemoveTextureAtlasData(textureAtlas.texture.name, disposeData);
				}
			}
		}
	}
}
