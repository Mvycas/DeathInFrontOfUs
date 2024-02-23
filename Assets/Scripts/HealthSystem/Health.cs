using System;
using UnityEngine;

namespace HealthSystem
{
    public class Health
    {
        private float _currentHealth;
        private float _maxHealth;
        public event Action OnDeath; // Event to notify when health reaches zero

        
        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                _currentHealth = value;
            }
        }
        public float MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
            }
            
        }

        public Health(float maxHealth)
        {
            // not sure if this is good way to do things?? I mean, to set max health to current health here as well
            // but in theory this is constructor, so it is called only once?
            // during awake method in character class?
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            Debug.Log(_currentHealth);
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke(); // Trigger the death event
            }
        }

        public void Heal(float amount)
        {
            if (_currentHealth < _maxHealth)
            {
                _currentHealth += amount;
            }
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

    }
}