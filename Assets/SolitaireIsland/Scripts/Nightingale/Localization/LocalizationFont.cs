using Nightingale.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.Localization
{
	public class LocalizationFont : MonoBehaviour
	{
		public FontConfig config;

		private void Start()
		{
			if (config == null)
			{
				config = FontConfig.GetDefault();
			}
			SetFont(config, force: true);
			FontConfig component = GetComponent<FontConfig>();
			if (component != null)
			{
				SetFont(component, force: false);
			}
		}

		private void SetFont(FontConfig config, bool force)
		{
			Text label = GetComponent<Text>();
			if (label == null)
			{
				return;
			}
			FontData fontData = config.Finder(force);
			if (fontData != null)
			{
				if (fontData.Font != null)
				{
					label.font = fontData.Font;
				}
				label.fontSize = (int)((float)label.fontSize * fontData.SizeScaler);
				label.lineSpacing *= fontData.LineSpacingScaler;
				if (label.resizeTextForBestFit)
				{
					label.resizeTextMinSize = (int)((float)label.resizeTextMinSize * fontData.SizeScaler);
					label.resizeTextMaxSize = (int)((float)label.resizeTextMaxSize * fontData.SizeScaler);
				}
				Vector3 offset = fontData.Offset;
				if (!offset.Equals(Vector3.zero))
				{
					this.DelayDo(new WaitForSeconds(0.05f), delegate
					{
						try
						{
							label.transform.localPosition += offset;
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.Log(ex.Message);
						}
					});
				}
			}
		}
	}
}
