using System.Collections;
using UnityEngine;

public class BuildingVisibilityController : MonoBehaviour
{
    public Material[] buildingMaterials; // Materials that have the HeightThreshold property
    public float targetThreshold = 20f; // Target height of the building // maximum height of the building upon entrance
    private float _initialThreshold; // initial value of height threshold // used as a reference when reverting back
    public float enterDuration = 0.5f; // Duration of the interpolation for trigger enter
    public float exitDuration = 2f; // Duration of the interpolation for trigger exit

    private void Start()
    {
        // Actually all materials must have the same initial threshold for one specific object,
        // so we just need to check the first one for this value:
        if (buildingMaterials.Length > 0 && buildingMaterials[0].HasProperty("_HeightThreshold"))
        {
            _initialThreshold = buildingMaterials[0].GetFloat("_HeightThreshold");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any existing interpolation coroutines
            StopAllCoroutines(); 
            // Start interpolation towards the target thresholds
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, false, enterDuration)); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any existing interpolation coroutines
            StopAllCoroutines(); 
            // Start interpolation back to the initial threshold
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, true, exitDuration)); 
        }
    }

    // Unified coroutine for interpolating the height threshold
    private IEnumerator InterpolateHeightThreshold(float targetValue, bool reverting, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            foreach (Material material in buildingMaterials)
            {
                if (material.HasProperty("_HeightThreshold"))
                {
                    // Determine the start and end values based on whether we are reverting or not
                    float startValue = reverting ? targetValue : _initialThreshold;
                    float endValue = reverting ? _initialThreshold : targetValue;

                    float newThreshold = Mathf.Lerp(startValue, endValue, time / duration);
                    material.SetFloat("_HeightThreshold", newThreshold);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        // After interpolation, set the height threshold to the exact target or initial value
        foreach (Material material in buildingMaterials)
        {
            if (material.HasProperty("_HeightThreshold"))
            {
                material.SetFloat("_HeightThreshold", reverting ? _initialThreshold : targetValue);
            }
        }
    }
}
