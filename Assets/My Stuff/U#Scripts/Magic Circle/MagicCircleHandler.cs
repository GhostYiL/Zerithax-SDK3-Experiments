
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using zerithax.StaticClasses;
using zerithax.MagicCircle;

namespace zerithax.MagicCircle
{
    public class MagicCircleHandler : UdonSharpBehaviour
    {
        [Header("Proportions")]
        [SerializeField] private StaticPlayerPorportions playerProportionScript;
        private Vector3 baseScale;

        [Header("Main Variables")]
        [SerializeField] private DetectMagicCircle magicCircleDetector;
        [SerializeField] private float maxDistanceFromPlayer;
        [SerializeField] private float countdownLimit;
        [SerializeField] private bool isOwnedByLocalPlayer;
        [SerializeField] private bool magicCircleSpawned;

        [Header("Debugs")]
        [SerializeField] private bool debugCircle;
        [SerializeField] private TextMeshPro debugCountdown;
        [SerializeField] private TextMeshPro debugDot;

        private float currentCountdown;

        private VRCPlayerApi playerAPI;
        private VRCPlayerApi.TrackingData playerHead;

        void Start()
        {
            baseScale = transform.localScale;
            playerAPI = Networking.LocalPlayer;
        }

        private void Update()
        {
            if (playerAPI != null)
            {
                playerHead = playerAPI.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            }

            //If this object is owned by the local player, let's spawn it in when the hand is lifted
            if (isOwnedByLocalPlayer && !magicCircleSpawned)
            {
                currentCountdown += SetCountdownIfHandAligned(currentCountdown);
                
                if (currentCountdown >= countdownLimit)
                {
                    if (debugCircle) Debug.Log("Circle should be spawning now!");
                    SpawnCircle();

                    currentCountdown = 0;
                }

                if (debugCircle) debugCountdown.text = currentCountdown.ToString(); 
            }

            if (magicCircleSpawned && Vector3.Distance(playerHead.position, transform.position) >= maxDistanceFromPlayer) CleanupMagicCircle();
        }

        public void SetOwnedByPlayer()
        {
            isOwnedByLocalPlayer = true;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        private float SetCountdownIfHandAligned(float currentTime)
        {
            float dot = Vector3.Dot(magicCircleDetector.transform.rotation * Vector3.forward, playerHead.rotation * Vector3.forward);
            
            if (debugCircle) debugDot.text = "Dot: " + dot.ToString("F3");

            if (dot >= 0.9) return Time.deltaTime;
            else return -currentCountdown;
        }

        private void SpawnCircle()
        {
            //REMOVE THIS WHEN NETWORKING COMES OUT!
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Debug.Log("Temporary cur-user set of " + gameObject.name + " since no networked arrays.");

            transform.localScale = playerProportionScript.MultiplyVector3ByPlayerHeightProportions(baseScale);

            //Temp: grab the magic circle and set it to current hand pos
            transform.position = magicCircleDetector.transform.position;

            Vector3 newRotation = new Vector3(transform.rotation.eulerAngles.x, magicCircleDetector.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newRotation);

            magicCircleDetector.SetMagicCircle(gameObject);

            magicCircleSpawned = true;


            //In the future this command will need to search for an available circle that has yet to be claimed by a networked player from the 'Magic Circles' array!
            //magicCircle = "find available magic circle from array"
            //Also remember to then set that index on the "available magic circles" array to owned!

            //Maybe return some sort of event after confirming a magic circle was found?
            //Or conversely, DetectMagicCircle can constantly be running "GetMagicCircle" with a null check so it knows when one was found! 

            //SHOULD WE do the same with the bullets array here too?


            //Above likely false... Simply SetOwnedByPlayer() should be enough to take care of all the necessary networking stuffs 
        }

        public void CleanupMagicCircle()
        {

            if (debugCircle) Debug.Log("Magic Circle: cleaning up self");

            transform.position = Vector3.zero;

            magicCircleSpawned = false;

            //Set the index on the "available magic circles" array to no longer owned

            //Should we do the same with the bullets array here too?
        }
    }
}