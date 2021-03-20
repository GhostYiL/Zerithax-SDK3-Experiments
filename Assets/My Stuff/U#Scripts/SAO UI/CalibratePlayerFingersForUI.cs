
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using zerithax.HandSwipeUI;

public class CalibratePlayerFingersForUI : UdonSharpBehaviour
{
    [Header("Main Variables")]
    [SerializeField] private DetectPlayerFingerSwipeForUI detector;
    [SerializeField] private TextMeshPro calibrationCountdownText;

    [SerializeField] private float calibratorCountdownTime;
    private float curCalibratorCountdown;
    private bool startCalibrateCountdown;


    [SerializeField] private bool hasCalibrated;
    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    private void Update()
    {
        //Calibrate if activated
        CalibrateCountdown();
    }

    public bool HasCalibrated()
    {
        return hasCalibrated;
    }

    //Main methods
    private void CalibrateHands()
    {
        Vector3 leftHandPosition = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);
        //Get left fingertips' current distance from hand
        float leftIndexMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.LeftIndexDistal) - leftHandPosition).sqrMagnitude;
        float leftMiddleMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.LeftMiddleDistal) - leftHandPosition).sqrMagnitude;
        float leftRingMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.LeftRingDistal) - leftHandPosition).sqrMagnitude;
        float leftPinkyMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.LeftLittleDistal) - leftHandPosition).sqrMagnitude;

        //The distance to check "if mostly opened" for the index and middle finger is 3 quarters their distance from the hand
        float leftIndexMostlyClosed = (leftIndexMaxDistance / 4) * 3;
        float leftMiddleMostlyClosed = (leftMiddleMaxDistance / 4) * 3;

        //The distance to check "if closed" for the ring and pinky finger is half their distance from the hand
        float leftRingHalfClosed = leftRingMaxDistance / 2;
        float leftPinkyHalfClosed = leftPinkyMaxDistance / 2;


        Vector3 rightHandPosition = localPlayer.GetBonePosition(HumanBodyBones.RightHand);
        //Get right fingertips' current distance from hand
        float rightIndexMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.RightIndexDistal) - rightHandPosition).sqrMagnitude;
        float rightMiddleMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.RightMiddleDistal) - rightHandPosition).sqrMagnitude;
        float rightRingMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.RightRingDistal) - rightHandPosition).sqrMagnitude;
        float rightPinkyMaxDistance = (localPlayer.GetBonePosition(HumanBodyBones.RightLittleDistal) - rightHandPosition).sqrMagnitude;

        //The distance to check "if mostly opened" for the index and middle finger is 3 quarters their distance from the hand
        float rightIndexMostlyClosed = (rightIndexMaxDistance / 4) * 3;
        float rightMiddleMostlyClosed = (rightMiddleMaxDistance / 4) * 3;

        //The distance to check "if closed" for the ring and pinky finger is half their distance from the hand
        float rightRingHalfClosed = rightRingMaxDistance / 2;
        float rightPinkyHalfClosed = rightPinkyMaxDistance / 2;

        hasCalibrated = true;

        Debug.Log("Finger positions successfully calibrated");

        detector.AssignCalibratedFingerPositions(leftIndexMostlyClosed, leftMiddleMostlyClosed, leftRingHalfClosed, leftPinkyHalfClosed,
                                                 rightIndexMostlyClosed, rightMiddleMostlyClosed, rightRingHalfClosed, rightPinkyHalfClosed);



    }

    private void CalibrateCountdown()
    {
        if (startCalibrateCountdown)
        {
            curCalibratorCountdown -= Time.deltaTime;
            calibrationCountdownText.text = curCalibratorCountdown.ToString("F2");

            if (curCalibratorCountdown <= 0)
            {
                Debug.Log("Calibrating now!");
                startCalibrateCountdown = false;
                CalibrateHands();
            }
        }
        else
        {
            curCalibratorCountdown = calibratorCountdownTime;
            calibrationCountdownText.text = "Done";
        }
    }

    public override void Interact()
    {
        Debug.Log("Calibration countdown started");
        startCalibrateCountdown = true;
    }
}
