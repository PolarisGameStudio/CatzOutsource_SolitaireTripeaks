using DG.Tweening;
using DragonBones;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class Seagull : BaseAdditional
	{
		public enum SeagullType
		{
			Coins,
			Poker
		}

		public UnityArmatureComponent ArmatureComponent;

		private SeagullType seagullType;

		private int Numbers;

		public override void UpdateLayer(int index)
		{
			if (ArmatureComponent != null)
			{
				ArmatureComponent.sortingOrder = 2000;
			}
		}

		private void Start()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOPath(GetRandomPaths(), 20f, PathType.CatmullRom, PathMode.Sidescroller2D).SetEase(Ease.Linear).SetLookAt(0.001f));
			sequence.SetId(GetInstanceID());
			sequence.OnComplete(delegate
			{
				PlayAdditional.Get().Remove(this);
			});
		}

		private Vector3[] GetRandomPaths()
		{
			Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1f));
			a -= new Vector3(0.3f, 0.2f);
			List<Vector3> list = new List<Vector3>();
			list.Add(base.transform.position);
			List<Vector3> list2 = list;
			int num = Random.Range(5, 10);
			for (int i = 0; i < num; i++)
			{
				Vector3 item = list2[list2.Count - 1];
				if (item.x > 0f)
				{
					item.x = Random.Range(0f - a.x, 0f);
				}
				else
				{
					item.x = Random.Range(0f, a.x);
				}
				item.y = Random.Range(0f - a.y, a.y);
				list2.Add(item);
			}
			list2.Add(RandomScreenOutsidePoint());
			return list2.ToArray();
		}

		public override bool OnClick()
		{
			GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Particles/StarGetParticle"));
			gameObject.transform.position = base.transform.position;
			UnityEngine.Object.Destroy(gameObject, 4f);
			AudioUtility.GetSound().Play("Audios/clickBird.mp3");
			switch (seagullType)
			{
			case SeagullType.Coins:
				SingletonClass<OnceGameData>.Get().StreaksCoins += Numbers;
				SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(base.transform.position, LocalizationUtility.Get().GetString("Bonus"), Numbers.ToString(), null);
				SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, Numbers);
				break;
			case SeagullType.Poker:
				if (PlayDesk.Get() != null && !PlayDesk.Get().IsGameOver)
				{
					HandCardSystem.Get().AppendLeftCards(Numbers);
					SingletonBehaviour<Effect2DUtility>.Get().CreateText(string.Format(LocalizationUtility.Get().GetString("+N Cards"), Numbers), base.transform.position);
				}
				break;
			}
			PlayAdditional.Get().Remove(this);
			return true;
		}

		public override void OnRemove()
		{
			base.OnRemove();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public override void OnOver()
		{
			DOTween.Kill(GetInstanceID());
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOPath(new Vector3[2]
			{
				base.transform.position,
				RandomScreenOutsidePoint()
			}, 2f, PathType.CatmullRom, PathMode.Sidescroller2D).SetEase(Ease.Linear).SetLookAt(0.001f));
			sequence.SetId(GetInstanceID());
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}

		public static void CreateSeagulls(string extraContent)
		{
			PlayDesk playDesk = Object.FindObjectOfType<PlayDesk>();
			if (playDesk == null || playDesk.IsGameOver)
			{
				return;
			}
			string[] array = extraContent.Split(',');
			int num = (Object.FindObjectOfType<DoubleSeagullBooster>() == null) ? 1 : 2;
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('-');
				if (array2.Length != 2)
				{
					continue;
				}
				for (int j = 0; j < num; j++)
				{
					Seagull component = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/Additionals/Seagull")).GetComponent<Seagull>();
					component.transform.position = RandScreenEdgePoint();
					if (array2[0] == "1")
					{
						component.seagullType = SeagullType.Coins;
					}
					else
					{
						component.seagullType = SeagullType.Poker;
					}
					component.Numbers = int.Parse(array2[1]);
					PlayAdditional.Get().Append(component);
				}
			}
		}

		private static Vector3 RandomScreenOutsidePoint()
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1f));
			if (Random.Range(0, 100) % 2 == 0)
			{
				return new Vector3(Random.Range(vector.x * 1.2f, vector.x * 1.5f) * (float)((Random.Range(0, 100) % 2 == 0) ? 1 : (-1)), Random.Range(0f - vector.y, vector.y));
			}
			return new Vector3(Random.Range(0f - vector.x, vector.x), Random.Range(vector.y * 1.2f, vector.y * 1.5f) * (float)((Random.Range(0, 100) % 2 == 0) ? 1 : (-1)));
		}

		private static Vector3 RandScreenEdgePoint()
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1f));
			if (Random.Range(0, 100) % 2 == 0)
			{
				return new Vector3(Random.Range(vector.x * 1.1f, vector.x * 1.2f) * (float)((Random.Range(0, 100) % 2 == 0) ? 1 : (-1)), Random.Range(0f - vector.y, vector.y));
			}
			return new Vector3(Random.Range(0f - vector.x, vector.x), Random.Range(vector.y * 1.1f, vector.y * 1.2f) * (float)((Random.Range(0, 100) % 2 == 0) ? 1 : (-1)));
		}
	}
}
