using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IslandLineEffect : MonoBehaviour
{
	public Color32 _grayColor32 = new Color32(167, 179, 182, byte.MaxValue);

	public Color32 _lightColor32 = new Color32(byte.MaxValue, 238, 8, byte.MaxValue);

	public void SetLock(bool isLock)
	{
		Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>();
		Image[] array = componentsInChildren;
		foreach (Image image in array)
		{
			image.color = ((!isLock) ? _lightColor32 : _grayColor32);
		}
		if (!isLock)
		{
			for (int j = 0; j < base.transform.childCount; j++)
			{
				Transform _transform = base.transform.GetChild(j);
				Sequence seq = DOTween.Sequence();
				seq.AppendInterval((float)j * 0.1f);
				seq.SetEase(Ease.Linear);
				seq.OnComplete(delegate
				{
					seq = DOTween.Sequence();
					Sequence s = seq;
					Transform target = _transform;
					Vector3 localPosition = _transform.localPosition;
					s.Append(target.DOLocalMoveX(localPosition.x + 30f, 1f));
					Sequence s2 = seq;
					Transform target2 = _transform;
					Vector3 localPosition2 = _transform.localPosition;
					s2.Append(target2.DOLocalMoveX(localPosition2.x, 1f));
					seq.SetEase(Ease.Linear);
					seq.SetLoops(-1);
				});
			}
		}
	}
}
