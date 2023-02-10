using UnityEngine;

namespace I2.MiniGames
{
	[AddComponentMenu("I2/MiniGames/Wheel/Reward")]
	public class PrizeWheel_Reward : MiniGame_Reward
	{
		public RectTransform _Content;

		public bool _RotateContent = true;

		public RectTransform _Separator;

		public UnityEventFloat _OnBackgroundSetFillAmount = new UnityEventFloat();

		public virtual void ApplyLayout(float Angle, float SliceSize, PrizeWheel wheel)
		{
			float num = 90 * (int)wheel._SelectorDirection;
			float num2 = 0f - Angle - num;
			Quaternion quaternion = Quaternion.Euler(0f, 0f, num2);
			Quaternion quaternion2 = Quaternion.Euler(0f, 0f, (0f - SliceSize) / 2f);
			Quaternion localRotation = Quaternion.Euler(0f, 0f, (!_RotateContent) ? (0f - num2) : (num - SliceSize / 2f));
			Vector3 localPosition = quaternion * quaternion2 * Vector3.up * wheel._Elements_Offset;
			base.transform.localRotation = quaternion;
			base.transform.localPosition = localPosition;
			_OnBackgroundSetFillAmount.Invoke(SliceSize / 360f);
			if ((bool)_Separator)
			{
				_Separator.localRotation = quaternion;
				_Separator.localPosition = localPosition;
			}
			if ((bool)_Content)
			{
				float magnitude = _Content.localPosition.magnitude;
				_Content.localPosition = quaternion2 * (Vector3.up * magnitude);
				_Content.localRotation = localRotation;
			}
		}

		public void CollapseTransform(RectTransform tr)
		{
			Rect rect = tr.rect;
			Vector3 localPosition = tr.localPosition;
			tr.offsetMin = tr.anchorMin;
			tr.offsetMax = tr.anchorMax;
			tr.anchorMin = new Vector2(0.5f, 0.5f);
			tr.anchorMax = tr.anchorMin;
			tr.offsetMin = rect.min + (Vector2)localPosition;
			tr.offsetMax = rect.max + (Vector2)localPosition;
		}

		public virtual void SpinningUpdate()
		{
			if (!_RotateContent && (bool)_Content)
			{
				_Content.rotation = Quaternion.identity;
			}
		}

		public override void Hide()
		{
		}

		public override void Show(Transform parent)
		{
		}
	}
}
