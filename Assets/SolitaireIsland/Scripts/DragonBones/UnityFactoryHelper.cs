using UnityEngine;

namespace DragonBones
{
	internal static class UnityFactoryHelper
	{
		internal static Material GenerateMaterial(string shaderName, string materialName, Texture texture)
		{
			Shader shader = Shader.Find(shaderName);
			Material material = new Material(shader);
			material.name = materialName;
			material.mainTexture = texture;
			return material;
		}

		internal static string CheckResourecdPath(string path)
		{
			int num = path.LastIndexOf("Resources");
			if (num > 0)
			{
				path = path.Substring(num + 10);
			}
			num = path.LastIndexOf(".");
			if (num > 0)
			{
				path = path.Substring(0, num);
			}
			return path;
		}

		internal static string GetTextureAtlasImagePath(string textureAtlasJSONPath, string textureAtlasImageName)
		{
			int num = textureAtlasJSONPath.LastIndexOf("Resources");
			if (num > 0)
			{
				textureAtlasJSONPath = textureAtlasJSONPath.Substring(num + 10);
			}
			num = textureAtlasJSONPath.LastIndexOf("/");
			string text = textureAtlasImageName;
			if (num > 0)
			{
				text = textureAtlasJSONPath.Substring(0, num + 1) + textureAtlasImageName;
			}
			num = text.LastIndexOf(".");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		internal static string GetTextureAtlasNameByPath(string textureAtlasJSONPath)
		{
			string result = string.Empty;
			int num = textureAtlasJSONPath.LastIndexOf("/") + 1;
			int num2 = textureAtlasJSONPath.LastIndexOf("_tex");
			if (num2 > -1)
			{
				result = ((num2 <= num) ? textureAtlasJSONPath.Substring(num) : textureAtlasJSONPath.Substring(num, num2 - num));
			}
			else if (num > -1)
			{
				result = textureAtlasJSONPath.Substring(num);
			}
			return result;
		}

		internal static void DestroyUnityObject(Object obj)
		{
			if (!(obj == null))
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
	}
}
