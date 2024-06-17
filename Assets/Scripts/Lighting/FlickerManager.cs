using System.Collections.Generic;
using UnityEngine;

namespace Lighting
{
    public class FlickerManager : MonoBehaviour
    {
        public static FlickerManager Instance { get; private set; }

        private readonly List<LightFlicker> _flickeringLights = new List<LightFlicker>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void InitializeFlickeringLights()
        {
            GameObject[] lights = GameObject.FindGameObjectsWithTag("PoleLight");
            foreach (GameObject lightObject in lights)
            {
                LightFlicker lightFlicker = lightObject.GetComponent<LightFlicker>();
                if (lightFlicker != null)
                {
                    _flickeringLights.Add(lightFlicker);
                }
            }
        }

        public void ActivateFlickeringLights()
        {
            foreach (LightFlicker flickerLight in _flickeringLights)
            {
                if (flickerLight != null)
                {
                    flickerLight.ActivateFlickering();
                }
            }
        }
    }
}