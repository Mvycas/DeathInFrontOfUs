using System;
using System.Collections;
using AISystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace HealthSystem
{
    public class Enemy : Character
    {
        private Animator anim;
        private AIController ai;  
        private Rigidbody rb;
        private CapsuleCollider col;
        private NavMeshAgent agent;
        private Character character;
        [SerializeField] private GameObject goreEffectPrefab; 
        [SerializeField] private int takenDamageAmount;
        [SerializeField] private float knockbackForce = 50f;
        [SerializeField] private float knockbackDuration = 0.5f;
        private Vector3 knockbackVector;
        private float knockbackTimer;
        
        public AudioClip[] audioClips; // Assign this in the Inspector
        private AudioSource audioSource;
        private AudioClip zombieSplash;


        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            ai = GetComponent<AIController>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<Character>();
            knockbackVector = Vector3.zero;
            knockbackTimer = 0;
        }
        
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null || audioClips.Length < 2)
            {
                Debug.LogError("Incorrect setup of Audio components or clips");
            }
            audioSource.clip = audioClips[1];
            audioSource.Play();
            audioSource.clip = audioClips[0];

        }

        private void Update()
        {
            if (knockbackTimer > 0)
            {
                float step = (knockbackForce / knockbackDuration) * Time.deltaTime;
                transform.position += knockbackVector * step;
                knockbackTimer -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("bullet"))
            {
                character.ApplyDamage(takenDamageAmount);
                Vector3 effectPosition = GetComponent<Collider>().bounds.center;
                Quaternion effectRotation = Quaternion.LookRotation(-collision.contacts[0].normal); 
                
                var goreEffectInstance = Instantiate(goreEffectPrefab, effectPosition, effectRotation);
                var ps = goreEffectInstance.GetComponent<ParticleSystem>();
                audioSource.Play();
                ps.Play();

                knockbackVector = -transform.forward;
                knockbackTimer = knockbackDuration;

                Destroy(goreEffectInstance, ps.main.duration);
            }
        }

        protected override void Die()
        {
            if (col != null)
            {
                rb.isKinematic = true;  // stop physics but keep the body in place
                rb.detectCollisions = false;
            }
            anim.SetBool("die", true);
            agent.enabled = false;
            ai.enabled = false;
            StartCoroutine(HandleDeath());
            
        }
        
        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(3f);
            //reset 
            ai.enabled = true;
            rb.isKinematic = false; 
            rb.detectCollisions = true;
            agent.enabled = true;
            gameObject.SetActive(false);
        }
    }
}

