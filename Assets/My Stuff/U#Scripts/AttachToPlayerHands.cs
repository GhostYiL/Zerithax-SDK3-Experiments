
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.attachToHands
{
    public class AttachToPlayerHands : UdonSharpBehaviour
    {
        //This is a bone you want to get the tracking data from
        [SerializeField] private VRCPlayerApi.TrackingDataType trackingTarget;
        [SerializeField] private bool setPosition;
        [SerializeField] private bool setRotation;

        [Header("Offsets")]
        [SerializeField] private bool offsetRotation;
        [SerializeField] private Vector3 rotationOffset;

        [SerializeField] private bool offsetPosition;
        [SerializeField] private Vector3 positionOffset;

        private VRCPlayerApi playerAPI;
        private bool inEditor;

        void Start()
        {
            playerAPI = Networking.LocalPlayer;
            inEditor = playerAPI == null;
        }

        private void LateUpdate()
        {
            if (inEditor) return;

            VRCPlayerApi.TrackingData trackingData = playerAPI.GetTrackingData(trackingTarget);

            //POS
            Vector3 trackingDataPos = trackingData.position;
            if (offsetPosition)
            {
                trackingDataPos += positionOffset;
            }
            if (setPosition) transform.position = trackingDataPos;



            //ROT
            Quaternion trackingDataRot = trackingData.rotation;
            if (offsetRotation)
            {
                Quaternion rotMultiplier = Quaternion.Euler(rotationOffset);
                trackingDataRot *= rotMultiplier;
            }
            if (setRotation) transform.rotation = trackingDataRot;
        }
    }
}