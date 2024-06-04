using System;
using UnityEngine;

namespace HealthSystem
{
    public class Health
    {
        public event Action OnDeath; // Event to notify when health reaches zero


        private float CurrentHealth { get; set; }

        private float MaxHealth { get; set; }

        public Health(float maxHealth)
        {
            // not sure if this is good way to do things?? I mean, to set max health to current health here as well
            // but in theory this is constructor, so it is called only once?
            // during awake method in character class?
            CurrentHealth = maxHealth;
            MaxHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;
            Debug.Log(CurrentHealth);
            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke(); // Trigger the death event
            }
        }
        public float GetCurrentHealth()
        {
            return CurrentHealth;
        }

        public void Heal(float amount)
        {
            if (CurrentHealth < MaxHealth)
            {
                CurrentHealth += amount;
            }
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

    }
}