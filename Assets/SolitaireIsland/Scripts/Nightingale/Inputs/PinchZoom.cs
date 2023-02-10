using UnityEngine;

namespace Nightingale.Inputs
{
	public class PinchZoom : MonoBehaviour
	{
		private float scale_factor = 0.07f;

		private float MAXSCALE = 6f;

		private float MIN_SCALE = 0.6f;

		private bool isMousePressed;

		private Vector2 prevDist = new Vector2(0f, 0f);

		private Vector2 curDist = new Vector2(0f, 0f);

		private Vector2 midPoint = new Vector2(0f, 0f);

		private Vector2 ScreenSize;

		private Vector3 originalPos;

		private GameObject parentObject;

		private static Vector3 prevPos = Vector3.zero;

		private void Start()
		{
			parentObject = new GameObject("ParentObject");
			parentObject.transform.parent = base.transform.parent;
			Transform transform = parentObject.transform;
			Vector3 position = base.transform.position;
			float x = position.x * -1f;
			Vector3 position2 = base.transform.position;
			float y = position2.y * -1f;
			Vector3 position3 = base.transform.position;
			transform.position = new Vector3(x, y, position3.z);
			base.transform.parent = parentObject.transform;
			ScreenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
			originalPos = base.transform.position;
			isMousePressed = false;
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				isMousePressed = true;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				isMousePressed = false;
			}
			if (!isMousePressed || UnityEngine.Input.touchCount != 1 || UnityEngine.Input.GetTouch(0).phase != TouchPhase.Moved)
			{
				goto IL_029a;
			}
			Vector3 localScale = parentObject.transform.localScale;
			if (!(localScale.x > MIN_SCALE))
			{
				Vector3 localScale2 = parentObject.transform.localScale;
				if (!(localScale2.y > MIN_SCALE))
				{
					goto IL_029a;
				}
			}
			Vector3 b = UnityEngine.Input.GetTouch(0).deltaPosition * 0.1f;
			Vector3 position = base.transform.position + b;
			float x = position.x;
			float x2 = ScreenSize.x;
			Vector3 localScale3 = parentObject.transform.localScale;
			if (x > x2 * (localScale3.x - 1f))
			{
				float x3 = ScreenSize.x;
				Vector3 localScale4 = parentObject.transform.localScale;
				position.x = x3 * (localScale4.x - 1f);
			}
			float x4 = position.x;
			float x5 = ScreenSize.x;
			Vector3 localScale5 = parentObject.transform.localScale;
			if (x4 < x5 * (localScale5.x - 1f) * -1f)
			{
				float x6 = ScreenSize.x;
				Vector3 localScale6 = parentObject.transform.localScale;
				position.x = x6 * (localScale6.x - 1f) * -1f;
			}
			float y = position.y;
			float y2 = ScreenSize.y;
			Vector3 localScale7 = parentObject.transform.localScale;
			if (y > y2 * (localScale7.y - 1f))
			{
				float y3 = ScreenSize.y;
				Vector3 localScale8 = parentObject.transform.localScale;
				position.y = y3 * (localScale8.y - 1f);
			}
			float y4 = position.y;
			float y5 = ScreenSize.y;
			Vector3 localScale9 = parentObject.transform.localScale;
			if (y4 < y5 * (localScale9.y - 1f) * -1f)
			{
				float y6 = ScreenSize.y;
				Vector3 localScale10 = parentObject.transform.localScale;
				position.y = y6 * (localScale10.y - 1f) * -1f;
			}
			base.transform.position = position;
			goto IL_0336;
			IL_0336:
			checkForMultiTouch();
			return;
			IL_029a:
			if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began && UnityEngine.Input.GetTouch(0).tapCount == 2)
			{
				parentObject.transform.localScale = Vector3.one;
				parentObject.transform.position = new Vector3(originalPos.x * -1f, originalPos.y * -1f, originalPos.z);
				base.transform.position = originalPos;
			}
			goto IL_0336;
		}

		private void checkForMultiTouch()
		{
			if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved && UnityEngine.Input.GetTouch(1).phase == TouchPhase.Moved)
			{
				Vector2 position = UnityEngine.Input.GetTouch(0).position;
				float x = position.x;
				Vector2 position2 = UnityEngine.Input.GetTouch(1).position;
				float x2 = (x + position2.x) / 2f;
				Vector2 position3 = UnityEngine.Input.GetTouch(0).position;
				float y = position3.y;
				Vector2 position4 = UnityEngine.Input.GetTouch(1).position;
				midPoint = new Vector2(x2, (y + position4.y) / 2f);
				midPoint = Camera.main.ScreenToWorldPoint(midPoint);
				curDist = UnityEngine.Input.GetTouch(0).position - UnityEngine.Input.GetTouch(1).position;
				prevDist = UnityEngine.Input.GetTouch(0).position - UnityEngine.Input.GetTouch(0).deltaPosition - (UnityEngine.Input.GetTouch(1).position - UnityEngine.Input.GetTouch(1).deltaPosition);
				float num = curDist.magnitude - prevDist.magnitude;
				if (num > 0f)
				{
					Vector3 localScale = parentObject.transform.localScale;
					if (localScale.x < MAXSCALE)
					{
						Vector3 localScale2 = parentObject.transform.localScale;
						if (localScale2.y < MAXSCALE)
						{
							Vector3 localScale3 = parentObject.transform.localScale;
							float x3 = localScale3.x + scale_factor;
							Vector3 localScale4 = parentObject.transform.localScale;
							Vector3 scale = new Vector3(x3, localScale4.y + scale_factor, 1f);
							scale.x = ((!(scale.x > MAXSCALE)) ? scale.x : MAXSCALE);
							scale.y = ((!(scale.y > MAXSCALE)) ? scale.y : MAXSCALE);
							scaleFromPosition(scale, midPoint);
						}
					}
				}
				else
				{
					if (!(num < 0f))
					{
						return;
					}
					Vector3 localScale5 = parentObject.transform.localScale;
					if (localScale5.x > MIN_SCALE)
					{
						Vector3 localScale6 = parentObject.transform.localScale;
						if (localScale6.y > MIN_SCALE)
						{
							Vector3 localScale7 = parentObject.transform.localScale;
							float x4 = localScale7.x + scale_factor * -1f;
							Vector3 localScale8 = parentObject.transform.localScale;
							Vector3 scale2 = new Vector3(x4, localScale8.y + scale_factor * -1f, 1f);
							scale2.x = ((!(scale2.x < MIN_SCALE)) ? scale2.x : MIN_SCALE);
							scale2.y = ((!(scale2.y < MIN_SCALE)) ? scale2.y : MIN_SCALE);
							scaleFromPosition(scale2, midPoint);
						}
					}
				}
			}
			else
			{
				if (UnityEngine.Input.touchCount != 2 || (UnityEngine.Input.GetTouch(0).phase != TouchPhase.Ended && UnityEngine.Input.GetTouch(0).phase != TouchPhase.Canceled && UnityEngine.Input.GetTouch(1).phase != TouchPhase.Ended && UnityEngine.Input.GetTouch(1).phase != TouchPhase.Canceled))
				{
					return;
				}
				Vector3 localScale9 = parentObject.transform.localScale;
				if (!(localScale9.x < 1f))
				{
					Vector3 localScale10 = parentObject.transform.localScale;
					if (!(localScale10.y < 1f))
					{
						Vector3 position5 = base.transform.position;
						float x5 = position5.x;
						float x6 = ScreenSize.x;
						Vector3 localScale11 = parentObject.transform.localScale;
						if (x5 > x6 * (localScale11.x - 1f))
						{
							float x7 = ScreenSize.x;
							Vector3 localScale12 = parentObject.transform.localScale;
							position5.x = x7 * (localScale12.x - 1f);
						}
						float x8 = position5.x;
						float x9 = ScreenSize.x;
						Vector3 localScale13 = parentObject.transform.localScale;
						if (x8 < x9 * (localScale13.x - 1f) * -1f)
						{
							float x10 = ScreenSize.x;
							Vector3 localScale14 = parentObject.transform.localScale;
							position5.x = x10 * (localScale14.x - 1f) * -1f;
						}
						float y2 = position5.y;
						float y3 = ScreenSize.y;
						Vector3 localScale15 = parentObject.transform.localScale;
						if (y2 > y3 * (localScale15.y - 1f))
						{
							float y4 = ScreenSize.y;
							Vector3 localScale16 = parentObject.transform.localScale;
							position5.y = y4 * (localScale16.y - 1f);
						}
						float y5 = position5.y;
						float y6 = ScreenSize.y;
						Vector3 localScale17 = parentObject.transform.localScale;
						if (y5 < y6 * (localScale17.y - 1f) * -1f)
						{
							float y7 = ScreenSize.y;
							Vector3 localScale18 = parentObject.transform.localScale;
							position5.y = y7 * (localScale18.y - 1f) * -1f;
						}
						base.transform.position = position5;
						return;
					}
				}
				parentObject.transform.localScale = Vector3.one;
				parentObject.transform.position = new Vector3(originalPos.x * -1f, originalPos.y * -1f, originalPos.z);
				base.transform.position = originalPos;
			}
		}

		private void scaleFromPosition(Vector3 scale, Vector3 fromPos)
		{
			if (!fromPos.Equals(prevPos))
			{
				Vector3 position = parentObject.transform.position;
				parentObject.transform.position = fromPos;
				Vector3 vector = parentObject.transform.position - position;
				float x = vector.x;
				Vector3 localScale = parentObject.transform.localScale;
				float x2 = x / localScale.x * -1f;
				float y = vector.y;
				Vector3 localScale2 = parentObject.transform.localScale;
				float y2 = y / localScale2.y * -1f;
				Vector3 position2 = base.transform.position;
				Vector3 vector2 = new Vector3(x2, y2, position2.z);
				Transform transform = base.transform;
				Vector3 localPosition = base.transform.localPosition;
				float x3 = localPosition.x + vector2.x;
				Vector3 localPosition2 = base.transform.localPosition;
				transform.localPosition = new Vector3(x3, localPosition2.y + vector2.y, vector2.z);
			}
			parentObject.transform.localScale = scale;
			prevPos = fromPos;
		}
	}
}
