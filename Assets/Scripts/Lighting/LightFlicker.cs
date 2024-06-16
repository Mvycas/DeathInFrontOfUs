using System.Collections;
using UnityEngine;

namespace Lighting
{
    public class LightFlicker : MonoBehaviour
    {
        public Light flickerLight; 
        public float minIntensity = 30f; 
        public float maxIntensity = 109f;  
        public Color normalColor = Color.white;  
        public Color flickerColor = Color.yellow; 
        private Coroutine _flickerCoroutine;  
        private void Start()
        {
            if (flickerLight == null)
                flickerLight = GetComponent<Light>();
        }
    
        public void ActivateFlickering()
        {
            if (_flickerCoroutine != null)
                StopCoroutine(_flickerCoroutine); 

            _flickerCoroutine = StartCoroutine(Flicker());
        }

        private IEnumerator Flicker()
        {
            yield return new WaitForSeconds(0.5f); //bypass
            while (true)
            {
                flickerLight.intensity = Random.Range(minIntensity, maxIntensity);

                flickerLight.color = (Random.value > 0.5) ? normalColor : flickerColor;

                yield return new WaitForSeconds(Random.Range(0.3f, 1.2f));
            }
        }
    }
}