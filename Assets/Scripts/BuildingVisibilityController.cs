using UnityEngine;
using System.Collections;

public class BuildingVisibilityController : MonoBehaviour
{
    public Material[] buildingMaterials; // Materials that have the HeightThreshold property
    public float targetThreshold = 20f; // Target height of the building // maximum height of the building upon entrance
    private float[] _initialThresholds; // Array to hold the initial values // In case specific building has/owns more than one material
    public float enterDuration = 0.5f; // Duration of the interpolation for trigger enter
    public float exitDuration = 2f; // Duration of the interpolation for trigger exit

    private void Start()
    {
        // Initialize the initialThresholds array to hold the initial height threshold values of each material.
        // This is crucial because different materials may have distinct initial threshold values.
        // Failing to store these values could lead to inconsistencies in restoring thresholds on OnTriggerExit.
        _initialThresholds = new float[buildingMaterials.Length];
        for (int i = 0; i < buildingMaterials.Length; i++)
        {
            if (buildingMaterials[i].HasProperty("_HeightThreshold"))
            {
                _initialThresholds[i] = buildingMaterials[i].GetFloat("_HeightThreshold");
            }
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
            // Start interpolation back to the initial thresholds
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, true, exitDuration));
        }
    }

    // Unified coroutine for interpolating the height threshold
    private IEnumerator InterpolateHeightThreshold(float targetValue, bool reverting, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            for (int i = 0; i < buildingMaterials.Length; i++)
            {
                if (buildingMaterials[i].HasProperty("_HeightThreshold"))
                {
                    // Determine the start and end values based on whether we are reverting or not
                    float startValue = reverting ? targetValue : _initialThresholds[i];
                    float endValue = reverting ? _initialThresholds[i] : targetValue;

                    float newThreshold = Mathf.Lerp(startValue, endValue, time / duration);
                    buildingMaterials[i].SetFloat("_HeightThreshold", newThreshold);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        // After interpolation, set the height threshold to the exact target or initial value
        for (int i = 0; i < buildingMaterials.Length; i++)
        {
            if (buildingMaterials[i].HasProperty("_HeightThreshold"))
            {
                buildingMaterials[i].SetFloat("_HeightThreshold", reverting ? _initialThresholds[i] : targetValue);
            }
        }
    }
}
