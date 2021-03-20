
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TwoHandedGrip : UdonSharpBehaviour
{
    [SerializeField] private Transform originalParent;
    [SerializeField] private Transform originalPosition;

    private VRC_Pickup gripPickup;
    private bool isCurrentlyHeld = false;

    void Start()
    {
        gripPickup = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
    }

    private void Update()
    {
        UnParentWhileGripped();

        DropIfPulledTooFar();
    }

    private void DropIfPulledTooFar()
    {
        //if (Vector3.Distance(transform.position, originalPosition.TransformPoint(originalPosition.position)) >= 0.3) gripPickup.Drop();
    }

    private void UnParentWhileGripped()
    {
        if (!gripPickup.IsHeld && isCurrentlyHeld)
        {
            isCurrentlyHeld = false;
            transform.parent = originalPosition.transform.parent;
            transform.position = originalPosition.position;
            transform.rotation = originalPosition.rotation;
        }

        if (gripPickup.IsHeld)
        {
            isCurrentlyHeld = true;
            transform.parent = originalParent;
        }

        /*
        if (gripPickup.IsHeld)
        {
            isCurrentlyHeld = true;
        }

        if (isCurrentlyHeld && !gripPickup.IsHeld)
        {
            isCurrentlyHeld = false;
            transform.parent = originalPosition.transform.parent;
            transform.position = originalPosition.position;
            transform.rotation = originalPosition.rotation;
            transform.parent = originalParent;
        }
        */
    }
}
