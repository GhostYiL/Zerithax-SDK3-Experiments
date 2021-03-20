using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using zerithax.StaticClasses;

namespace zerithax.HandSwipeUI
{
    public class DetectPlayerFingerSwipeForUI : UdonSharpBehaviour
    {
        [Header("Proportion Script")]
        [SerializeField] private StaticPlayerPorportions playerProportionScript;

        [Header("Main Values")]
        [SerializeField] private SaoUI uiObject;
        [SerializeField] private CalibratePlayerFingersForUI calibrator;
        [SerializeField] private float targetSwipeSpeed;
        [SerializeField] private float handSwipeCountdownTime;
        private float curHandSwipeCounter;
        [SerializeField] private float handStopCountdownTime;
        private float curHandStopCounter;

        [Header("Debugs")]
        [SerializeField] private bool debug;
        [SerializeField] private bool manualUISpawn;

        [Space(10)]
        [SerializeField] private TextMeshPro leftHandSwipeSpeed;
        [SerializeField] private TextMeshPro rightHandSwipeSpeed;

        [Space(10)]
        [SerializeField] private TextMeshPro leftIndexDebug;
        [SerializeField] private TextMeshPro leftMiddleDebug;
        [SerializeField] private TextMeshPro leftRingHalfDebug;
        [SerializeField] private TextMeshPro leftPinkyHalfDebug;

        [Space(10)]
        [SerializeField] private TextMeshPro rightIndexDebug;
        [SerializeField] private TextMeshPro rightMiddleDebug;
        [SerializeField] private TextMeshPro rightRingHalfDebug;
        [SerializeField] private TextMeshPro rightPinkyHalfDebug;

        [Space(10)]
        [SerializeField] private TextMeshPro requiredHandSwipeSpeedLeft;
        [SerializeField] private TextMeshPro requiredHandSwipeSpeedRight;


        private float leftIndexMostlyOpened;
        private float leftMiddleMostlyOpened;
        private float leftRingHalfClosed;
        private float leftPinkyHalfClosed;

        private float rightIndexMostlyOpened;
        private float rightMiddleMostlyOpened;
        private float rightRingHalfClosed;
        private float rightPinkyHalfClosed;


        private VRCPlayerApi localPlayer;
        private Vector3 leftHandPos;
        private Vector3 leftHandLastPos;

        private Vector3 rightHandPos;
        private Vector3 rightHandLastPos;

        void Start()
        {
            localPlayer = Networking.LocalPlayer;
        }

        private void Update()
        {
            //TODO (DONE ???): Wait to see if hand speed is 0 for a duration before activating spawn?
            //TODO (DONE): Z offset so UI spawns further from player hand 
            //TODO: Figure out why proportions for smaller people aren't working (smaller people don't have to swipe as far as large people to activate)


            if (calibrator.HasCalibrated())
            {
                //If left hand has fingers positioned correctly and swipes, add to counter
                if (CheckIfLeftFingersPositioned())
                {
                    bool isSwiping = LeftHandIsSwiping();

                    if (isSwiping) curHandSwipeCounter += Time.deltaTime;

                    //If I've swept long enough...
                    if (curHandSwipeCounter >= handSwipeCountdownTime && !isSwiping)
                    {
                        curHandStopCounter += Time.deltaTime;

                        //Make sure I've then also stopped swiping for a short period...
                        if (curHandStopCounter >= handStopCountdownTime)
                        {
                            curHandSwipeCounter = 0;
                            curHandStopCounter = 0;
                            Debug.Log("Time's up, directing UI to spawn at left hand");

                            //Spawn UI
                            uiObject.SpawnUI(localPlayer.GetPosition(), localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation, leftHandPos);
                        }
                    }
                }

                //If right hand has fingers positioned correclty and swipes, add to counter
                if (CheckIfRightFingersPositioned())
                {
                    bool isSwiping = RightHandIsSwiping();

                    if (isSwiping) curHandSwipeCounter += Time.deltaTime;

                    //If I've swept long enough...
                    if (curHandSwipeCounter >= handSwipeCountdownTime && !isSwiping)
                    {
                        curHandStopCounter += Time.deltaTime;

                        //Make sure I've then also stopped swiping for a short period...
                        if (curHandStopCounter >= handStopCountdownTime)
                        {
                            curHandSwipeCounter = 0;
                            curHandStopCounter = 0;
                            Debug.Log("Time's up, directing UI to spawn at right hand");

                            //Spawn UI
                            uiObject.SpawnUI(localPlayer.GetPosition(), localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation, rightHandPos);
                        }
                    }
                }

                if (!CheckIfLeftFingersPositioned() && !CheckIfRightFingersPositioned())
                {
                    curHandSwipeCounter = 0;
                    curHandStopCounter = 0;
                }

                if (manualUISpawn)
                {
                    Debug.Log("Debug UI Spawn detected...");
                    manualUISpawn = false;
                    uiObject.SpawnUI(localPlayer.GetPosition(), localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation, leftHandPos);
                    curHandSwipeCounter = 0;
                    curHandStopCounter = 0;
                }
            }
        }

        public void AssignCalibratedFingerPositions(float leftIndex, float leftMiddle, float leftRing, float leftPinky, float rightIndex, float rightMiddle, float rightRing, float rightPinky)
        {
            //Assign left hand's finger pos values
            leftIndexMostlyOpened = leftIndex;
            leftMiddleMostlyOpened = leftMiddle;
            leftRingHalfClosed = leftRing;
            leftPinkyHalfClosed = leftPinky;

            //Assign right hand's finger pos values
            rightIndexMostlyOpened = rightIndex;
            rightMiddleMostlyOpened = rightMiddle;
            rightRingHalfClosed = rightRing;
            rightPinkyHalfClosed = rightPinky;

            Debug.Log("Detector has received calibrated finger postiions");
        }

        private bool LeftHandIsSwiping()
        {
            leftHandPos = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);

            float swipeSpeed = (leftHandPos.y - leftHandLastPos.y) / Time.deltaTime;

            leftHandSwipeSpeed.text = "Left Swipe Cur: " + swipeSpeed.ToString("F4");

            leftHandLastPos = leftHandPos;

            float targetSwipeSpeedProportion = playerProportionScript.MultiplyFloatByPlayerHeightProportions(targetSwipeSpeed);
            requiredHandSwipeSpeedLeft.text = "Swipe Minimum: " + targetSwipeSpeedProportion.ToString("F4");

            if (swipeSpeed >= targetSwipeSpeedProportion) return true;
            else return false;
        }
        private bool RightHandIsSwiping()
        {
            rightHandPos = localPlayer.GetBonePosition(HumanBodyBones.RightHand);

            float swipeSpeed = (rightHandPos.y - rightHandLastPos.y) / Time.deltaTime;

            rightHandSwipeSpeed.text = "Right Swipe Cur: " + swipeSpeed.ToString("F4");

            rightHandLastPos = rightHandPos;

            float targetSwipeSpeedProportion = playerProportionScript.MultiplyFloatByPlayerHeightProportions(targetSwipeSpeed);
            requiredHandSwipeSpeedRight.text = "Swipe Minimum: " + targetSwipeSpeedProportion.ToString("F4");

            if (swipeSpeed >= targetSwipeSpeedProportion) return true;
            else return false;
        }

        private bool CheckIfLeftFingersPositioned()
        {
            Vector3 leftHandPosition = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);
            //Get left fingertips' current distance from hand
            float leftIndexDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.LeftIndexDistal) - leftHandPosition).sqrMagnitude;
            float leftMiddleDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.LeftMiddleDistal) - leftHandPosition).sqrMagnitude;
            float leftRingDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.LeftRingDistal) - leftHandPosition).sqrMagnitude;
            float leftPinkyDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.LeftLittleDistal) - leftHandPosition).sqrMagnitude;

            //If my left Index/Middle fingers are fully extended while the ring/pinky are half-closed, I'm correctly positioned!
            bool leftHandSatisfies = leftIndexDistanceFromHand >= leftIndexMostlyOpened
                /* && leftMiddleDistanceFromHand >= leftMiddleMostlyClosed */
                && leftRingDistanceFromHand <= leftRingHalfClosed
                && leftPinkyDistanceFromHand <= leftPinkyHalfClosed;

            if (debug)
            {
                leftIndexDebug.text = "Cur: " + leftIndexDistanceFromHand.ToString("F4") + " | Min: > " + leftIndexMostlyOpened.ToString("F4");
                leftMiddleDebug.text = "Cur: " + leftMiddleDistanceFromHand.ToString("F4") + " | Min: > " + leftMiddleMostlyOpened.ToString("F4");
                leftRingHalfDebug.text = "Cur: " + leftRingDistanceFromHand.ToString("F4") + " | Max: < " + leftRingHalfClosed.ToString("F4");
                leftPinkyHalfDebug.text = "Cur: " + leftPinkyDistanceFromHand.ToString("F4") + " | Max: < " + leftPinkyHalfClosed.ToString("F4");
            }

            //Return true if left hand is correctly positioned
            return leftHandSatisfies;
        }

        private bool CheckIfRightFingersPositioned()
        {
            Vector3 rightHandPosition = localPlayer.GetBonePosition(HumanBodyBones.RightHand);
            //Get right fingertips' current distance from hand
            float rightIndexDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.RightIndexDistal) - rightHandPosition).sqrMagnitude;
            float rightMiddleDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.RightMiddleDistal) - rightHandPosition).sqrMagnitude;
            float rightRingDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.RightRingDistal) - rightHandPosition).sqrMagnitude;
            float rightPinkyDistanceFromHand = (localPlayer.GetBonePosition(HumanBodyBones.RightLittleDistal) - rightHandPosition).sqrMagnitude;

            //If my right Index/Middle fingers are fully extended while the ring/pinky are half-closed, I'm correctly positioned!
            bool rightHandSatisfies = rightIndexDistanceFromHand >= rightIndexMostlyOpened
                /* && rightMiddleDistanceFromHand >= rightMiddleMostlyClosed */
                && rightRingDistanceFromHand <= rightRingHalfClosed
                && rightPinkyDistanceFromHand <= rightPinkyHalfClosed;

            if (debug)
            {
                rightIndexDebug.text = "Cur: " + rightIndexDistanceFromHand.ToString("F4") + " | Min: > " + rightIndexMostlyOpened.ToString("F4");
                rightMiddleDebug.text = "Cur: " + rightMiddleDistanceFromHand.ToString("F4") + " | Min: > " + rightMiddleMostlyOpened.ToString("F4");
                rightRingHalfDebug.text = "Cur: " + rightRingDistanceFromHand.ToString("F4") + " | Max: < " + rightRingHalfClosed.ToString("F4");
                rightPinkyHalfDebug.text = "Cur: " + rightPinkyDistanceFromHand.ToString("F4") + " | Max: < " + rightPinkyHalfClosed.ToString("F4");
            }

            //Return true if right hand is correctly positioned
            return rightHandSatisfies;
        }

        private bool ApproximateWithThreshold(float a, float b, float threshold)
        {
            return Mathf.Abs(a - b) <= threshold;
        }
    }
}