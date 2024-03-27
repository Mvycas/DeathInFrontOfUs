using System.Collections;
using HealthSystem;
using UnityEngine;
using UnityEngine.AI;

namespace HealthSystem
{
    public class Enemy : Character
    {
        protected override void Die()
        {
            Debug.Log("Zombie died");
            gameObject.SetActive(false);
        }
    }
}