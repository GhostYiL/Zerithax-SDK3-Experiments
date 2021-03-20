
using UdonSharp;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax
{
    [RequireComponent(typeof(Camera))]
    public class WaterRippleCamera : UdonSharpBehaviour
    {
        [SerializeField] private MeshRenderer waterPlane;
        [SerializeField] private Camera cam;

        private void Start()
        {
            //cam = GetComponent<Camera>();
        }

        private void Update()
        {
            waterPlane.sharedMaterial.SetVector("_CamPosition", transform.position);
            waterPlane.sharedMaterial.SetFloat("_OrthographicCamSize", cam.orthographicSize);
        }
    }
}