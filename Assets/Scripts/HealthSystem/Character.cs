using Interfaces;
using ScriptableCharactersData;
using UnityEngine;

namespace HealthSystem
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        [SerializeField] private CharacterData characterData;
        private Health _health;

        protected virtual void Awake()
        {
            //Initialize Character health with data coming from CharacterData asset file
            _health = new Health(characterData.maxHealth);
            // Subscribe to the OnDeath event
            _health.OnDeath += Die;
        }
    
        public void ApplyDamage(float damage)
        {
            _health.TakeDamage(damage);
            Debug.Log(damage);
        }

        // The Die method is abstract - it will be implemented differently by every subclass
        protected abstract void Die();
    }
}

