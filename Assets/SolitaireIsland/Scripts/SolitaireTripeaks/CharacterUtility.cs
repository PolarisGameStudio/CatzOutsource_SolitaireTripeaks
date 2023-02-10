using DG.Tweening;
using DragonBones;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class CharacterUtility : MonoBehaviour
	{
		public UnityArmatureComponent ArmatureComponent;

		private const string IdleArmature = "Standby";

		private const string BeckArmature = "Idle";

		private const string JumpArmature = "Run";

		private const string VictoryArmature = "Victory";

		private Vector3 offset = new Vector3(0f, 30f, 0f);

		private const float islandScale = 30f;

		private void OnPlayBeck()
		{
			if (!(ArmatureComponent == null))
			{
				CancelInvoke("OnPlayIdle");
				CancelInvoke("OnPlayBeck");
				Invoke("OnPlayIdle", ArmatureComponent.animation.Play("Idle").totalTime);
			}
		}

		private void OnPlayIdle()
		{
			if (!(ArmatureComponent == null))
			{
				CancelInvoke("OnPlayIdle");
				CancelInvoke("OnPlayBeck");
				Invoke("OnPlayBeck", ArmatureComponent.animation.Play("Standby").totalTime * (float)UnityEngine.Random.Range(5, 10));
			}
		}

		public void Jump(UnityEngine.Transform trans, UnityAction unityAction)
		{
			Vector3 v = base.transform.parent.InverseTransformPoint(trans.position) + offset;
			RectTransform rectTransform = base.transform as RectTransform;
			UnityEngine.Transform transform = base.transform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			transform.localScale = new Vector3((!(anchoredPosition.x < v.x)) ? (-30f) : 30f, 30f);
			rectTransform.DOJumpAnchorPos(v, 300f, 1, 1.1f).SetEase(Ease.Linear).OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
			if (!(ArmatureComponent == null))
			{
				CancelInvoke("OnPlayIdle");
				CancelInvoke("OnPlayBeck");
				Invoke("OnPlayIdle", ArmatureComponent.animation.Play("Run").totalTime);
			}
		}

		public void SetPosition(UnityEngine.Transform trans)
		{
			Vector3 v = base.transform.parent.InverseTransformPoint(trans.position) + offset;
			(base.transform as RectTransform).anchoredPosition = v;
		}

		public void CreateInIsland()
		{
			OnPlayBeck();
		}

		public void CreateInVictory()
		{
			if (!(ArmatureComponent == null))
			{
				RectTransform rectTransform = base.transform as RectTransform;
				rectTransform.anchorMin = new Vector2(0.5f, 0f);
				rectTransform.anchorMax = new Vector2(0.5f, 0f);
				rectTransform.anchoredPosition = new Vector3(620f, -140f, 0f);
				base.transform.localScale = new Vector3(-100f, 100f, 100f);
				ArmatureComponent.animation.Play("Victory", 1);
			}
		}
	}
}
