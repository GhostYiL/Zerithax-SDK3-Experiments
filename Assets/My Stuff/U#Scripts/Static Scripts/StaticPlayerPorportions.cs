
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace zerithax.StaticClasses
{
    public class StaticPlayerPorportions : UdonSharpBehaviour
    {
        [SerializeField] private float playerHeight;
        private float maxDistanceFromHeadToNeck;

        [SerializeField] private float defaultPlayerHeight;

        [SerializeField] private GameObject[] heightReliantScripts = new GameObject[16];

        [Header("Debugs")]
        [SerializeField] private bool debugHeight;
        [SerializeField] private TextMeshPro heightText;
        [SerializeField] private bool overrideHeight;
        [SerializeField] private float forcedHeight;

        private VRCPlayerApi localPlayer;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;
            playerHeight = defaultPlayerHeight;
        }

        private void Update()
        {
            if (!overrideHeight)
            {
                if (PlayerSkeletonHasChanged())
                {
                    playerHeight = GetPlayerHeight();

                    foreach (GameObject obj in heightReliantScripts)
                    {
                        //obj.component.sendcustomevent?
                    }

                    if (debugHeight) heightText.text = "Player Height: " + playerHeight.ToString("F2") + "m";
                }
            }
            else
            {
                playerHeight = forcedHeight;
            }
        }

        public float MultiplyFloatByPlayerHeightProportions(float input)
        {
            float scaleFactor = playerHeight / defaultPlayerHeight;
            return input * scaleFactor;
        }

        public Vector3 MultiplyVector3ByPlayerHeightProportions(Vector3 input)
        {
            if (playerHeight != 0)
            {
                float scaleFactor = playerHeight / defaultPlayerHeight;
                return input * scaleFactor;
            }
            else return input;
        }

        public void SubscribeToHeightChangedEvent(GameObject objToAdd)
        {
            for (int i = 0; i < heightReliantScripts.Length; i++)
            {
                if (heightReliantScripts[i] == null)
                {
                    heightReliantScripts[i] = objToAdd;
                    break;
                }
            }
        }

        public float StaticPlayerHeight()
        {
            return playerHeight;
        }

        private bool PlayerSkeletonHasChanged()
        {
            Vector3 headPos = localPlayer.GetBonePosition(HumanBodyBones.Head);
            Vector3 neckPos = localPlayer.GetBonePosition(HumanBodyBones.Neck);

            float curDistanceFromHeadToNeck = Vector3.Distance(headPos, neckPos);

            if (Mathf.Abs(curDistanceFromHeadToNeck - maxDistanceFromHeadToNeck) > 0.00001)
            {
                maxDistanceFromHeadToNeck = curDistanceFromHeadToNeck;
                return true;
            }
            return false;
        }

        private float GetPlayerHeight()
        {
            Vector3 playerHeadPos = localPlayer.GetBonePosition(HumanBodyBones.Head);
            Vector3 playerFeetPos = (localPlayer.GetBonePosition(HumanBodyBones.LeftFoot) + localPlayer.GetBonePosition(HumanBodyBones.RightFoot)) / 2;

            float playerHeight = Vector3.Distance(playerHeadPos, playerFeetPos);

            return playerHeight;
        }
    }
}