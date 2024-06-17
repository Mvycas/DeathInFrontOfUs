using UnityEngine;
using InputSystem;

public class FlashlightController : MonoBehaviour
{
    private Light flashLight; // Reference to the flashlight component
    private PlayerInputConfig inputConfig; // Reference to the PlayerInputConfig
    private bool isLightOn = false; // State tracking whether the light is on

    void Start()
    {
        // Find the Light GameObject that is a child of this GameObject (Megan)
        GameObject lightObject = transform.Find("Light").gameObject;
        if (lightObject != null)
        {
            flashLight = lightObject.GetComponent<Light>();
        }

        // Initialize light off if the Light component is found
        if (flashLight != null)
        {
            flashLight.intensity = 0;
        }
        else
        {
            Debug.LogError("No Light component found on 'Light' GameObject.");
        }

        // Find the PlayerInputConfig component in the scene
        inputConfig = GameObject.FindObjectOfType<PlayerInputConfig>();
        if (inputConfig == null)
        {
            Debug.LogError("PlayerInputConfig component not found in the scene.");
        }
    }

    void Update()
    {
        // Check if the flash trigger has been pressed
        if (inputConfig != null && inputConfig.FlashTrigger)
        {
            // Toggle the flashlight
            ToggleLight();
        }
    }

    private void ToggleLight()
    {
        if (flashLight == null)
        {
            return;
        }

        // Toggle the light's intensity
        if (isLightOn)
        {
            flashLight.intensity = 0;
        }
        else
        {
            flashLight.intensity = 80;
        }
        isLightOn = !isLightOn;
    }
}
