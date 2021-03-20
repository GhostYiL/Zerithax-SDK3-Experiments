
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.twoHanding
{
    //This is the object that will be moved by two "Two Handed Grip" game objects
    public class TwoHandedObject : UdonSharpBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float verticalOffset;
        [SerializeField] private GameObject grip1;
        [SerializeField] private GameObject grip2;

        //[SerializeField] private VRC_Pickup[] gripsList = new VRC_Pickup[5];

        [Header("Allow One Handed")]
        [SerializeField] private bool oneHanded;

        

        private VRC_Pickup grip1Pickup;
        private VRC_Pickup grip2Pickup;

        void Start()
        {
            grip1Pickup = (VRC_Pickup)grip1.GetComponent(typeof(VRC_Pickup));
            grip2Pickup = (VRC_Pickup)grip2.GetComponent(typeof(VRC_Pickup)); 
        }

        private void Update()
        {
            if (oneHanded)
            {
                TrackIfSingleGripHeld(grip1Pickup, grip2Pickup);
            }

            TrackIfTwoGripsHeld(grip1Pickup, grip2Pickup);
        }

        private void TrackIfSingleGripHeld(VRC_Pickup gripOption1, VRC_Pickup gripOption2)
        {
            VRC_Pickup currentGripHeld;

            if (gripOption1.IsHeld ^ gripOption2.IsHeld)
            {
                currentGripHeld = gripOption1.IsHeld ? gripOption1 : gripOption2;

                Networking.SetOwner(currentGripHeld.currentPlayer, gameObject);
            }
            else currentGripHeld = null;

            if (currentGripHeld != null)
            {
                Vector3 gripPos = currentGripHeld.transform.position;
                Quaternion gripRot = currentGripHeld.transform.rotation;

                transform.position = gripPos - currentGripHeld.transform.forward * verticalOffset;
                transform.rotation = gripRot;
            }
        }

        private void TrackIfTwoGripsHeld(VRC_Pickup gripOption1, VRC_Pickup gripOption2)
        {
            if (gripOption1.IsHeld && gripOption2.IsHeld)
            {
                if (grip1Pickup.currentPlayer == grip2Pickup.currentPlayer)
                {
                    Networking.SetOwner(grip1Pickup.currentPlayer, gameObject);
                }

                Vector3 grip1Pos = grip1.transform.position;
                Vector3 grip2Pos = grip2.transform.position;

                Quaternion grip1Rot = grip1.transform.rotation;
                Quaternion grip2Rot = grip2.transform.rotation;

                transform.position = ((grip1Pos - grip1.transform.forward * verticalOffset) + (grip2Pos - grip2.transform.forward * verticalOffset)) / 2;
                transform.rotation = Quaternion.Slerp(grip1Rot, grip2Rot, 0.5f);

                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Slerp(grip1Rot, grip2Rot, 0.5f), rotationSpeed * Time.deltaTime);

            }
        }

    }
}