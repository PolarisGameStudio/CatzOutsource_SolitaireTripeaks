public class NtgNativeAgent
{
	public static void Init(bool debug = false)
	{
		NtgAndroidAgent.Init(debug);
	}

	public static string GetInstallReferrer()
	{
		return NtgAndroidAgent.GetInstallReferrer();
	}
}
