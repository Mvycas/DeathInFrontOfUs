using UnityEngine;

namespace HealthSystem
{
    public class Player : Character
    {
        protected override void Awake()
        {
            // this calls characters awake method
            // It is required! idk why compiler greyed it out,
            // without this call it would be null pointer smth
           
            base.Awake(); 
            
            // Player-specific initialization to implement later on
        }

        protected override void Die()
        {
            Debug.Log("Player died!");
            // animations? // death UI? To continue and respawn or to quit game? // Sound effects?
        }
    }
}