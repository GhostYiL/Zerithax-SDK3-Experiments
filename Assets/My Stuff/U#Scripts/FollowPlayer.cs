
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax
{
    public class FollowPlayer : UdonSharpBehaviour
    {
        private VRCPlayerApi playerApi;

        [Header("Main Settings")]
        [SerializeField] private bool followPosition = false;
        [SerializeField] private bool followRotation = false;
        [SerializeField] private HumanBodyBones boneToFollow;

        [Header("Position Offsets")]
        [SerializeField] private float offsetXPos = 0;
        [SerializeField] private float offsetYPos = 0;
        [SerializeField] private float offsetZPos = 0;

        [Header("Rotation Offsets")]
        [SerializeField] private float offsetXRot = 0;
        [SerializeField] private float offsetYRot = 0;
        [SerializeField] private float offsetZRot = 0;

        [Header("Hacky networking")]
        [SerializeField] private bool isForOwner;

        void Start()
        {
            playerApi = Networking.LocalPlayer;
        }

        private void Update()
        {
            if (isForOwner)
            {
                if (playerApi.isMaster)
                {
                    Networking.SetOwner(playerApi, gameObject);

                    if (followPosition)
                    {
                        Vector3 bonePosition = playerApi.GetBonePosition(boneToFollow);
                        Vector3 offsetPosition = new Vector3(bonePosition.x + offsetXPos, bonePosition.y + offsetYPos, bonePosition.z + offsetZPos);
                        transform.position = offsetPosition;
                    }

                    if (followRotation)
                    {
                        Vector3 boneRotation = playerApi.GetBonePosition(boneToFollow);
                        Vector3 offsetRotation = new Vector3(boneRotation.x + offsetXRot, boneRotation.y + offsetYRot, boneRotation.z + offsetZRot);
                        transform.rotation = Quaternion.Euler(offsetRotation);
                    }
                }
            }
            else
            {
                if (!playerApi.isMaster)
                {
                    Networking.SetOwner(playerApi, gameObject);

                    if (followPosition)
                    {
                        Vector3 bonePosition = playerApi.GetBonePosition(boneToFollow);
                        Vector3 offsetPosition = new Vector3(bonePosition.x + offsetXPos, bonePosition.y + offsetYPos, bonePosition.z + offsetZPos);
                        transform.position = offsetPosition;
                    }

                    if (followRotation)
                    {
                        Vector3 boneRotation = playerApi.GetBonePosition(boneToFollow);
                        Vector3 offsetRotation = new Vector3(boneRotation.x + offsetXRot, boneRotation.y + offsetYRot, boneRotation.z + offsetZRot);
                        transform.rotation = Quaternion.Euler(offsetRotation);
                    }
                }
            }

            /*
            if (followPosition)
            {
                Vector3 bonePosition = playerApi.GetBonePosition(boneToFollow);
                Vector3 offsetPosition = new Vector3(bonePosition.x + offsetXPos, bonePosition.y + offsetYPos, bonePosition.z + offsetZPos);
                transform.position = offsetPosition;
            }

            if (followRotation)
            {
                Vector3 boneRotation = playerApi.GetBonePosition(boneToFollow);
                Vector3 offsetRotation = new Vector3(boneRotation.x + offsetXRot, boneRotation.y + offsetYRot, boneRotation.z + offsetZRot);
                transform.rotation = Quaternion.Euler(offsetRotation);
            }
            */
        }
    }
}