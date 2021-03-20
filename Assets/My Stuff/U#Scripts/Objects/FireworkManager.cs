
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace zerithax
{
    public class FireworkManager : UdonSharpBehaviour
    {
        /// <summary>
        /// This is a management script that will check the local player's hand positions and spawn a firework if they are within the given boundaries for a certain amount of time
        /// </summary>

        [SerializeField] private Firework fireworkComponent;

        [SerializeField] private float dotToMeasureAgainst = 0.5f;

        [SerializeField] private float rightFireworkCountdown;
        private float rightFireworkCurrentCountdown = 0;

        [SerializeField] private float leftFireworkCountdown;
        private float leftFireworkCurrentCountdown = 0;

        [Header("Hand Position Debug Cubes")]
        [SerializeField] private bool debug = false;
        [SerializeField] private GameObject debugLiftCube;
        [SerializeField] private GameObject debugRotateCube;
        [SerializeField] private GameObject debugTimeCube;
        [SerializeField] private GameObject debugDotDisplayLeft;
        [SerializeField] private GameObject debugDotDisplayRight;

        private VRCPlayerApi.TrackingDataType playerHead = VRCPlayerApi.TrackingDataType.Head;
        private Vector3 playerHeadPosition;

        private VRCPlayerApi.TrackingDataType playerRightHand = VRCPlayerApi.TrackingDataType.RightHand;
        private VRCPlayerApi.TrackingDataType playerLeftHand = VRCPlayerApi.TrackingDataType.LeftHand;

        private HumanBodyBones rightHand = HumanBodyBones.RightHand;
        private HumanBodyBones leftHand = HumanBodyBones.LeftHand;



        private bool[] rightHandHasFiredWrapper = new bool[1] { false };
        private bool[] leftHandHasFiredWrapper = new bool[1] { false };

        private VRCPlayerApi playerApi;

        void Start()
        {
            playerApi = Networking.LocalPlayer;

            if (debug)
            {
                debugLiftCube.GetComponent<Renderer>().material.color = Color.blue;
                debugRotateCube.GetComponent<Renderer>().material.color = Color.blue;
                debugTimeCube.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
            
        void Update()
        {
            if (playerApi != null)
            {
                playerHeadPosition = playerApi.GetTrackingData(playerHead).position;
            }
            
            //If right hand is correctly positioned, add time in seconds to the countdown
            rightFireworkCurrentCountdown += SetRightCountdownIfHandAligned(playerHeadPosition, rightHandHasFiredWrapper);

            //If I hit the time, fire the firework!
            if (rightFireworkCurrentCountdown >= rightFireworkCountdown)
            {
                //SpawnFirework(rightHand, rightHandHasFiredWrapper, fireworkComponent);
                SpawnFirework(playerRightHand, rightHandHasFiredWrapper, fireworkComponent);
                rightFireworkCurrentCountdown = 0;
            }

            
            //If left hand is correctly positioned, add time in seconds to the countdown
            leftFireworkCurrentCountdown += SetLeftCountdownIfHandAligned(playerHeadPosition, leftHandHasFiredWrapper);

            //If I hit the time, fire the firework!
            if (leftFireworkCurrentCountdown >= leftFireworkCountdown)
            {
                //SpawnFirework(leftHand, leftHandHasFiredWrapper, fireworkComponent);
                SpawnFirework(playerLeftHand, leftHandHasFiredWrapper, fireworkComponent);
                leftFireworkCurrentCountdown = 0;
            }
            
        }

        private void SpawnFirework(VRCPlayerApi.TrackingDataType handToFireFrom, bool[] handToNotifyHasFired, Firework firework)
        {
            handToNotifyHasFired[0] = true;
            Networking.SetOwner(playerApi, firework.gameObject);

            Vector3 fireDirection;

            if (handToFireFrom == VRCPlayerApi.TrackingDataType.LeftHand)
            {
                fireDirection = playerApi.GetTrackingData(handToFireFrom).rotation * Vector3.up;
            }
            else
            {
                fireDirection = playerApi.GetTrackingData(handToFireFrom).rotation * Vector3.down;
            }

            if (debug) debugTimeCube.GetComponent<Renderer>().material.color = Color.green;

            //Tell the firework to spawn at your hand, pointing in the direction of your palm (roughly)
            firework.SetupFlying(playerApi.GetTrackingData(handToFireFrom).position, fireDirection);
        }

        private float SetLeftCountdownIfHandAligned(Vector3 localPlayerHeadPos, bool[] handHasFired)
        {
            HumanBodyBones handToCheck = HumanBodyBones.LeftHand;

            Vector3 handPosition = playerApi.GetBonePosition(handToCheck);
            Quaternion handRotation = playerApi.GetBoneRotation(handToCheck);

            float dotProduct = Vector3.Dot(handRotation * Vector3.right, Vector3.up);
            if (debug) debugDotDisplayLeft.GetComponent<TextMeshPro>().text = ("Dot: " + dotProduct.ToString("F3"));

            //If a hand is above your head...
            if (handPosition.y >= localPlayerHeadPos.y)
            {
                if (debug) debugLiftCube.GetComponent<Renderer>().material.color = Color.red;                    

                //And the hand is mostly angled toward the sky
                if (dotProduct >= dotToMeasureAgainst)
                {
                    if (debug) debugRotateCube.GetComponent<Renderer>().material.color = Color.red;

                    //If I haven't already fired once, start the countdown (technically up)
                    if (!handHasFired[0])
                    {
                        if (debug) debugTimeCube.GetComponent<Renderer>().material.color = Color.red;
                        return Time.deltaTime;
                    }
                }
                //My hand is incorrectly rotated now
                else
                {
                    if (debug) debugRotateCube.GetComponent<Renderer>().material.color = Color.blue;
                    return 0;
                }

                return 0;
            }
            //I've put my hand down, so reset the firing to allow me to shoot again!
            else
            {
                if (debug)
                {
                    debugLiftCube.GetComponent<Renderer>().material.color = Color.blue;
                    debugRotateCube.GetComponent<Renderer>().material.color = Color.blue;
                    debugTimeCube.GetComponent<Renderer>().material.color = Color.blue;
                }

                handHasFired[0] = false;
                return 0;
            }
        }

        private float SetRightCountdownIfHandAligned(Vector3 localPlayerHeadPos, bool[] handHasFired)
        {
            HumanBodyBones handToCheck = HumanBodyBones.RightHand;

            Vector3 handPosition = playerApi.GetBonePosition(handToCheck);
            Quaternion handRotation = playerApi.GetBoneRotation(handToCheck);

            float dotProduct = Vector3.Dot(handRotation * Vector3.right, Vector3.down);
            if (debug) debugDotDisplayRight.GetComponent<TextMeshPro>().text = ("Dot: " + dotProduct.ToString("F3"));

            //If a hand is above your head...
            if (handPosition.y >= localPlayerHeadPos.y)
            {
                if (debug) debugLiftCube.GetComponent<Renderer>().material.color = Color.red;

                //And the hand is mostly angled toward the sky
                if (dotProduct >= dotToMeasureAgainst)
                {
                    if (debug) debugRotateCube.GetComponent<Renderer>().material.color = Color.red;

                    //If I haven't already fired once, start the countdown (technically up)
                    if (!handHasFired[0])
                    {
                        if (debug) debugTimeCube.GetComponent<Renderer>().material.color = Color.red;
                        return Time.deltaTime;
                    }
                }
                //My hand is incorrectly rotated now
                else
                {
                    if (debug) debugRotateCube.GetComponent<Renderer>().material.color = Color.blue;
                    return 0;
                }

                return 0;
            }
            //I've put my hand down, so reset the firing to allow me to shoot again!
            else
            {
                if (debug)
                {
                    debugLiftCube.GetComponent<Renderer>().material.color = Color.blue;
                    debugRotateCube.GetComponent<Renderer>().material.color = Color.blue;
                    debugTimeCube.GetComponent<Renderer>().material.color = Color.blue;
                }

                handHasFired[0] = false;
                return 0;
            }
        }

    }
}