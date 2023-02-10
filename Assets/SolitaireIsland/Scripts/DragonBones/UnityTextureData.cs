using System.Collections.Generic;
using UnityEngine;

namespace DragonBones
{
	internal class UnityTextureData : TextureData
	{
		public const string SHADER_PATH = "Shaders/";

		public const string SHADER_GRAP = "DB_BlendMode_Grab";

		public const string SHADER_FRAME_BUFFER = "DB_BlendMode_Framebuffer";

		public const string UI_SHADER_GRAP = "DB_BlendMode_UIGrab";

		public const string UI_SHADER_FRAME_BUFFER = "DB_BlendMode_UIFramebuffer";

		internal Dictionary<string, Material> _cacheBlendModeMats = new Dictionary<string, Material>();

		protected override void _OnClear()
		{
			base._OnClear();
			foreach (string key in _cacheBlendModeMats.Keys)
			{
				Material material = _cacheBlendModeMats[key];
				if (material != null)
				{
					UnityFactoryHelper.DestroyUnityObject(material);
				}
			}
			_cacheBlendModeMats.Clear();
		}

		private Material _GetMaterial(BlendMode blendMode)
		{
			if (blendMode == BlendMode.Normal)
			{
				return (parent as UnityTextureAtlasData).texture;
			}
			string key = blendMode.ToString();
			if (_cacheBlendModeMats.ContainsKey(key))
			{
				return _cacheBlendModeMats[key];
			}
			Material material = new Material(Resources.Load<Shader>("Shaders/DB_BlendMode_Grab"));
			Texture mainTexture = (parent as UnityTextureAtlasData).texture.mainTexture;
			material.name = mainTexture.name + "_DB_BlendMode_Grab_Mat";
			material.hideFlags = HideFlags.HideAndDontSave;
			material.mainTexture = mainTexture;
			_cacheBlendModeMats.Add(key, material);
			return material;
		}

		private Material _GetUIMaterial(BlendMode blendMode)
		{
			if (blendMode == BlendMode.Normal)
			{
				return (parent as UnityTextureAtlasData).uiTexture;
			}
			string key = "UI_" + blendMode.ToString();
			if (_cacheBlendModeMats.ContainsKey(key))
			{
				return _cacheBlendModeMats[key];
			}
			Material material = new Material(Resources.Load<Shader>("Shaders/DB_BlendMode_UIGrab"));
			Texture mainTexture = (parent as UnityTextureAtlasData).uiTexture.mainTexture;
			material.name = mainTexture.name + "_DB_BlendMode_Grab_Mat";
			material.hideFlags = HideFlags.HideAndDontSave;
			material.mainTexture = mainTexture;
			_cacheBlendModeMats.Add(key, material);
			return material;
		}

		internal Material GetMaterial(BlendMode blendMode, bool isUGUI = false)
		{
			if (isUGUI)
			{
				return _GetUIMaterial(blendMode);
			}
			return _GetMaterial(blendMode);
		}

		public override void CopyFrom(TextureData value)
		{
			base.CopyFrom(value);
			(value as UnityTextureData)._cacheBlendModeMats = _cacheBlendModeMats;
		}
	}
}
