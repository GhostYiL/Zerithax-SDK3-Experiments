
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.HandSwipeUI
{
    public class UIButtonSpriteManager : UdonSharpBehaviour
    {
        [SerializeField] private UIButton[] uIButtons;



        public bool CheckIfButtonSelected()
        {
            foreach (UIButton button in uIButtons)
            {
                if (button.ButtonIsSelected()) return true;
            }

            return false;
        }
        
        public void NotifyButtonSelected(UIButton selectedButton)
        {
            foreach (UIButton button in uIButtons)
            {
                if (button != selectedButton)
                {
                    button.SetUnselected();
                }
            }
        }

        public void NotifyButtonUnselected(UIButton unselectedButton)
        {
            bool selectedButtonExists = false;
            foreach (UIButton button in uIButtons)
            {
                if (button.ButtonIsSelected())
                {
                    selectedButtonExists = true;
                    break;
                }
            }

            if (!selectedButtonExists)
            {
                foreach (UIButton button in uIButtons)
                {
                    button.SetNeutral();
                }
            }
        }
    }
}