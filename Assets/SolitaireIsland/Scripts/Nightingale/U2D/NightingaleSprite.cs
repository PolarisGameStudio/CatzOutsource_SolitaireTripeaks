using Nightingale.Extensions;
using Nightingale.JSONUtilitys;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Nightingale.U2D
{
	public class NightingaleSprite
	{
		private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

		public Texture2D Texture
		{
			get;
			private set;
		}

		public NightingaleSprite(string content)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(content) as Dictionary<string, object>;
			byte[] bytes = Convert.FromBase64String(dictionary["collection.json"].ToString());
			string @string = Encoding.UTF8.GetString(bytes);
			bytes = Convert.FromBase64String(dictionary["collection.png"].ToString());
			ConvertToNightingaleSprite(@string, bytes.ToTexture());
		}

		public NightingaleSprite(string json, Texture2D texture)
		{
			ConvertToNightingaleSprite(json, texture);
		}

		private void ConvertToNightingaleSprite(string json, Texture2D texture)
		{
			sprites.Clear();
			Texture = texture;
			int width = texture.width;
			int height = texture.height;
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = dictionary["frames"] as Dictionary<string, object>;
			foreach (string key in dictionary2.Keys)
			{
				Dictionary<string, object> dictionary3 = dictionary2[key] as Dictionary<string, object>;
				int num = int.Parse(dictionary3["x"].ToString());
				int num2 = int.Parse(dictionary3["y"].ToString());
				int num3 = int.Parse(dictionary3["w"].ToString());
				int num4 = int.Parse(dictionary3["h"].ToString());
				int num5 = int.Parse(dictionary3["offX"].ToString());
				int num6 = int.Parse(dictionary3["offY"].ToString());
				int num7 = int.Parse(dictionary3["sourceW"].ToString());
				int num8 = int.Parse(dictionary3["sourceH"].ToString());
				sprites.Add(key, Sprite.Create(texture, new Rect(num, height - num2 - num4, num3, num4), new Vector2(0.5f, 0.5f)));
			}
		}

		public Sprite GetSprite(string path)
		{
			if (sprites.ContainsKey(path))
			{
				return sprites[path];
			}
			return null;
		}
	}
}
