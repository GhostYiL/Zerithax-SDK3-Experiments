
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.attachToHandBones
{
    public class AttachToPlayerHandBones : UdonSharpBehaviour
    {
        [SerializeField] private HumanBodyBones trackedBone;

        VRCPlayerApi playerApi;
        bool isInEditor;

        private void Start()
        {
            playerApi = Networking.LocalPlayer;
            isInEditor = playerApi == null;
        }

        private void Update()
        {
            if (isInEditor) return;

            transform.SetPositionAndRotation(playerApi.GetBonePosition(trackedBone), playerApi.GetBoneRotation(trackedBone));
        }
    }
}