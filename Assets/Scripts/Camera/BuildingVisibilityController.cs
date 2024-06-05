using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingVisibilityController : MonoBehaviour
{
    public Material[] buildingMaterials;
    public float targetThreshold = 20f;
    private float _initialThreshold;
    public float enterDuration = 0.5f;
    public float exitDuration = 2f;

    private void Start()
    {
        if (buildingMaterials.Length > 0 && buildingMaterials[0].HasProperty("_HeightThreshold"))
        {
            _initialThreshold = buildingMaterials[0].GetFloat("_HeightThreshold");
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetMaterialThresholds(_initialThreshold);
    }

    private void OnDestroy()
    {
        ResetMaterialThresholds(_initialThreshold);
        SceneManager.sceneLoaded -= OnSceneLoaded;  //to prevent memory leaks
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, false, enterDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, true, exitDuration));
        }
    }

    private IEnumerator InterpolateHeightThreshold(float targetValue, bool reverting, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            foreach (Material material in buildingMaterials)
            {
                if (material.HasProperty("_HeightThreshold"))
                {
                    float startValue = reverting ? targetValue : _initialThreshold;
                    float endValue = reverting ? _initialThreshold : targetValue;

                    float newThreshold = Mathf.Lerp(startValue, endValue, time / duration);
                    material.SetFloat("_HeightThreshold", newThreshold);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void ResetMaterialThresholds(float value)
    {
        foreach (Material material in buildingMaterials)
        {
            if (material.HasProperty("_HeightThreshold"))
            {
                material.SetFloat("_HeightThreshold", value);
            }
        }
    }
}
