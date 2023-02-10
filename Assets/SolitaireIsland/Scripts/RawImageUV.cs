using UnityEngine;
using UnityEngine.UI;

public class RawImageUV : MonoBehaviour
{
	public RawImage target;

	public float SpeedX;

	public float SpeedY;

	private float ox;

	private float oy;

	private float wc;

	private float hc;

	private void Start()
	{
		if (target == null)
		{
			target = GetComponent<RawImage>();
		}
		target.texture.wrapMode = TextureWrapMode.Repeat;
		wc = target.rectTransform.rect.width / (float)target.mainTexture.width;
		hc = target.rectTransform.rect.height / (float)target.mainTexture.height;
	}

	private void Update()
	{
		if (target != null)
		{
			ox += Time.deltaTime * SpeedX;
			oy += Time.deltaTime * SpeedY;
			ox %= 1f;
			oy %= 1f;
			target.uvRect = new Rect(ox, oy, wc, hc);
		}
	}
}
