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
            _flickeringLights.Clear();
            LightFlicker[] allFlickerLights = FindObjectsOfType<LightFlicker>();
            foreach (LightFlicker lightFlicker in allFlickerLights)
            {
                _flickeringLights.Add(lightFlicker);
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