using UnityEngine;

namespace ParticlesSystem
{
    public class GoreZombie : MonoBehaviour
    {
        public ParticleSystem goreEffect;

        private void OnCollisionEnter(Collision collision)
        {
        
            if (collision.gameObject.CompareTag("bullet"))
            {
                goreEffect.Play();
            }

        
        }
    }
}
