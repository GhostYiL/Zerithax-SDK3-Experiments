
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax
{
    [ExecuteInEditMode]
    public class TimeOfDay : UdonSharpBehaviour
    {
        [Header("Synced Vars")]
        //UDON SYNCED variables are synced
        [UdonSynced, SerializeField] private float syncedSunPositionX;
        [UdonSynced, SerializeField] private float syncedMoonPhase;

        [Header("Cycle Options")]
        [SerializeField] private float timeCycleSpeed;
        [SerializeField] private float moonCycleAmount = 0.1f;

        private Material skyMaterial;
        private bool canIncreaseDay = true;

        void Start()
        {
            skyMaterial = RenderSettings.skybox;

            if (Networking.LocalPlayer.isMaster)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                syncedSunPositionX = transform.rotation.eulerAngles.x;
                syncedMoonPhase = skyMaterial.GetFloat("_MoonPhase");
            }
        }

        private void Update()
        {
            //Check if master, if true set the synced variables
            if (Networking.IsMaster)
            {
                IncrementSunRotation(timeCycleSpeed);
                IncrementMoonPhase(syncedSunPositionX, moonCycleAmount);
            }

            //Run the actual object-modifiers using the recently-updated synced variables if the master, otherwise use variables assuming the master has updated them. This will run for the master after they sync, and for the players (who won't sync since they're not master, but use the variables that were synced by the master)
            SetNewSunRotation(syncedSunPositionX);
            SetNewMoonPhase(syncedSunPositionX, syncedMoonPhase);
        }

        private void IncrementSunRotation(float incrementAmount)
        {
            //If the sun has rotated out of bounds, flip it back
            if (syncedSunPositionX >= 180) syncedSunPositionX -= 360;
            else if (syncedSunPositionX <= -180) syncedSunPositionX += 360;

            //Increment sun's rotation
            syncedSunPositionX += Time.deltaTime * incrementAmount;
        }

        private void IncrementMoonPhase(float syncedSunRotation, float incrementAmount)
        {
            //If sun has set and we haven't already done this once!
            if (syncedSunRotation >= -0.05 && syncedSunRotation <= 0.05 && canIncreaseDay)
            {
                //Make sure this doesn't repeat
                canIncreaseDay = false;

                //Increment the moon's phase
                syncedMoonPhase = skyMaterial.GetFloat("_MoonPhase") + incrementAmount;

                //If moon is too far past its point, reset it
                if (syncedMoonPhase >= 1) syncedMoonPhase -= 1;
            }
        }

        private void SetNewSunRotation(float syncedSunRotation)
        {
            //Set sun's rotation to the synced variable, simple enough
            transform.rotation = Quaternion.Euler(new Vector3(syncedSunRotation, transform.rotation.y, transform.rotation.z));
        }

        private void SetNewMoonPhase(float syncedSunRotation, float syncedMoonPhase)
        {
            //Set material value to the new phase
            if (skyMaterial.GetFloat("_MoonPhase") != syncedMoonPhase) skyMaterial.SetFloat("_MoonPhase", syncedMoonPhase);

            //For network master only, since they set this to false when it came time to increment, they should set to true once they're past sunset
            if ((syncedSunRotation >= 0.06 || syncedSunRotation <= -0.06) && !canIncreaseDay) canIncreaseDay = true;
        }
    }
}