
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using zerithax.HandSwipeUI;

namespace zerithax.HandSwipeUI
{
    public class UIButton : UdonSharpBehaviour
    {
        [SerializeField] private UIButtonSpriteManager spriteManager;
        private bool isSelected = false;
        private bool isNeutral = true;
        private bool isHovered = false;

        [Header("Windows")]
        [SerializeField] private UIWindow parentWindow;
        [SerializeField] private UIWindow[] windowsToOpen;

        [Header("Sprites")]
        [SerializeField] private Sprite neutralSprite;
        private bool hasNeutralSprite;
        [SerializeField] private Sprite hoveredSprite;
        private bool hasHoveredSprite;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite unselectedSprite;
        private SpriteRenderer spRenderer;

        [Header("Finger Tracking")]
        //TODO: Split range into separate X and Y floats, to work with various shapes of buttons! 
        [SerializeField] private float rangeX;
        [SerializeField] private float rangeY;
        [SerializeField] private float buttonPressRange;
        [SerializeField] private float buttonHoverRange;
        [SerializeField] private Transform leftIndexTracker;
        [SerializeField] private Transform rightIndexTracker;
        private bool leftFingerInRangeToPress;
        private bool rightFingerInRangeToPress;

        [Header("Optional Encaser")]
        [SerializeField] private SpriteRenderer encaser;
        private bool hasEncaser;

        [Header("Debug")]
        [SerializeField] private bool selectedDebug;
        [SerializeField] private bool debugFingerPos;

        private void Start()
        {
            //hasIcon = icon != null ? true : false;
            hasEncaser = encaser != null;
            hasNeutralSprite = neutralSprite != null;
            hasHoveredSprite = hoveredSprite != null;
            spRenderer = gameObject.GetComponent<SpriteRenderer>();

            if (hasHoveredSprite) Debug.Log("'" + gameObject.name + "' has a hovered sprite! Will check for hover.");
        }

        private void Update()
        {
            //We don't have a parent window? Always check for pressing.
            if (parentWindow == null)
            {
                //SetButtonStateByLeftFingerPos();
                SetButtonStateByRightFingerPos();
            }
            else
            {
                //If my parent window is opened, start checking for pressing!
                if (parentWindow.IsWindowOpened())
                {
                    //SetButtonStateByLeftFingerPos();
                    SetButtonStateByRightFingerPos();
                }
                else
                {
                    //Unselect self if the window I belong to is closed
                    if (isSelected) SetUnselected();
                }

                //If (ButtonHovered() && neutral?)

                //Button checks aren't run here because parent window exists but isn't currently opened
            }


            //If debug box ticked, select button on/off
            if (selectedDebug)
            {
                selectedDebug = false;
                if (isSelected) SetUnselected();
                else SetSelected();
            }
        }

        private void SetButtonStateByLeftFingerPos()
        {
            //Get finger's distance from button if aligned along XY
            float curDistance = CheckFingerDistance(leftIndexTracker);

            //If finger is close enough to press the button...
            if (curDistance != 0 && curDistance <= buttonPressRange)
            {
                //Make sure we haven't already pressed the button, then press it
                if (!leftFingerInRangeToPress)
                {
                    if (!isSelected) SetSelected();
                    else SetUnselected();
                    leftFingerInRangeToPress = true;

                    Debug.Log("I've pressed button '" + gameObject.name + "'!");
                }

                //Do nothing; we have to be out of range after pressing before we can press it again!
            }
            //Out of range again? Set that.
            else leftFingerInRangeToPress = false;


            //Only do this if we have a sprite for hovering!
            if (hasHoveredSprite)
            {
                //Verify we are within range to activate a hover
                if (curDistance > buttonPressRange && curDistance <= buttonHoverRange)
                {
                    //Verify the button is neutral (cuz hover only when no sibling buttons are selected too, which only happens when Neutral). If neutral, we're obviously not selected, so we don't need to check for that.
                    if (isNeutral)
                    {
                        SetHovered();

                        Debug.Log("'" + gameObject.name + "' button has been hovered!");
                    }
                }
                //Not in range to hover? If we've already hovered and we haven't reached far enough to select, then set back to Neutral!
                else if (isHovered && !isSelected)
                {
                    SetNeutral();

                    Debug.Log("'" + gameObject.name + "' hovered button has returned to neutral!");
                }
            }
        }
        private void SetButtonStateByRightFingerPos()
        {
            //Get finger's distance from button if aligned along XY
            float curDistance = CheckFingerDistance(rightIndexTracker);

            //If finger is close enough to press the button...
            if (curDistance != 0 && curDistance <= buttonPressRange)
            {
                //Make sure we haven't already pressed the button, then press it
                if (!rightFingerInRangeToPress)
                {
                    if (!isSelected) SetSelected();
                    else SetUnselected();
                    rightFingerInRangeToPress = true;

                    Debug.Log("I've pressed button '" + gameObject.name + "'!");
                }

                //Do nothing; we have to be out of range after pressing before we can press it again!
            }
            //Out of range again? Set that.
            else rightFingerInRangeToPress = false;


            //Only do this if we have a sprite for hovering!
            if (hasHoveredSprite)
            {
                //Verify we are within range to activate a hover
                if (curDistance > buttonPressRange && curDistance <= buttonHoverRange)
                {
                    //Verify the button is neutral (cuz hover only when no sibling buttons are selected too, which only happens when Neutral). If neutral, we're obviously not selected, so we don't need to check for that.
                    if (isNeutral)
                    {
                        SetHovered();

                        Debug.Log("'" + gameObject.name + "' button has been hovered!");
                    }
                }
                //Not in range to hover? If we've already hovered and we haven't reached far enough to select, then set back to Neutral!
                else if (isHovered && !isSelected)
                {
                    SetNeutral();

                    Debug.Log("'" + gameObject.name + "' hovered button has returned to neutral!");
                }
            }
        }

        public bool ButtonIsSelected()
        {
            return isSelected;
        }

        private float CheckFingerDistance(Transform finger)
        {
            Vector3 localToFingertip = transform.InverseTransformPoint(finger.position);

            if (debugFingerPos)
            {
                Debug.Log("Finger X local to Button: " + localToFingertip.x);
                Debug.Log("Finger y local to Button: " + localToFingertip.y);
            }

            //If we are within the specified X and Y ranges, return our depth (distance from button on Z axis), otherwise return 0 (essentially null)
            if (Mathf.Abs(localToFingertip.x) < rangeX && Mathf.Abs(localToFingertip.y) < rangeY) return -localToFingertip.z;
            else return 0;
        }

        //four methods: private select, private hover, public unselect, and public neutral

        private void SetHovered()
        {
            spRenderer.sprite = hoveredSprite;

            isSelected = false;
            isNeutral = false;
            isHovered = true;
        }

        private void SetSelected()
        {
            spRenderer.sprite = selectedSprite;

            isSelected = true;
            isNeutral = false;
            isHovered = false;

            spriteManager.NotifyButtonSelected(this);

            if (hasEncaser) encaser.enabled = false;

            //Open the windows associated with this button
            foreach (UIWindow window in windowsToOpen)
            {
                window.OpenWindow();
            }
        }

        public void SetUnselected()
        {
            spRenderer.sprite = unselectedSprite;

            isSelected = false;
            isNeutral = false;
            isHovered = false;

            spriteManager.NotifyButtonUnselected(this);

            //Close the windows associated with this button
            foreach (UIWindow window in windowsToOpen)
            {
                window.CloseWindow();
            }
        }

        public void SetNeutral()
        {
            if (hasNeutralSprite) spRenderer.sprite = neutralSprite;
            else spRenderer.sprite = unselectedSprite;

            isSelected = false;
            isNeutral = true;
            isHovered = false;

            if (hasEncaser) encaser.enabled = true;
        }
    }
}