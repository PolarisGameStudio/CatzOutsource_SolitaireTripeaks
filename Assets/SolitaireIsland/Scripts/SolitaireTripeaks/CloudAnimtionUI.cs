using UnityEngine;

namespace SolitaireTripeaks
{
	public class CloudAnimtionUI : MonoBehaviour
	{
		public float minScale = 0.3f;

		public float maxScale = 0.5f;

		private Vector3 localPosition;

		private Vector3 Radius;

		private void Start()
		{
			Vector2 sizeDelta = (base.transform as RectTransform).sizeDelta;
			float x = sizeDelta.x;
			Vector2 sizeDelta2 = (base.transform as RectTransform).sizeDelta;
			float y = sizeDelta2.y;
			base.transform.localScale = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);
			localPosition = base.transform.localPosition;
			float num = x;
			Vector3 localScale = base.transform.localScale;
			float min = num * localScale.x * 0.05f;
			float num2 = x;
			Vector3 localScale2 = base.transform.localScale;
			float x2 = UnityEngine.Random.Range(min, num2 * localScale2.x * 0.25f);
			float num3 = y;
			Vector3 localScale3 = base.transform.localScale;
			float min2 = num3 * localScale3.y * 0.05f;
			float num4 = y;
			Vector3 localScale4 = base.transform.localScale;
			Radius = new Vector3(x2, UnityEngine.Random.Range(min2, num4 * localScale4.y * 0.25f), UnityEngine.Random.Range(0.1f, 0.5f));
		}

		private void Update()
		{
			base.transform.localPosition = new Vector3(localPosition.x + Mathf.Sin(Time.timeSinceLevelLoad * Radius.z) * Radius.x, localPosition.y + Mathf.Cos(Time.timeSinceLevelLoad * Radius.z) * Radius.y, 0f);
		}
	}
}
