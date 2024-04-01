using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public LayerMask transparencyLayer; // Layer for objects that become transparent upon collision
    public LayerMask zoomLayer; // Layer for objects that trigger camera zoom upon collision
    public Material transparentMaterial;
    

    private Vector3 offset;
    private Vector3 originalOffset;
    private float zoomSpeed = 7f;
    private float minimumOffsetDistance = 8.5f;
    private Dictionary<Collider, Material> originalMaterials = new Dictionary<Collider, Material>();
    private bool isObstructed = false;
    private int zoomLayerCollisionCount = 0;


    void Start()
    {
        originalOffset = transform.position - player.transform.position;
        offset = originalOffset;

        BoxCollider cameraCollider = gameObject.AddComponent<BoxCollider>();
        cameraCollider.isTrigger = true;
        cameraCollider.size = new Vector3(0.5f, 0.5f, 13f);
        cameraCollider.center = new Vector3(0f, 0f, 5f);

        Rigidbody cameraRigidbody = gameObject.AddComponent<Rigidbody>();
        cameraRigidbody.isKinematic = true;
        cameraRigidbody.useGravity = false;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;

        if (isObstructed)
        {
            float distanceToZoom = Mathf.Max(minimumOffsetDistance, offset.magnitude - (zoomSpeed * Time.deltaTime));
            offset = offset.normalized * distanceToZoom;
        }
        else if (offset.magnitude < originalOffset.magnitude)
        {
            offset = Vector3.MoveTowards(offset, originalOffset, zoomSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for transparency layer collision
        if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0)
        {
            Renderer objRenderer = other.GetComponent<Renderer>();
            if (objRenderer != null && !originalMaterials.ContainsKey(other))
            {
                originalMaterials[other] = objRenderer.material;
                objRenderer.material = transparentMaterial;
            }
        }

        if (((1 << other.gameObject.layer) & zoomLayer.value) != 0) {
            zoomLayerCollisionCount++;
            isObstructed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Handle exiting transparency layer
        if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0 && originalMaterials.TryGetValue(other, out Material originalMaterial))
        {
            Renderer objRenderer = other.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                objRenderer.material = originalMaterial;
                originalMaterials.Remove(other);
            }
        }

        // Handle exiting zoom layer
        if (((1 << other.gameObject.layer) & zoomLayer.value) != 0) {
            zoomLayerCollisionCount--;
            if (zoomLayerCollisionCount <= 0) {
                isObstructed = false;
                zoomLayerCollisionCount = 0; // Ensure it doesn't go negative
            }
        }
    }
}
