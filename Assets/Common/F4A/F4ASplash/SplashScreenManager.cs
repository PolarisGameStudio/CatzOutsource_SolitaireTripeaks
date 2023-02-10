using System.Collections;
using UnityEngine;

namespace com.F4A.MobileThird
{
	public class SplashScreenManager : MonoBehaviour
	{
		[SerializeField]
		private string nameSceneNext = "";
		[SerializeField]
		private float timeWait = 1;

		private void Start(){
			StartCoroutine (IEStart ());
		}

		private IEnumerator IEStart(){
			var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (nameSceneNext);
			async.allowSceneActivation = false;
			yield return new WaitForSeconds (timeWait);
			async.allowSceneActivation = true;
		}
	}
}