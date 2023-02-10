using UnityEngine;

public class RippleEffect : MonoBehaviour
{
	private class Droplet
	{
		private Vector2 position;

		private float time;

		public Droplet()
		{
			time = 1000f;
		}

		public void Reset()
		{
			position = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
			time = 0f;
		}

		public void Update()
		{
			time += Time.deltaTime;
		}

		public Vector4 MakeShaderParameter(float aspect)
		{
			return new Vector4(position.x * aspect, position.y, time, 0f);
		}
	}

	public AnimationCurve waveform = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 0f), new Keyframe(0.05f, 1f, 0f, 0f), new Keyframe(0.15f, 0.1f, 0f, 0f), new Keyframe(0.25f, 0.8f, 0f, 0f), new Keyframe(0.35f, 0.3f, 0f, 0f), new Keyframe(0.45f, 0.6f, 0f, 0f), new Keyframe(0.55f, 0.4f, 0f, 0f), new Keyframe(0.65f, 0.55f, 0f, 0f), new Keyframe(0.75f, 0.46f, 0f, 0f), new Keyframe(0.85f, 0.52f, 0f, 0f), new Keyframe(0.99f, 0.5f, 0f, 0f));

	[Range(0.01f, 1f)]
	public float refractionStrength = 0.5f;

	public Color reflectionColor = Color.gray;

	[Range(0.01f, 1f)]
	public float reflectionStrength = 0.7f;

	[Range(1f, 3f)]
	public float waveSpeed = 1.25f;

	[Range(0f, 2f)]
	public float dropInterval = 0.5f;

	[SerializeField]
	[HideInInspector]
	private Shader shader;

	private Droplet[] droplets;

	private Texture2D gradTexture;

	private Material material;

	private float timer;

	private int dropCount;

	private void UpdateShaderParameters()
	{
		Camera component = GetComponent<Camera>();
		material.SetVector("_Drop1", droplets[0].MakeShaderParameter(component.aspect));
		material.SetVector("_Drop2", droplets[1].MakeShaderParameter(component.aspect));
		material.SetVector("_Drop3", droplets[2].MakeShaderParameter(component.aspect));
		material.SetColor("_Reflection", reflectionColor);
		material.SetVector("_Params1", new Vector4(component.aspect, 1f, 1f / waveSpeed, 0f));
		material.SetVector("_Params2", new Vector4(1f, 1f / component.aspect, refractionStrength, reflectionStrength));
	}

	private void Awake()
	{
		droplets = new Droplet[3];
		droplets[0] = new Droplet();
		droplets[1] = new Droplet();
		droplets[2] = new Droplet();
		gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, mipChain: false);
		gradTexture.wrapMode = TextureWrapMode.Clamp;
		gradTexture.filterMode = FilterMode.Bilinear;
		for (int i = 0; i < gradTexture.width; i++)
		{
			float time = 1f / (float)gradTexture.width * (float)i;
			float num = waveform.Evaluate(time);
			gradTexture.SetPixel(i, 0, new Color(num, num, num, num));
		}
		gradTexture.Apply();
		material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;
		material.SetTexture("_GradTex", gradTexture);
		UpdateShaderParameters();
	}

	private void Update()
	{
		if (dropInterval > 0f)
		{
			timer += Time.deltaTime;
			while (timer > dropInterval)
			{
				Emit();
				timer -= dropInterval;
			}
		}
		Droplet[] array = droplets;
		foreach (Droplet droplet in array)
		{
			droplet.Update();
		}
		UpdateShaderParameters();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}

	public void Emit()
	{
		droplets[dropCount++ % droplets.Length].Reset();
	}
}
