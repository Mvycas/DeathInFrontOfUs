using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    // Layer mask of obstruction objects (That needs to be diffused)
    public LayerMask obstructionLayer; 
    // Material to apply when objects are obstructing the camera view
    public Material transparentMaterial; 

    private Vector3 offset;
    
    // Dictionary to store collided obj material before diffusion
    // used to revert back from transparent to the org material
    // when it does not obstruct camera view anymore
    private Dictionary<Collider, Material> originalMaterials = new Dictionary<Collider, Material>(); 

    void Start()
    {
        offset = transform.position - player.transform.position;

        // Add and configure the camera collider as a trigger
        // // later used for the obstruction diffusion logic
        BoxCollider cameraCollider = gameObject.AddComponent<BoxCollider>();
        cameraCollider.isTrigger = true;
        // Spawned collider size
        cameraCollider.size = new Vector3(4f, 4f, 4f);
        // Center the collider
        cameraCollider.center = new Vector3(0f, 0f, 5f);  

        // Add a Rigidbody to camera to interact with triggers,
        // set to kinematic since we don't want physics forces applied
        Rigidbody cameraRigidbody = gameObject.AddComponent<Rigidbody>();
        cameraRigidbody.isKinematic = true;
        cameraRigidbody.useGravity = false;
    }

    void LateUpdate()
    {
        // Move the camera to maintain the offset from the player
        transform.position = player.transform.position + offset;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object is on the obstruction layer
        if (((1 << other.gameObject.layer) & obstructionLayer) != 0)
        {
            Renderer objRenderer = other.GetComponent<Renderer>();
            if (objRenderer != null && !originalMaterials.ContainsKey(other))
            {
                // Save the original material and set to transparent
                originalMaterials[other] = objRenderer.material;
                objRenderer.material = transparentMaterial;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object is in the dictionary of obstructions
        if (originalMaterials.TryGetValue(other, out Material originalMaterial))
        {
            Renderer objRenderer = other.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                // Restore the original material
                objRenderer.material = originalMaterial;
                // Remove from the dictionary
                originalMaterials.Remove(other);
            }
        }
    }
}
