using Nightingale.Extensions;
using Nightingale.Utilitys;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Nightingale.Tasks
{
	public class ImageTask : NormalTask
	{
		private string fileNameGuid;

		public ImageTask(string url, string guid)
		{
			base.TaskId = url;
			DoSomething = DoImageTask;
			fileNameGuid = guid;
		}

		private IEnumerator DoImageTask(UnityAction<object, float> completed)
		{
			if (completed == null)
			{
				yield return new WaitForEndOfFrame();
				yield break;
			}
			if (string.IsNullOrEmpty(fileNameGuid))
			{
				fileNameGuid = string.Format("{0}.png", base.TaskId.GetHashCode().ToString().Replace("-", "s"));
			}
			string fileName = $"Caches/Images/{fileNameGuid}.png";
			if (FileUtility.Exists(Application.persistentDataPath, fileName))
			{
				yield return new WaitForEndOfFrame();
				base.Process = 1f;
				try
				{
					Sprite arg = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, fileName)).ToSprite();
					completed(arg, base.Process);
				}
				catch (Exception)
				{
					completed(null, base.Process);
				}
				yield break;
			}
			UnityEngine.Debug.LogWarningFormat("新建图片下载，地址：{0}", base.TaskId);
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(base.TaskId);
			yield return request.SendWebRequest();
			if (request.isDone)
			{
				if (!request.isNetworkError && !request.isHttpError)
				{
					base.Process = 1f;
					Texture2D content = DownloadHandlerTexture.GetContent(request);
					try
					{
						completed(content.ToSprite(), base.Process);
						FileUtility.SaveFile(Application.persistentDataPath, fileName, content.EncodeToPNG());
					}
					catch (Exception ex2)
					{
						UnityEngine.Debug.Log(ex2.Message);
					}
				}
				else
				{
					request.Dispose();
					completed(null, 1f);
				}
			}
		}
	}
}
