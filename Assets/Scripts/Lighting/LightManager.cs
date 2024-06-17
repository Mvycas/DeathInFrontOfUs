using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lighting
{
    public class LightManager : MonoBehaviour
    {
        public static LightManager Instance { get; private set; }

        private readonly Color _daylightColor = new Color(1f, 0.95f, 0.9f); // Daylight
        private readonly Color _sunsetColor = new Color(0.98f, 0.82f, 0.65f); // SUNSET
        private readonly Color _moonlightColor = new Color(0.5f, 0.6f, 1f); // Moonlight

        public Light sunLight;
        public Light pointLight; // Megan's flashlight
        private Coroutine sunDimmingCoroutine;


        private readonly List<Light> _poleLights = new List<Light>();

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

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            pointLight = GameObject.FindGameObjectWithTag("Player").transform.Find("Light").GetComponent<Light>();
            InitializePoleLights();
            FlickerManager.Instance.InitializeFlickeringLights();
            FindAndSetSunLight();
        }

        private void FindAndSetSunLight()
        {
            GameObject sunObject = GameObject.FindGameObjectWithTag("Sun");
            if (sunObject != null)
            {
                sunLight = sunObject.GetComponent<Light>();
                sunLight.color = _daylightColor;
            }
            else
            {
                Debug.LogError("SUN object not found.");
            }
        }

        private void InitializePoleLights()
        {
            GameObject[] lights = GameObject.FindGameObjectsWithTag("PoleLight");
            foreach (GameObject lightObject in lights)
            {
                Light lightComponent = lightObject.GetComponent<Light>();
                if (lightComponent != null)
                {
                    _poleLights.Add(lightComponent);
                }
            }
        }

        public void StartSunDimming()
        {
            if (sunDimmingCoroutine != null)
            {
                StopCoroutine(sunDimmingCoroutine);
                sunDimmingCoroutine = null;
            }
            sunDimmingCoroutine = StartCoroutine(DimSunIntensityAndChangeColor(30, 120));
        }

        IEnumerator DimSunIntensityAndChangeColor(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);

            float startIntensity = sunLight.intensity;
            float time = 0;
            float halfDuration = duration / 2;

            while (time < duration)
            {
                float normalizedTime = time / duration;
                sunLight.intensity = Mathf.Lerp(startIntensity, 0, normalizedTime);
                
                if (time <= halfDuration)
                {
                    sunLight.color = Color.Lerp(_daylightColor, _sunsetColor, normalizedTime * 2);
                }
                else
                {
                    sunLight.color = Color.Lerp(_sunsetColor, _moonlightColor, (normalizedTime - 0.5f) * 2);
                }

                time += Time.deltaTime;
                yield return null;
            }
            
            NightTime();
            sunLight.intensity = 0;
            sunLight.color = _moonlightColor;
        }

        private void NightTime()
        {
            if (pointLight != null)
            {
                pointLight.intensity = 80;
            }

            foreach (Light poleLight in _poleLights)
            {
                if (poleLight != null)
                {
                    poleLight.intensity = 110;
                }
            }
            FlickerManager.Instance.ActivateFlickeringLights();
        }
    }
}