using System;
using UnityEngine;

namespace Nightingale.Localization
{
	[Serializable]
	public class FontData
	{
		public SystemLanguage Language;

		public Font Font;

		public float SizeScaler;

		public float LineSpacingScaler;

		public Vector3 Offset;
	}
}
