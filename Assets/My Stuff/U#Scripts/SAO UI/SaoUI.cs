
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using zerithax.StaticClasses;

namespace zerithax.HandSwipeUI
{
    public class SaoUI : UdonSharpBehaviour
    {
        [Header("Proportionate Scaler")]
        [SerializeField] private StaticPlayerPorportions playerProportionScript;
        private Vector3 baseScale;

        [Header("Window Component")]
        [SerializeField] private UIWindow windowComp;

        [Header("Script Vars")]
        [SerializeField] private Vector3 spawnOffsets;
        [SerializeField] private float maxDistanceFromPlayer;
        [SerializeField] private Transform deadPos;

        private Vector2 playerPosWhenSpawned;

        void Start()
        {
            baseScale = transform.localScale;
            transform.position = deadPos.position;
        }

        private void Update()
        {
            Vector3 playerPos = Networking.LocalPlayer.GetPosition();
            Vector2 playerTruePos = new Vector2(playerPos.x, playerPos.z);

            //Kill self if player walks too far from me
            //if (playerPosWhenSpawned != Vector3.zero && (Networking.LocalPlayer.GetPosition() - playerPosWhenSpawned).sqrMagnitude >= maxDistanceFromPlayer * maxDistanceFromPlayer)
            if (playerPosWhenSpawned != Vector2.zero && Vector2.Distance(playerTruePos, playerPosWhenSpawned) >= maxDistanceFromPlayer)
            {
                Debug.Log("Player walked too far from UI");
                KillUI();
                playerPosWhenSpawned = Vector3.zero;
            }
        }

        public void SpawnUI(Vector3 initialPlayerPos, Quaternion playerHeadRot, Vector3 handPosition)
        {
            transform.localScale = playerProportionScript.MultiplyVector3ByPlayerHeightProportions(baseScale);

            playerPosWhenSpawned = new Vector2(initialPlayerPos.x, initialPlayerPos.z);

            Vector3 currentRotation = new Vector3(transform.rotation.eulerAngles.x, playerHeadRot.eulerAngles.y, transform.rotation.eulerAngles.z);
            Quaternion newRotation = Quaternion.Euler(currentRotation);
            transform.rotation = newRotation;


            Vector3 newPos = handPosition;
            newPos.y += spawnOffsets.y;

            Vector3 forwardDirection = newRotation * Vector3.forward;
            newPos += forwardDirection * spawnOffsets.z;

            transform.position = newPos;

            windowComp.OpenWindow();

            Debug.Log("UI succesfully spawned!");

            //And finally, run the animation that makes it look like it's opening all cool like and stuff
            //At first this will probably be setting a flat sprite to enabled o/
        }

        public void KillUI()
        {
            transform.position = deadPos.position;

            windowComp.CloseWindow();

            Debug.Log("UI Killed");

            //Run a closing animation?
        }
    }
}