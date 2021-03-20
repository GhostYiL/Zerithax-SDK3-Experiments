
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using zerithax.MagicCircle;

namespace zerithax.MagicCircle
{
    public class MagicAttack : UdonSharpBehaviour
    {
        [SerializeField] Transform deathPosition;

        [Header("Magic Attack Stats")]
        [SerializeField] private float smallSizeValue;
        [SerializeField] private float mediumSizeValue;
        [SerializeField] private float largeSizeValue;

        [Space(10)]
        [SerializeField] private float slowSpeedValue;
        [SerializeField] private float mediumSpeedValue;
        [SerializeField] private float fastSpeedValue;

        [Space(10)]
        [SerializeField] private float blockRange;
        [SerializeField] private float blastRange;
        [SerializeField] private float boltRange;

        [Header("Mesh Filters")]
        [SerializeField] private Mesh sphereMesh;
        [SerializeField] private Mesh cubeMesh;
        [SerializeField] private Mesh capsuleMesh;

        [Space(10)]
        [SerializeField] private GameObject visibleObject;

        [Space(10)]
        [SerializeField] private bool readyToFire = false;


        private MeshFilter childFilter;
        private MeshRenderer childRenderer;

        private SphereCollider blockCollider;
        private BoxCollider blastCollider;
        private CapsuleCollider boltCollider;

        private int magicAttackElement;
        private int magicAttackSpeed;
        private int magicAttackType;
        private int magicAttackSize;

        private float projectileRange;
        private float projectileSpeed;

        private Transform spawnTransform;
        private Vector3 spawnPosition;
        private Vector3 spawnScale;

        //This will need to be set by getting the currently owned magic circle when synced arrays/object pooling happens
        [SerializeField] private DetectMagicCircle magicCircleDetector;


        void Start()
        {
            projectileSpeed = 0;

            childFilter = visibleObject.GetComponent<MeshFilter>();
            childRenderer = visibleObject.GetComponent<MeshRenderer>();

            blockCollider = GetComponent<SphereCollider>();
            blastCollider = GetComponent<BoxCollider>();
            boltCollider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            if (readyToFire)
            {
                FireMagicAttack();
            }
        }

        private void FireMagicAttack()
        {
            Debug.Log("I should be firing right now!");

            //REMOVE THIS WHEN NETWORKING COMES OUT!
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Debug.Log("Temporary cur-user set of " + gameObject.name + " since no networked arrays.");

            if (magicAttackType == 1)
            {
                transform.localScale += transform.localScale * projectileSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.localScale, spawnScale) >= projectileRange)
                {
                    CleanupMagicAttack();
                }
            }
            else if (magicAttackType == 2)
            {
                transform.localScale += transform.localScale * projectileSpeed / 7f * Time.deltaTime;
                transform.position += transform.forward * projectileSpeed * Time.deltaTime;

                if (Vector3.Distance(spawnPosition, transform.position) >= projectileRange)
                {
                    CleanupMagicAttack();
                }
            }
            else
            {
                transform.position += transform.forward * projectileSpeed * Time.deltaTime;

                if (Vector3.Distance(spawnPosition, transform.position) >= projectileRange)
                {
                    CleanupMagicAttack();
                }
            }

            if (transform.position.y <= 0) CleanupMagicAttack();

        }

        public void SetMagicAttackSettingsThenFire(int element, int speed, int type, int size, Transform cubeTransform)
        {
            spawnTransform = cubeTransform;

            magicAttackElement = element;
            magicAttackSpeed = speed;
            magicAttackType = type;
            magicAttackSize = size;

            Debug.Log("Magic attack modifiers have been successfully assigned");

            SetupProjectile();
        }

        private void SetupProjectile()
        {
            Debug.Log("Setting up magic projectile...");

            //Element
            switch (magicAttackElement)
            {
                case 1:
                    childRenderer.material.SetColor("_Color", Color.red);
                    break;
                case 2:
                    childRenderer.material.SetColor("_Color", Color.green);
                    break;
                case 3:
                    childRenderer.material.SetColor("_Color", Color.blue);
                    break;
            }

            Debug.Log("Element color set");

            //Speed
            float speedToSet = 0;

            switch (magicAttackSpeed)
            {
                case 1:
                    speedToSet = slowSpeedValue;
                    break;
                case 2:
                    speedToSet = mediumSpeedValue;
                    break;
                case 3:
                    speedToSet = fastSpeedValue;
                    break;
            }

            projectileSpeed = speedToSet;

            Debug.Log("Speed prepared");

            //Size
            float sizeValueToUse = 0;

            switch (magicAttackSize)
            {
                case 1:
                    sizeValueToUse = smallSizeValue;
                    break;
                case 2:
                    sizeValueToUse = mediumSizeValue;
                    break;
                case 3:
                    sizeValueToUse = largeSizeValue;
                    break;
            }

            //Set all our colliders so whichever one we're using is sized correctly
            blockCollider.radius = sizeValueToUse;
            blastCollider.size = new Vector3(sizeValueToUse * 2, sizeValueToUse * 2, sizeValueToUse / 5);
            boltCollider.radius = sizeValueToUse / 10;
            boltCollider.height = sizeValueToUse / 2;

            Debug.Log("Sizes set");

            //Type
            switch (magicAttackType)
            {
                case 1:
                    //We are doing a block
                    blockCollider.enabled = true;
                    blastCollider.enabled = false;
                    boltCollider.enabled = false;
                    childFilter.mesh = sphereMesh;
                    projectileRange = blockRange;
                    visibleObject.transform.localScale = new Vector3(sizeValueToUse, sizeValueToUse, sizeValueToUse);
                    spawnScale = visibleObject.transform.localScale;

                    //Blocks are special because they appear on the ground and expand outward, so make sure we're on the ground
                    //transform.position = foot position? (probably passed into the init method)
                    break;

                case 2:
                    //We are doing a blast
                    blockCollider.enabled = false;
                    blastCollider.enabled = true;
                    boltCollider.enabled = false;
                    childFilter.mesh = cubeMesh;
                    projectileRange = blastRange;
                    visibleObject.transform.localScale = new Vector3(sizeValueToUse * 2, sizeValueToUse / 5, sizeValueToUse * 2);
                    break;

                case 3:
                    //We are doing a bolt
                    blockCollider.enabled = false;
                    blastCollider.enabled = false;
                    boltCollider.enabled = true;
                    childFilter.mesh = capsuleMesh;
                    projectileRange = boltRange;
                    visibleObject.transform.localScale = new Vector3(sizeValueToUse / 5, sizeValueToUse / 5, sizeValueToUse / 5);
                    break;
            }

            Debug.Log("Collider enabled for type; scale applied for type");

            //Multiply projectile's range by size, so big projectiles shoot further!
            projectileRange *= sizeValueToUse;

            Debug.Log("Range set");

            //Make the child object that actually shows a mesh visible, and size it correctly
            childRenderer.enabled = true;
            Debug.Log("Child object visibility enabled");

            spawnPosition = spawnTransform.position;
            transform.position = spawnPosition;
            Debug.Log("Spawn position set to " + spawnTransform.position);

            //Look in the same direction the cube is looking (which should consequentially be the hand?)
            transform.rotation = Quaternion.LookRotation(spawnTransform.forward, Vector3.up);
            Debug.Log("Look Direction set");

            readyToFire = true;
        }

        private void CleanupMagicAttack()
        {
            Debug.Log("Cleaning up Magic Attack...");

            readyToFire = false;

            blockCollider.enabled = false;
            blastCollider.enabled = false;
            boltCollider.enabled = false;
            childFilter.mesh = null;
            childRenderer.enabled = false;

            transform.position = deathPosition.position;
            transform.localScale = Vector3.one;

            magicCircleDetector.CleanupMagicCircleDetector();
        }
    }
}