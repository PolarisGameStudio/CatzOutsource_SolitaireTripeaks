using DG.Tweening;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public static class SolitaireVariationExtensions
	{
		public static string TOString(this TimeSpan timeSpan)
		{
			if (timeSpan.Days > 0)
			{
				return $"{timeSpan.Days:D1}d:{timeSpan.Hours:D1}:{timeSpan.Minutes:D1}:{timeSpan.Seconds:D1}";
			}
			if (timeSpan.TotalSeconds < 0.0)
			{
				timeSpan = default(TimeSpan);
			}
			return $"{timeSpan.Hours:D1}:{timeSpan.Minutes:D1}:{timeSpan.Seconds:D1}";
		}

		public static string TOShortString(this TimeSpan timeSpan)
		{
			if (timeSpan.TotalSeconds < 0.0)
			{
				timeSpan = default(TimeSpan);
			}
			return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}

		public static string TORoughString(this TimeSpan timeSpan)
		{
			if (timeSpan.TotalSeconds < 0.0)
			{
				timeSpan = default(TimeSpan);
			}
			return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		}

		public static Tweener DOPath(this Transform transform, Vector3 targetPos, float duration)
		{
			Vector3 position = transform.position;
			Vector3 item = targetPos;
			if (item.x > position.x)
			{
				item.x += 20f;
			}
			else
			{
				item.x -= 20f;
			}
			item.y += item.y - position.y;
			List<Vector3> list = new List<Vector3>();
			list.Add(position);
			list.Add(targetPos);
			list.Add(item);
			Vector3[] path = MathUtility.MakeSmoothCurve(list, 10f).ToArray();
			return transform.DOPath(path, duration);
		}

		public static Tweener DORotateX(this Transform transform, float endVaule, float duration)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float angle = eulerAngles.x;
			return DOTween.To(() => angle, delegate(float vaule)
			{
				Transform transform2 = transform;
				Vector3 eulerAngles2 = transform.eulerAngles;
				float y = eulerAngles2.y;
				Vector3 eulerAngles3 = transform.eulerAngles;
				transform2.eulerAngles = new Vector3(vaule, y, eulerAngles3.z);
			}, endVaule, duration);
		}

		public static Tweener DORotateZ(this Transform transform, float endVaule, float duration)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float angle = eulerAngles.x;
			return DOTween.To(() => angle, delegate(float vaule)
			{
				Transform transform2 = transform;
				Vector3 eulerAngles2 = transform.eulerAngles;
				float x = eulerAngles2.x;
				Vector3 eulerAngles3 = transform.eulerAngles;
				transform2.eulerAngles = new Vector3(x, eulerAngles3.y, vaule);
			}, endVaule, duration);
		}

		public static Tweener DOShakeRotateZ(this Transform transform, float strength, int count, float duration)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float angle = eulerAngles.z;
			float fillAmount = 0f;
			return DOTween.To(() => fillAmount, delegate(float vaule)
			{
				Transform transform2 = transform;
				Vector3 eulerAngles2 = transform.eulerAngles;
				float x = eulerAngles2.x;
				Vector3 eulerAngles3 = transform.eulerAngles;
				transform2.eulerAngles = new Vector3(x, eulerAngles3.y, Mathf.Sin(vaule * 3.14159274f * (float)count) * strength + angle);
			}, 1f, duration);
		}

		public static BaseCard GetBaseCard(CardConfig config, bool random = true)
		{
			if (random || config.Index == 1)
			{
				if (config.CardType == CardType.Number || config.CardType == CardType.Swallowed || config.CardType == CardType.Scarecrow || config.CardType == CardType.Coin)
				{
					config.Index = HandCardSystem.Get().GetRandom();
				}
				else
				{
					config.Index = UnityEngine.Random.Range(1, 53);
				}
			}
			BaseCard baseCard = null;
			Type stringType = EnumUtility.GetStringType(config.CardType);
			GameObject gameObject = new GameObject(stringType.Name);
			baseCard = (BaseCard)gameObject.AddComponent(stringType);
			baseCard.OnStart(config);
			return baseCard;
		}

		public static Sequence DoReDealEffect(this Transform transform, Vector3 middlePosition, Vector3 lastPosition, Vector3 angle, float duration = 0.5f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOMove(middlePosition, duration * 0.5f));
			sequence.Join(transform.DORotate(UnityEngine.Random.insideUnitSphere * 360f, duration * 0.5f));
			sequence.Append(transform.DOMove(lastPosition, duration));
			sequence.Join(transform.DORotate(angle, duration * 0.5f));
			return sequence;
		}

		public static void DoClickEffect(this Transform transform, Vector3 lastPosition, TweenCallback tweenCallback = null, float jumpPower = 4f, int turns = 2, float duration = 0.5f)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			transform.position = new Vector3(x, position2.y, lastPosition.z);
			int num = 360 * turns;
			Vector3 position3 = transform.position;
			float z = num * ((position3.x > lastPosition.x) ? 1 : (-1));
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DORotate(new Vector3(0f, 0f, z), duration, RotateMode.FastBeyond360));
			sequence.Join(transform.DOJump(lastPosition, jumpPower, 1, duration));
			sequence.OnComplete(tweenCallback);
		}

		public static Tweener ShakeRotation(this Transform transform, float duration = 0.2f, float strength = 30f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
		{
			return transform.DOShakeRotation(duration, Vector3.forward * strength, vibrato, randomness, fadeOut).SetLoops(-1);
		}
	}
}
