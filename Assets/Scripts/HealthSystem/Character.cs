using HealthSystem;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    private Health health;

    protected virtual void Awake()
    {
        //Initialize Character health with data coming from CharacterData asset file
        health = new Health(characterData.maxHealth);
        // Subscribe to the OnDeath event
        health.OnDeath += Die;
    }
    
    public void ApplyDamage(float damage)
    {
        health.TakeDamage(damage);
    }

    // The Die method is abstract - it will be implemented differently by every subclass
    protected abstract void Die();
}

