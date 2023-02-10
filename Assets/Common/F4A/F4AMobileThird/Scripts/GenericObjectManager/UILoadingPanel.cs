namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UILoadingPanel : MonoBehaviour
    {
        [SerializeField]
        private bool isShowLoading = false;
        public bool IsShowLoading
        {
            get { return isShowLoading; }
        }

        public void SetActiveLoading(bool isValue)
        {
            isShowLoading = isValue;
            gameObject.SetActive(isShowLoading);
        }
    }
}