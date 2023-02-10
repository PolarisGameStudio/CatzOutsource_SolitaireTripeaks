using Nightingale.Inputs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class PlayAdditional : MonoBehaviour
	{
		private static PlayAdditional _PlayAdditional;

		private List<BaseAdditional> BaseAdditionals = new List<BaseAdditional>();

		public static PlayAdditional Get()
		{
			if (_PlayAdditional == null)
			{
				UnityEngine.Debug.Log($"请先添加脚本:{typeof(PlayAdditional).GetType().FullName}，再进行游戏。");
			}
			return _PlayAdditional;
		}

		private void Awake()
		{
			_PlayAdditional = this;
		}

		private void Start()
		{
			FindObjectsWithClick.Get().Append(OnFindObjects, 3);
		}

		private void OnDestroy()
		{
			FindObjectsWithClick.Get().Remove(OnFindObjects);
		}

		private bool OnFindObjects(Transform[] transforms)
		{
			foreach (Transform transform in transforms)
			{
				BaseAdditional component = transform.gameObject.GetComponent<BaseAdditional>();
				if (component != null)
				{
					component.OnClick();
					return true;
				}
			}
			foreach (Transform transform2 in transforms)
			{
				CardAdditional cardAdditional = FindCardAdditional(transform2.gameObject.GetComponent<BaseCard>());
				if (cardAdditional != null)
				{
					cardAdditional.OnClick();
					return true;
				}
			}
			return false;
		}

		private CardAdditional FindCardAdditional(BaseCard baseCard)
		{
			if (baseCard == null)
			{
				return null;
			}
			BaseAdditional[] array = (from e in BaseAdditionals
				where e is CardAdditional
				select e).ToArray();
			BaseAdditional[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				CardAdditional cardAdditional = (CardAdditional)array2[i];
				if (cardAdditional.baseCard != null && cardAdditional.baseCard == baseCard)
				{
					return cardAdditional;
				}
			}
			return null;
		}

		public void Append(BaseAdditional baseAdditional)
		{
			baseAdditional.transform.SetParent(base.transform, worldPositionStays: true);
			baseAdditional.UpdateLayer(BaseAdditionals.Count);
			BaseAdditionals.Add(baseAdditional);
		}

		public void Remove(BaseAdditional baseAdditional)
		{
			BaseAdditionals.Remove(baseAdditional);
			baseAdditional.OnRemove();
		}

		public void Over()
		{
			BaseAdditionals.ToList().ForEach(delegate(BaseAdditional additional)
			{
				additional.OnOver();
			});
			BaseAdditionals.Clear();
		}

		public InsectKing FindInsectKing()
		{
			foreach (BaseAdditional baseAdditional in BaseAdditionals)
			{
				if (baseAdditional is InsectKing)
				{
					return (InsectKing)baseAdditional;
				}
			}
			return null;
		}
	}
}
