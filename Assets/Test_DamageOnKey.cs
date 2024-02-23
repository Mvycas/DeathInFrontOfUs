using UnityEngine;

public class Test_DamageOnKey : MonoBehaviour
{
    private Character player; // Now private and will be set in Awake or Start
    public float damageAmount = 10f; // The amount of damage to apply

    void Awake()
    {
        // Automatically get the Character component attached to the same GameObject
        player = GetComponent<Character>();
        
        // Optional: Check if player is null to avoid future NullReferenceException
        if (player == null)
        {
            Debug.LogError("Character component not found on the GameObject", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar was pressed this frame
        if (Input.GetKeyDown(KeyCode.Space) && player != null)
        {
            // Apply damage to the player
            player.ApplyDamage(damageAmount);
        }
    }
}