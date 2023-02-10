using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace I2.MiniGames
{
	[AddComponentMenu("I2/MiniGames/Wheel/Game")]
	public class PrizeWheel : MiniGame
	{
		public enum eSelectorDirection
		{
			top,
			right,
			bottom,
			left
		}

		[Header("Selector")]
		public RectTransform _Selector;

		[Rename("Direction", null)]
		public eSelectorDirection _SelectorDirection;

		[Rename("Anchor To Center", null)]
		public bool _AnchorSelectorToCenter;

		[Rename("Elastic Duration", null)]
		public float _SelectorElasticDuration = 1f;

		[Rename("Elastic Amplitud", null)]
		public float _SelectorElasticAmplitud;

		[Rename("Elastic Period", null)]
		public float _SelectorElasticPeriod;

		[Header("Wheel")]
		public Transform _Wheel;

		public float _Elements_Spread;

		public float _Elements_Offset;

		[Rename("Equal Distribution", null)]
		public bool _Elements_EqualDistribution;

		[Header("Simulation")]
		public bool _CenterOnElement;

		public bool _RotateSelector;

		public float _Rotation_MinCycles = 2f;

		public float _Rotation_MaxCycles = 4f;

		public float _Rotation_MinTime = 3f;

		public float _Rotation_MaxTime = 4f;

		public float _SpeedUpTime = 0.2f;

		public float _Friction = 5f;

		public UnityEvent _OnStartSpinning = new UnityEvent();

		public UnityEventInt _OnFinishSpinning = new UnityEventInt();

		[Header("Timing")]
		public float _TimeCollectReward;

		public float _TimeNewRound;

		private bool mIsPlaying;

		private float mSelectorMaxAngle;

		private float mWheelMaxAngleForKnot;

		private float mSelectorElasticStartTime;

		private float mSelectorElasticStartAngle;

		private int mForceReward = -1;

		public override void ApplyLayout()
		{
			mForceReward = -1;
			base.ApplyLayout();
			int count = mRewards.Count;
			float num = (!_Elements_EqualDistribution) ? mRewards.Sum((MiniGame_Reward r) => r.Probability) : ((float)count);
			float num2 = 0f;
			bool flag = true;
			float num3 = 0f;
			_Wheel.localRotation = Quaternion.identity;
			foreach (PrizeWheel_Reward mReward in mRewards)
			{
				if ((bool)mReward._Separator)
				{
					mReward._Separator.gameObject.SetActive(value: true);
				}
				float num4 = (!_Elements_EqualDistribution) ? mReward.Probability : 1f;
				float num5 = 360f * num4 / num;
				if (flag)
				{
					num2 -= num5 / 2f;
					num3 = num2;
					flag = false;
				}
				mReward.ApplyLayout(num2, num5 - _Elements_Spread, this);
				num2 += num5;
			}
			if ((bool)_Selector)
			{
				float num6 = 90 * (int)_SelectorDirection;
				if (_AnchorSelectorToCenter)
				{
					_Selector.localRotation = Quaternion.Euler(0f, 0f, 180f - num6);
					_Selector.localPosition = Vector3.zero;
				}
				else
				{
					_Selector.localRotation = Quaternion.Euler(0f, 0f, 0f - num6);
					_Selector.localPosition = Quaternion.Euler(0f, 0f, 0f - num6) * (Vector3.up * _Selector.localPosition.magnitude);
				}
				mSelectorElasticStartTime = -1000f;
				Vector3 eulerAngles = _Selector.rotation.eulerAngles;
				mSelectorElasticStartAngle = eulerAngles.z;
			}
		}

		public override void FilterRewards()
		{
			base.FilterRewards();
			mRewards.RemoveAll((MiniGame_Reward r) => !r.gameObject.activeInHierarchy);
		}

		public void StartSpinning()
		{
			StartSpinning(-1);
		}

		public void StartSpinning(int forceReward)
		{
			mForceReward = forceReward;
			TryPlayingRound();
		}

		public override void StartRound()
		{
			_OnStartSpinning.Invoke();
			GetRandomElement(ref mForceReward, out float Angle);
			StartCoroutine(DoPlay(mForceReward, Angle));
			mForceReward = -1;
		}

		public void StopPlay()
		{
			mIsPlaying = false;
		}

		private void GetRandomElement(ref int ElementIdx, out float Angle)
		{
			int num = ElementIdx;
			ElementIdx = 0;
			Angle = 0f;
			float num2 = mRewards.Sum((MiniGame_Reward r) => r.Probability);
			float num3 = (num < 0) ? (Random.value * num2) : (num2 + 1f);
			float num4 = 90 * (int)_SelectorDirection;
			if (_RotateSelector)
			{
				Angle = num4 + (float)(_AnchorSelectorToCenter ? 180 : 0);
			}
			else
			{
				Angle = 0f;
			}
			bool flag = true;
			float num5 = (!_Elements_EqualDistribution) ? num2 : ((float)mRewards.Count);
			int num6 = 0;
			int count = mRewards.Count;
			float num8;
			while (true)
			{
				if (num6 < count)
				{
					MiniGame_Reward miniGame_Reward = mRewards[num6];
					float num7 = (!_Elements_EqualDistribution) ? miniGame_Reward.Probability : 1f;
					num8 = 360f * num7 / num5;
					if (flag)
					{
						Angle -= num8 / 2f;
						flag = false;
					}
					num3 -= miniGame_Reward.Probability;
					if (num3 < 0f || (num >= 0 && num6 == num))
					{
						break;
					}
					Angle += num8;
					num6++;
					continue;
				}
				return;
			}
			Angle += mWheelMaxAngleForKnot;
			if (_CenterOnElement)
			{
				Angle += 0.5f * num8;
			}
			else
			{
				Angle += Mathf.Lerp(0.2f, 0.8f, Random.value) * (num8 - _Elements_Spread - mWheelMaxAngleForKnot);
			}
			ElementIdx = num6;
		}

		private IEnumerator DoPlay(int TargetElementIdx, float ElementAngle)
		{
			if (mIsPlaying)
			{
				yield break;
			}
			mIsPlaying = true;
			float InitialAngle;
			if (_RotateSelector && (bool)_Selector)
			{
				Vector3 eulerAngles = _Selector.rotation.eulerAngles;
				InitialAngle = 0f - eulerAngles.z;
			}
			else
			{
				Vector3 eulerAngles2 = _Wheel.rotation.eulerAngles;
				InitialAngle = eulerAngles2.z;
			}
			float TotalRotation2 = Mathf.DeltaAngle(InitialAngle, ElementAngle);
			if (TotalRotation2 < 0f)
			{
				TotalRotation2 += 360f;
			}
			float NumCycles = Random.Range(_Rotation_MinCycles, _Rotation_MaxCycles);
			TotalRotation2 += (float)(Mathf.CeilToInt(NumCycles) * 360);
			float TotalTime = Random.Range(_Rotation_MinTime, _Rotation_MaxTime);
			float InitialTime = Time.time;
			float centerTime = _SpeedUpTime;
			while (true)
			{
				bool finished = UpdateRotation(InitialAngle, TotalRotation2, InitialTime, TotalTime, centerTime);
				foreach (MiniGame_Reward mReward in mRewards)
				{
					((PrizeWheel_Reward)mReward).SpinningUpdate();
				}
				if (finished)
				{
					break;
				}
				yield return null;
				if (!mIsPlaying)
				{
					yield break;
				}
			}
			mIsPlaying = false;
			_OnFinishSpinning.Invoke(TargetElementIdx);
			if (_TimeCollectReward >= 0f)
			{
				if (_TimeCollectReward > 0f)
				{
					yield return new WaitForSeconds(_TimeCollectReward);
				}
				MiniGame_Reward reward = mRewards[TargetElementIdx];
				reward.Execute(this, null);
			}
		}

		private bool UpdateRotation(float initialAngle, float totalRotation, float initialTime, float totalTime, float speedUpTime)
		{
			float num = Time.time - initialTime;
			float num2 = Mathf.Clamp01(num / totalTime);
			float num3 = speedUpTime / totalTime;
			if (num2 < num3)
			{
				float f = num2 / num3;
				num2 = Mathf.Pow(f, _Friction) * num3;
			}
			else
			{
				float num4 = 1f - num3;
				float num5 = (num2 - num3) / num4;
				num2 = 1f - Mathf.Pow(1f - num5, _Friction);
				num2 = num2 * num4 + num3;
			}
			float num6 = initialAngle + num2 * totalRotation;
			if (Mathf.Abs(num6 - (initialAngle + totalRotation)) < 0.5f)
			{
				num6 = initialAngle + totalRotation;
				num2 = 1f;
			}
			if (_RotateSelector && (bool)_Selector)
			{
				float magnitude = _Selector.localPosition.magnitude;
				Quaternion rotation = Quaternion.Euler(0f, 0f, 0f - num6);
				if (_AnchorSelectorToCenter)
				{
					_Selector.rotation = rotation;
				}
				else
				{
					_Selector.localPosition = rotation * (Vector3.up * magnitude);
				}
			}
			else
			{
				_Wheel.localRotation = Quaternion.Euler(0f, 0f, num6);
			}
			return num2 >= 1f;
		}

		private float GetElasticOut(float t, float a, float b, float amplitude, float period)
		{
			if (t <= 0f)
			{
				return a;
			}
			if (t >= 1f)
			{
				return b;
			}
			if (period == 0f)
			{
				period = 0.3f;
			}
			float num = Mathf.DeltaAngle(a, b);
			float num2;
			if (amplitude == 0f || (num > 0f && amplitude < num) || (num < 0f && amplitude < 0f - num))
			{
				amplitude = num;
				num2 = period / 4f;
			}
			else
			{
				num2 = period / 6.28318548f * Mathf.Asin(num / amplitude);
			}
			return amplitude * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - num2) * 6.28318548f / period) + num + a;
		}
	}
}
