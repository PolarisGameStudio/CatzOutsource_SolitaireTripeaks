namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;

    public class UIDailyReward : MonoBehaviour
    {
        public static event System.Action OnSpinCompleted = delegate { };
        [SerializeField]
        private int numberPieceOfSpin = 8;

        [SerializeField]
        private Transform spinWheelTF = null;
        [SerializeField]
        private float durationRoundWheel = 1.5f;

        private float angleOfPiece = 45;

        [SerializeField]
        private int pieceSelect = 1;
        public int PieceSelect
        {
            get { return pieceSelect; }
        }

        public void Start()
        {
            angleOfPiece = 360f / numberPieceOfSpin;

            PlaySpin();
        }

        public void PlaySpin()
        {
            pieceSelect = Random.Range(1, numberPieceOfSpin + 1);
            float _angleEnd = angleOfPiece * (pieceSelect - 1);

            spinWheelTF.localEulerAngles = Vector3.zero;
            Sequence seq = DOTween.Sequence();
            seq.Append(spinWheelTF.DORotate(new Vector3(0, 0, -(360 * 4 + _angleEnd)),
                durationRoundWheel * (4f + (float)pieceSelect / numberPieceOfSpin), RotateMode.FastBeyond360).SetRelative(true))
                .SetEase(Ease.InSine)
                .OnComplete(() => { OnSpinCompleted?.Invoke(); });
            //seq.Append(spinWheelTF.DORotate(new Vector3(0, 0, _angleEnd),
            //durationRoundWheel * 2 * _pieceSelect / numberPieceOfSpin, RotateMode.FastBeyond360).SetRelative(true)
            //.SetEase(Ease.InSine));
        }
    }
}