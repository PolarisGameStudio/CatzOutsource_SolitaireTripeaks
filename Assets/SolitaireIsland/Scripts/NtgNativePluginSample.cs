using UnityEngine;
using UnityEngine.UI;

public class NtgNativePluginSample : MonoBehaviour
{
	public Text TxtInstallReferrer;

	private void Start()
	{
		NtgNativeAgent.Init(debug: true);
		TxtInstallReferrer.text = NtgNativeAgent.GetInstallReferrer();
	}

	private void Update()
	{
	}
}
