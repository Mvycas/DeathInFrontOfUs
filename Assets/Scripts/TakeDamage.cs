using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TakeDamage : MonoBehaviour
{
    public float maxIntensity = 0.3f;  
    private Vignette _vignette;
    private Coroutine fadeCoroutine = null;  
    private float fadeOutRate = 0.05f;     
    private float fadeOutDelay = 0.05f;     
    private float holdTime = 1.5f;     

    void Start()
    {
        Volume volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out _vignette))
        {
            _vignette.active = true;
            _vignette.intensity.value = 0f; 
        }
    }

    public void ApplyDamageEffect()
    {
        _vignette.intensity.value = Mathf.Min(_vignette.intensity.value + 0.1f, maxIntensity);

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeVignetteEffect());
    }

    IEnumerator FadeVignetteEffect()
    {
        yield return new WaitForSeconds(holdTime);

        while (_vignette.intensity.value > 0)
        {
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0f, fadeOutRate);
            yield return new WaitForSeconds(fadeOutDelay);
        }
        _vignette.intensity.value = 0f;  
    }

    public void OnPlayerDeath()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
}
