using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Nightingale.Extensions
{
	public class DelayBehaviour : MonoBehaviour
	{
		private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		private IEnumerator delayDo(YieldInstruction instruction, UnityAction unityAction)
		{
			yield return instruction;
			unityAction?.Invoke();
		}

		private IEnumerator delayDo(CustomYieldInstruction instruction, UnityAction unityAction)
		{
			yield return instruction;
			unityAction?.Invoke();
		}

		private IEnumerator loopDelayDo(Func<bool> func, UnityAction unityAction)
		{
			while (func())
			{
				yield return waitForEndOfFrame;
			}
			unityAction?.Invoke();
		}

		protected IEnumerator StartUnityWeb(UnityWebRequest unityWebRequest, UnityAction<DownloadHandler> unityAction)
		{
			yield return unityWebRequest.SendWebRequest();
			unityAction?.Invoke(unityWebRequest.downloadHandler);
		}

		protected IEnumerator StartUnityWeb(UnityWebRequest unityWebRequest, UnityAction<UnityWebRequest> unityAction)
		{
			yield return unityWebRequest.SendWebRequest();
			unityAction?.Invoke(unityWebRequest);
		}

		protected void DelayDo(UnityAction unityAction)
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(delayDo(waitForEndOfFrame, unityAction));
			}
		}

		protected void LoopDelayDo(Func<bool> func, UnityAction unityAction)
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(loopDelayDo(func, unityAction));
			}
		}

		protected void DelayDo(YieldInstruction instruction, UnityAction unityAction)
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(delayDo(instruction, unityAction));
			}
		}

		protected void DelayDo(CustomYieldInstruction instruction, UnityAction unityAction)
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(delayDo(instruction, unityAction));
			}
		}
	}
}
