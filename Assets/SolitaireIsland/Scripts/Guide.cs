using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour, ICanvasRaycastFilter
{
	public RectTransform Target;

	public Canvas Canvas;

	private Vector4 center;

	private Material material;

	private float diameter;

	private float current;

	private Vector3[] corners = new Vector3[4];

	private float yVelocity;

	private void Start()
	{
		Target.GetWorldCorners(corners);
		diameter = Vector2.Distance(WordToCanvasPos(Canvas, corners[0]), WordToCanvasPos(Canvas, corners[2])) / 2f;
		float x = corners[0].x + (corners[3].x - corners[0].x) / 2f;
		float y = corners[0].y + (corners[1].y - corners[0].y) / 2f;
		Vector3 v = new Vector3(x, y, 0f);
		Vector2 localPoint = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, v, Canvas.GetComponent<Camera>(), out localPoint);
		v = new Vector4(localPoint.x, localPoint.y, 0f, 0f);
		material = GetComponent<Image>().material;
		material.SetVector("_Center", v);
		(Canvas.transform as RectTransform).GetWorldCorners(corners);
		for (int i = 0; i < corners.Length; i++)
		{
			current = Mathf.Max(Vector3.Distance(WordToCanvasPos(Canvas, corners[i]), v), current);
		}
		material.SetFloat("_Silder", current);
	}

	private void Update()
	{
		float a = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.2f);
		if (!Mathf.Approximately(a, current))
		{
			current = a;
			material.SetFloat("_Silder", current);
		}
	}

	private Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
	{
		Vector2 localPoint = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out localPoint);
		return localPoint;
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		if (Target == null || Target.gameObject == null || !Target.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool flag = RectTransformUtility.RectangleContainsScreenPoint(Target, sp, eventCamera);
		return !flag;
	}
}
