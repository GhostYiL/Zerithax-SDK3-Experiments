
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.HandSwipeUI
{
    public class UIWindow : UdonSharpBehaviour
    {
        private Vector3 baseScale;

        private bool windowIsOpened;

        void Start()
        {
            //gameObject.SetActive(false);

            baseScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        public bool IsWindowOpened()
        {
            return windowIsOpened;
        }

        public void OpenWindow()
        {
            //gameObject.SetActive(true);

            windowIsOpened = true;

            transform.localScale = baseScale;
        }

        public void CloseWindow()
        {
            //gameObject.SetActive(false);

            windowIsOpened = false;

            transform.localScale = Vector3.zero;
        }
    }
}