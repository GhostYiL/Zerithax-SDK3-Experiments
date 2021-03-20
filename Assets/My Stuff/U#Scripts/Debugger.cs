
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Debugger : UdonSharpBehaviour
{
    [SerializeField] private bool debugOn = false;

    private VRCPlayerApi playerApi;

    [SerializeField] private GameObject positionCubeX;
    [SerializeField] private GameObject positionCubeY;
    [SerializeField] private GameObject positionCubeZ;
    [SerializeField] private GameObject positionCubeXMinus;
    [SerializeField] private GameObject positionCubeYMinus;
    [SerializeField] private GameObject positionCubeZMinus;

    private VRCPlayerApi.TrackingDataType playerHead = VRCPlayerApi.TrackingDataType.Head;
    private Vector3 playerHeadPosition;

    private Vector3 debugOffPos = new Vector3(0, -1, 0);

    void Start()
    {
        playerApi = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (debugOn)
        {
            playerHeadPosition = playerApi.GetTrackingData(playerHead).position;

            positionCubeX.transform.position = playerHeadPosition + Vector3.right;
            positionCubeXMinus.transform.position = playerHeadPosition - Vector3.right;

            positionCubeY.transform.position = playerHeadPosition + Vector3.forward;
            positionCubeYMinus.transform.position = playerHeadPosition - Vector3.forward;

            positionCubeZ.transform.position = playerHeadPosition + new Vector3(0, 0.1f, 0);
            positionCubeZMinus.transform.position = playerHeadPosition;
        }
        else
        {
            positionCubeX.transform.position = debugOffPos;
            positionCubeXMinus.transform.position = debugOffPos;
            positionCubeY.transform.position = debugOffPos;
            positionCubeYMinus.transform.position = debugOffPos;
            positionCubeZ.transform.position = debugOffPos;
            positionCubeZMinus.transform.position = debugOffPos;
        }
    }

    public void ToggleDebug()
    {
        debugOn = !debugOn;
    }
}
