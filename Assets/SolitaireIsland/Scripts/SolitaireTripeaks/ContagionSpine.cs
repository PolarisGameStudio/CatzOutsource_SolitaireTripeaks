using DG.Tweening;
using DragonBones;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class ContagionSpine : BaseSpine
	{
		private const string Left = "zuozhuan";

		private const string Right = "youzhuan";

		private const string Attack = "shenshetou";

		private const string Idle = "daiji";

		public GameObject _FlyObject;

		public GameObject _BombObject;

		public UnityArmatureComponent Armature;

		public override void PlayTransform(UnityEngine.Transform pokerTransform, UnityAction unityAction)
		{
			if (!(Armature == null))
			{
				Vector3 rotate = new Vector3(0f, 0f, MathUtility.CalcAngle(pokerTransform.position - base.transform.position));
				if (rotate.z < 0f)
				{
					rotate.z += 360f;
				}
				Armature.sortingOrder = 500;
				UnityEngine.Transform transform = base.transform;
				Vector3 endValue = rotate;
				DragonBones.Animation animation = Armature.animation;
				float z = rotate.z;
				Vector3 eulerAngles = pokerTransform.eulerAngles;
				transform.DORotate(endValue, animation.Play((!(z > eulerAngles.z)) ? "youzhuan" : "zuozhuan").totalTime).OnComplete(delegate
				{
					AudioUtility.GetSound().Play("Audios/koushui.wav");
					Armature.animation.Play("shenshetou", 1);
					DelayDo(new WaitForSeconds(0.2f), delegate
					{
						GameObject fly = Object.Instantiate(_FlyObject);
						fly.transform.position = base.transform.position;
						fly.transform.eulerAngles = rotate;
						fly.transform.DOMove(pokerTransform.position, 0.5f).OnComplete(delegate
						{
							UnityEngine.Object.Destroy(fly);
							Armature.animation.Play("daiji");
							if (unityAction != null)
							{
								unityAction();
							}
							UnityEngine.Object.Destroy(Object.Instantiate(_BombObject, pokerTransform.position, Quaternion.identity), 0.5f);
						});
					});
				});
			}
		}

		public override void PlayDestroy(UnityAction unityAction)
		{
			base.transform.DOScale(0f, 0.2f).OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			if (!(Armature == null))
			{
				Armature.sortingOrder = zIndex + index * 2;
			}
		}

		public override void UpdateColor(bool white)
		{
		}
	}
}
