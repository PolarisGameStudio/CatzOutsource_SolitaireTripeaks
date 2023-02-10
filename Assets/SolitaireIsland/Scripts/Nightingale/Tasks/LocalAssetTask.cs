using Nightingale.Utilitys;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Nightingale.Tasks
{
	public class LocalAssetTask : NormalTask
	{
		public LocalAssetTask(string path)
		{
			if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
			{
				//base.TaskId = Application.persistentDataPath + "/" + path;
#if UNITY_ANDROID || UNITY_IOS
				base.TaskId = "file://" + Application.persistentDataPath + "/" + path;
#else
				base.TaskId = Application.persistentDataPath + "/" + path;
#endif
            }
            else
			{
				base.TaskId = StreamingAssetsPathUtility.StreamingAssetsPath(path);
            }
            //base.TaskId = Path.Combine(base.TaskId, path);
			DoSomething = DoAssetTask;
		}

		private IEnumerator DoAssetTask(UnityAction<object, float> completed)
		{
			if (completed == null)
			{
				yield return new WaitForEndOfFrame();
				yield break;
			}
			AssetBundle asset = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(e.name, TaskId));
			if (asset != null)
			{
				completed(asset, 1f);
				yield return new WaitForEndOfFrame();
				yield break;
			}
            Debug.Log("@LOG LocalAssetTask.DoAssetTask TaskId:" + base.TaskId);
            //AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(base.TaskId);
            //yield return request;
            //if (request.isDone)
            //{
            //    completed(request.assetBundle, 1f);
            //    yield break;
            //}

            UnityWebRequest request = new UnityWebRequest();
            request = UnityWebRequestAssetBundle.GetAssetBundle(base.TaskId);
            yield return request.SendWebRequest();
            if (request.isDone)
			{
                var assetBundle = ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
                completed(assetBundle, 1f);
				yield break;
			}
			yield return new WaitForSeconds(1f);
			completed(null, 1f);
		}
	}
}
