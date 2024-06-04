using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public Transform player;       

    private Transform targetCrate;  // the nearest unopened crate

    public Transform objTransform;      // Reference to an obj that is attached to player.
                                       // This is so that the arrow would not jump around when the player rotates,
                                      // because the arrow is attached to the player.

    void Update()
    {
        targetCrate = CrateManager.Instance.GetNearestUnopenedCrate(player.position).transform;

        if (targetCrate != null)
        {
            // Calculate the direction to the target crate RELATIVE TO CERTAIN OBJECT
            Vector3 targetDirection = targetCrate.position - objTransform.position;
            targetDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, objTransform.up); // calculate target rotation using obj ref
            
            // rotate on Y-axis ONLY
            float targetYRotation = targetRotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(90f, targetYRotation, 0f); 
        }
    }
}
