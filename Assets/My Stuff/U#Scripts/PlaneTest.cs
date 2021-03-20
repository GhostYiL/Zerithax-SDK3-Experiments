
using UdonSharp;
using UnityEngine;
using System;
using UnityEditor;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.planeTests
{
    public class PlaneTest : UdonSharpBehaviour
    {
        [SerializeField]
        private bool rotateOnX = false;

        [SerializeField]
        private bool rotateOnY = false;

        [SerializeField]
        private bool rotateOnZ = false;

        [SerializeField]
        private float rotateAmount;

        [SerializeField]
        private GameObject plane;

        private void Update()
        {
            RotatePlane();
        }

        private void RotatePlane()
        {
            if (plane) plane.transform.Rotate(new Vector3(
                            (rotateOnX ? 1 : 0) * rotateAmount * Time.deltaTime,
                            (rotateOnY ? 1 : 0) * rotateAmount * Time.deltaTime,
                            (rotateOnZ ? 1 : 0) * rotateAmount * Time.deltaTime));
        }
    }
}