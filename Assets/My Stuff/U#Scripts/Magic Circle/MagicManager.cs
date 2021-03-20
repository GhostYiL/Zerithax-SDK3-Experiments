
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using zerithax.MagicCircle;

namespace zerithax.MagicCircle
{
    public class MagicManager : UdonSharpBehaviour
    {
        //This script will be in charge of dealing with pools and setting which player owns which magic circle when they join

        [SerializeField] private MagicCircleHandler[] magicCircles;
        [/*UdonSynced, */SerializeField] private string[] ownedMagicCircles;

        private void Start()
        {
            for (int i = 0; i >= ownedMagicCircles.Length; i++)
            {
                if (ownedMagicCircles[i] == Networking.LocalPlayer.displayName) magicCircles[i].SetOwnedByPlayer();
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsMaster)
            {
                if (player == Networking.LocalPlayer)
                {
                    Networking.SetOwner(player, gameObject);
                    ownedMagicCircles = new string[magicCircles.Length];
                }

                for (int i = 0; i >= ownedMagicCircles.Length; i++)
                {
                    if (ownedMagicCircles[i] == null)
                    {
                        ownedMagicCircles[i] = player.displayName;
                        break;
                    }
                }
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (Networking.IsMaster)
            {
                for (int i = 0; i >= ownedMagicCircles.Length; i++)
                {
                    if (ownedMagicCircles[i] == player.displayName)
                    {
                        ownedMagicCircles[i] = null;
                        break;
                    }
                }
            }
        }
    }
}