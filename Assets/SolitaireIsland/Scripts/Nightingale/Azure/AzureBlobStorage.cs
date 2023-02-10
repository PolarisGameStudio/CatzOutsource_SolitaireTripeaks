using Nightingale.Utilitys;
using System.IO;
using UnityEngine.Networking;

namespace Nightingale.Azure
{
	public class AzureBlobStorage : SingletonClass<AzureBlobStorage>
	{
		public UnityWebRequest GetCDNBlob(string path, long length = 0L)
		{
			string uri = Path.Combine(NightingaleConfig.Get().StorageBlobAddress, path);
			UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri);
			if (length > 0)
			{
				unityWebRequest.SetRequestHeader("Range", $"bytes={length}-");
			}
			return unityWebRequest;
		}
	}
}
