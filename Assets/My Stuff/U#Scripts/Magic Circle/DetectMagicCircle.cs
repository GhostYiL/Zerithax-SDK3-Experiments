
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using zerithax.StaticClasses;
using zerithax.MagicCircle;

namespace zerithax.MagicCircle
{
    public class DetectMagicCircle : UdonSharpBehaviour
    {
        [Header("Proportions")]
        [SerializeField] private StaticPlayerPorportions playerProportionScript;

        [Header("Handler")]
        [SerializeField] private MagicCircleHandler circleHandler;

        [Header("Current Magic Attributes")]
        [SerializeField] private int modifierElement;
        [SerializeField] private int modifierSpeed;
        [SerializeField] private int modifierType;
        [SerializeField] private int modifierSize;

        [Header("Magic Attack Object")]
        [SerializeField] private GameObject magicAttackObject;
        [SerializeField] private bool fireAttackDebug = false;
        private bool hasFired;

        [Header("Magic Modifiers")]
        //TODO NETWORKING: Make magicCircle get set to the circle taken from the pool (spawned by the SpawnMagicCircle script)
        [SerializeField] private GameObject magicCircle;
        [SerializeField] private Material magicModifierUnselectedMat;
        [SerializeField] private Material magicModifierSelectedMat;

        [Space(10)]
        [SerializeField] private MagicModifier[] elementModifiers;
        [SerializeField] private MagicModifier[] speedModifiers;
        [SerializeField] private MagicModifier[] typeModifiers;
        [SerializeField] private MagicModifier[] sizeModifiers;

        [Space(10)]
        [SerializeField] private float magicCircleCenterRange;
        [SerializeField] private float magicModifierRange;

        private float magicCircleCenterRangeProportion;
        private float magicModifierRangeProportion;
        [Space(10)]
        [SerializeField] private bool neutral;
        [SerializeField] private bool hasReturnedToNeutral;

        [Header("Debug")]
        [SerializeField] private bool debug = false;
        [SerializeField] private MeshRenderer debugTrigger;
        [SerializeField] private TextMeshPro debugElement;
        [SerializeField] private TextMeshPro debugSpeed;
        [SerializeField] private TextMeshPro debugType;
        [SerializeField] private TextMeshPro debugSize;


        private void Update()
        {
            if (magicCircle != null)
            {
                CheckIfNearMagicCircleCenter();

                //If hovering element mods
                foreach (MagicModifier currentMagicModifier in elementModifiers)
                {
                    modifierElement = CheckIfNearModifiers(currentMagicModifier, modifierElement);
                }

                //If hovering speed mods
                foreach (MagicModifier currentMagicModifier in speedModifiers)
                {
                    modifierSpeed = CheckIfNearModifiers(currentMagicModifier, modifierSpeed);
                }

                //If hovering type mods
                foreach (MagicModifier currentMagicModifier in typeModifiers)
                {
                    modifierType = CheckIfNearModifiers(currentMagicModifier, modifierType);
                }

                //If hovering size mods
                foreach (MagicModifier currentMagicModifier in sizeModifiers)
                {
                    modifierSize = CheckIfNearModifiers(currentMagicModifier, modifierSize);
                }
            }

            //If I am in Neutral position and I clench fist and I've already selected elements, tell magic attack to SetMagicAttackSettingsThenFire(); using the given attributes then unselect them (or kill magic circle?)
            //TODO: Make sure this still works!

            DebugHandCube();

            if (neutral)
            {
                if (Input.GetAxisRaw("Oculus_CrossPlatform_SecondaryHandTrigger") == 1 && !hasFired)
                {
                    hasFired = true;

                    if (modifierElement == 0) modifierElement = 2;
                    if (modifierSpeed == 0) modifierSpeed = 2;
                    if (modifierType == 0) modifierType = 3;
                    if (modifierSize == 0) modifierSize = 1;

                    if (debug) Debug.Log("Attempting to set magic attack settings...");
                    magicAttackObject.GetComponent<MagicAttack>().SetMagicAttackSettingsThenFire(modifierElement, modifierSpeed, modifierType, modifierSize, transform);
                }
            }

        }

        private void CheckIfNearMagicCircleCenter()
        {
            //magicCircleCenterRangeProportion = playerProportionScript.MultiplyFloatByPlayerHeightProportions(magicCircleCenterRange);
            //Debug.Log("Center proportion set to : " + magicCircleCenterRangeProportion);

            Vector3 localToCircle3 = magicCircle.transform.InverseTransformPoint(transform.position);

            //float distanceSqr = (magicCircle.transform.position - transform.position).sqrMagnitude;

            //TODO: this is breaking? is something wrong somewhere else? modifiers immediately detected

            //original (proportionate range)
            //if (localToCircle3.sqrMagnitude <= magicCircleCenterRangeProportion * magicCircleCenterRangeProportion)

            //distance square (broken?)
            //if (distanceSqr <= magicCircleCenterRangeProportion * magicCircleCenterRangeProportion)

            //vector3.distance (broken?)
            //if (Vector3.Distance(magicCircle.transform.position, transform.position) <= magicCircleCenterRangeProportion)

            //original range
            if (localToCircle3.sqrMagnitude <= magicCircleCenterRange * magicCircleCenterRange)
            {
                hasReturnedToNeutral = true;
                neutral = true;
            }
            else neutral = false;
        }

        private int CheckIfNearModifiers(MagicModifier modifierToCheckAgainst, int currentValue)
        {
            //magicModifierRangeProportion = playerProportionScript.MultiplyFloatByPlayerHeightProportions(magicModifierRange);

            Vector3 localToModifier3 = modifierToCheckAgainst.transform.InverseTransformPoint(transform.position);

            //float distanceSqr = (modifierToCheckAgainst.transform.position - transform.position).sqrMagnitude;

            //original (proportionate range)
            //if (localToModifier3.sqrMagnitude <= magicModifierRangeProportion * magicModifierRangeProportion && hasReturnedToNeutral)

            //distance square (broken?)
            //if (distanceSqr <= magicCircleCenterRangeProportion * magicCircleCenterRangeProportion && hasReturnedToNeutral)

            //vector3.distance (broken?)
            //if (Vector3.Distance(modifierToCheckAgainst.transform.position, transform.position) <= magicModifierRangeProportion && hasReturnedToNeutral)

            //original range
            if (localToModifier3.sqrMagnitude <= magicModifierRange * magicModifierRange && hasReturnedToNeutral)
            {
                hasReturnedToNeutral = false;

                //Temp?: Search through a list of all the siblings of this mod's mod group, then set current material to bright glow and all siblings to dull
                foreach (MagicModifier mod in modifierToCheckAgainst.GetSiblingModifiers())
                {
                    if (mod.gameObject.name == modifierToCheckAgainst.gameObject.name) mod.GetComponent<Renderer>().material = magicModifierSelectedMat;
                    else mod.GetComponent<Renderer>().material = magicModifierUnselectedMat;

                }

                return modifierToCheckAgainst.GetModifier();
            }
            return currentValue;
        }

        private void DebugHandCube()
        {
            if (debug)
            {
                debugElement.text = modifierElement.ToString();
                debugSpeed.text = modifierSpeed.ToString();
                debugType.text = modifierType.ToString();
                debugSize.text = modifierSize.ToString();

                if (Input.GetAxisRaw("Oculus_CrossPlatform_SecondaryHandTrigger") == 1)
                {
                    debugTrigger.material.color = Color.red;
                    //Debug.Log("Set debug cube color to red for trigger!");
                }
                else debugTrigger.material.color = Color.blue;
            }

            if (fireAttackDebug && !hasFired)
            {
                hasFired = true;
                fireAttackDebug = false;

                Debug.Log("Attempting to set magic attack settings...");
                magicAttackObject.GetComponent<MagicAttack>().SetMagicAttackSettingsThenFire(modifierElement, modifierSpeed, modifierType, modifierSize, transform);
            }
        }

        public void SetMagicCircle(GameObject circle)
        {
            magicCircle = circle;
        }

        public void CleanupMagicCircleDetector()
        {
            if (debug) Debug.Log("Cleaning up Magic Circle Detector...");

            hasFired = false;

            modifierElement = 0;
            modifierSpeed = 0;
            modifierType = 0;
            modifierSize = 0;

            //Tell magic circle it can clean itself up
            circleHandler.CleanupMagicCircle();
            magicCircle = null;

            //Resetting element mod mats
            foreach (MagicModifier currentMagicModifier in elementModifiers)
            {
                currentMagicModifier.GetComponent<Renderer>().material = magicModifierUnselectedMat;
            }

            //Resetting speed mod mats
            foreach (MagicModifier currentMagicModifier in speedModifiers)
            {
                currentMagicModifier.GetComponent<Renderer>().material = magicModifierUnselectedMat;
            }

            //Resetting type mod mats
            foreach (MagicModifier currentMagicModifier in typeModifiers)
            {
                currentMagicModifier.GetComponent<Renderer>().material = magicModifierUnselectedMat;
            }

            //Resetting size mod mats
            foreach (MagicModifier currentMagicModifier in sizeModifiers)
            {
                currentMagicModifier.GetComponent<Renderer>().material = magicModifierUnselectedMat;
            }
        }
    }
}