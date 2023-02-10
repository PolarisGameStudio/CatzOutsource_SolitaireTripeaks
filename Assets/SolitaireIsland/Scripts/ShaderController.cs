using UnityEngine;

public class ShaderController : MonoBehaviour
{
	public string ShaderName = "Mobile/Particles/Additive";

	private void Start()
	{
		GetComponent<ParticleSystemRenderer>().sharedMaterial.shader = Shader.Find(ShaderName);
	}
}
