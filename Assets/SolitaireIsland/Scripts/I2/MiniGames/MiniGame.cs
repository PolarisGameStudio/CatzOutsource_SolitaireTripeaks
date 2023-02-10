using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace I2.MiniGames
{
	public class MiniGame : MonoBehaviour
	{
		public Transform _Rewards;

		[Rename("Start On Enabled", null)]
		public bool _SetupGameOnEnable;

		public UnityEvent _OnStart = new UnityEvent();

		[NonSerialized]
		[HideInInspector]
		public List<MiniGame_Reward> mRewards = new List<MiniGame_Reward>();

		public virtual void SetupGame()
		{
			ApplyLayout();
			_OnStart.Invoke();
		}

		public virtual void ApplyLayout()
		{
			MiniGame_Reward[] componentsInChildren = _Rewards.GetComponentsInChildren<MiniGame_Reward>(includeInactive: true);
			mRewards = componentsInChildren.ToList();
			FilterRewards();
			MiniGame_Reward[] array = componentsInChildren;
			foreach (MiniGame_Reward miniGame_Reward in array)
			{
				miniGame_Reward.Hide();
			}
			mRewards.RemoveAll((MiniGame_Reward r) => r.Probability <= 0f);
		}

		public virtual void FilterRewards()
		{
		}

		public virtual bool CanPlayAnotherRound()
		{
			return true;
		}

		public void TryPlayingRound()
		{
			StartRound();
		}

		public virtual void StartRound()
		{
		}

		public virtual void CancelRound()
		{
		}

		public void OnEnable()
		{
			if (_SetupGameOnEnable)
			{
				Invoke("SetupGame", 0f);
			}
		}

		public virtual MiniGame_Reward GetRandomReward()
		{
			if (mRewards.Count <= 0)
			{
				return null;
			}
			int num = Mathf.Max(0, NumChoices() - mRewards.Count);
			float max = (float)num + mRewards.Sum((MiniGame_Reward e) => e.Probability);
			float rnd = UnityEngine.Random.Range(0f, max);
			MiniGame_Reward miniGame_Reward = mRewards.Find(delegate(MiniGame_Reward e)
			{
				rnd -= e.Probability;
				return e.Probability > 0f && rnd <= 0f;
			});
			if ((bool)miniGame_Reward)
			{
				mRewards.Remove(miniGame_Reward);
			}
			return miniGame_Reward;
		}

		public virtual int NumChoices()
		{
			return 0;
		}

		public void SetRewardPriority(string RewardName, float Probability)
		{
			MiniGame_Reward miniGame_Reward = mRewards.Find((MiniGame_Reward r) => r.name == RewardName);
			if (miniGame_Reward == null)
			{
				miniGame_Reward = _Rewards.GetComponentsInChildren<MiniGame_Reward>(includeInactive: true).First((MiniGame_Reward r) => r.name == RewardName);
			}
			if (miniGame_Reward != null)
			{
				miniGame_Reward.Probability = Probability;
				if (Probability > 0f && !mRewards.Contains(miniGame_Reward))
				{
					mRewards.Add(miniGame_Reward);
				}
				if (Probability <= 0f && mRewards.Contains(miniGame_Reward))
				{
					mRewards.Remove(miniGame_Reward);
				}
			}
		}

		public void EnableGameObject(GameObject go)
		{
			go.SetActive(value: true);
		}

		public void DeactivateGameObject(GameObject go)
		{
			go.SetActive(value: false);
		}
	}
}
