using DG.Tweening;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SlotsLightUI : MonoBehaviour
	{
		public GameObject[] Lights;

		private Sequence sequence;

		private void Awake()
		{
			GameObject[] lights = Lights;
			foreach (GameObject gameObject in lights)
			{
				gameObject.SetActive(value: false);
			}
		}

		private void OnDestroy()
		{
			StopLight();
		}

		public void StartLight()
		{
			StopLight();
			int index = UnityEngine.Random.Range(0, Lights.Length);
			sequence = DOTween.Sequence();
			sequence.AppendInterval(0.08f);
			sequence.AppendCallback(delegate
			{
				index++;
				index %= Lights.Length;
				for (int i = 0; i < Lights.Length; i++)
				{
					Lights[i].SetActive(i == index);
				}
			});
			sequence.SetLoops(-1);
		}

		public void StopLight()
		{
			if (sequence != null)
			{
				sequence.Kill(complete: true);
			}
			GameObject[] lights = Lights;
			foreach (GameObject gameObject in lights)
			{
				gameObject.SetActive(value: false);
			}
		}

		public void WinLight()
		{
			StopLight();
			bool visable = true;
			sequence = DOTween.Sequence();
			sequence.AppendInterval(0.2f);
			sequence.AppendCallback(delegate
			{
				visable = !visable;
				GameObject[] lights = Lights;
				foreach (GameObject gameObject in lights)
				{
					gameObject.SetActive(visable);
				}
			});
			sequence.SetLoops(-1);
		}
	}
}
