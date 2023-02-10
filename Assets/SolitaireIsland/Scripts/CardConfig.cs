using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public struct CardConfig
	{
		public CardType CardType;

		public Vector2 Position;

		public float EulerAngles;

		public int Index;

		public int zIndex;

		public bool IsOpen;

		public ExtraConfig[] ExtraConfigs;

		public string ExtraContent;

		public void ReadFrom(CardConfig config)
		{
			Position = config.Position;
			EulerAngles = config.EulerAngles;
			zIndex = config.zIndex;
			IsOpen = config.IsOpen;
		}

		public Vector2 GetPosition()
		{
			return Position + new Vector2(0f, 0.38f);
		}

		public bool HasExtraType(ExtraType extraType)
		{
			if (ExtraConfigs == null)
			{
				return false;
			}
			ExtraConfig[] extraConfigs = ExtraConfigs;
			for (int i = 0; i < extraConfigs.Length; i++)
			{
				ExtraConfig extraConfig = extraConfigs[i];
				if (extraConfig.ClassType == extraType)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasExtraType(ExtraConfig config)
		{
			if (ExtraConfigs == null)
			{
				return false;
			}
			ExtraConfig[] extraConfigs = ExtraConfigs;
			for (int i = 0; i < extraConfigs.Length; i++)
			{
				ExtraConfig extraConfig = extraConfigs[i];
				if (extraConfig.ClassType == config.ClassType)
				{
					if (extraConfig.ClassType == ExtraType.NumberGrow)
					{
						return extraConfig.Index % 2 == config.Index % 2;
					}
					return true;
				}
			}
			return false;
		}

		public void Init()
		{
			if (HasExtraType(ExtraType.Batter))
			{
				IsOpen = true;
			}
			if (HasExtraType(ExtraType.Key))
			{
				IsOpen = true;
			}
			if (HasExtraType(ExtraType.Lock))
			{
				IsOpen = true;
			}
		}
	}
}
