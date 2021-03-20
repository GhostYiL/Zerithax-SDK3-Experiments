
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax
{
    public class Firework : UdonSharpBehaviour
    {
        //[SerializeField] private GameObject[] objectManager;
        [SerializeField] private Transform deathPosition;

        [Header("Firework Attributes")]
        [SerializeField] private float maxFleightDistance;
        [SerializeField] private float flightSpeed;

        [Header("Particles")]
        [SerializeField] private ParticleSystem particleTrail;
        [SerializeField] private ParticleSystem particleExplosion;

        [Header("Firework States")]
        [SerializeField] private bool flying = false;
        [SerializeField] private bool canDie = false;

        private Vector3 originalPosition;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            originalPosition = Vector3.zero;
        }

        private void Update()
        {
            if (flying) FlyAway();

            if (canDie && particleExplosion.particleCount > 30) Die();
        }

        public void SetupFlying(Vector3 startPosition, Vector3 direction)
        {
            //Debug.Log("Attempting to Start Flying...");

            particleTrail.Pause();

            originalPosition = startPosition;
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            flying = true;

        }

        private void FlyAway()
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
            //transform.localPosition += transform.forward * flightSpeed;

            rb.AddForce(transform.forward * flightSpeed * 50);

            particleTrail.Play();

            if (Vector3.Distance(originalPosition, transform.position) >= maxFleightDistance)
            {
                particleTrail.Pause();
                particleExplosion.Play();
                canDie = true;
            }
        }

        private void Die()
        {
            canDie = false;

            //For now just TP away and disable flying
            rb.velocity = Vector3.zero;
            transform.SetPositionAndRotation(deathPosition.position, deathPosition.rotation);
            particleTrail.Play();
            flying = false;
        }
    }
}