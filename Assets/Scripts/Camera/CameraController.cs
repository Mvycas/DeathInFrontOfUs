using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        private Dictionary<Collider, Material> originalMaterials = new Dictionary<Collider, Material>();

        [SerializeField] private GameObject player;
        [SerializeField] private Transform pauseViewTransform;
        [SerializeField] private LayerMask transparencyLayer;
        [SerializeField] private Material transparentMaterial;
        [SerializeField] private float minimumOffsetDistance = 8.5f;
        private Vector3 _offset;


        private Vector3 _originalOffset;
        private bool _isPaused;
        private bool IsTransitioning { get; set; }
        private Quaternion _originalRotation;
        private Coroutine _currentTransition;
        private static CameraController instance;
        
        
        private void Awake()
        {

            BoxCollider cameraCollider = gameObject.AddComponent<BoxCollider>();
            cameraCollider.isTrigger = true;
            cameraCollider.size = new Vector3(5f, 5f, 13f);
            cameraCollider.center = new Vector3(0f, 0f, 5f);

            Rigidbody cameraRigidbody = gameObject.AddComponent<Rigidbody>();
            cameraRigidbody.isKinematic = true;
            cameraRigidbody.useGravity = false;

            var orgCamTransform = transform; // Main cam. the one which is top down
            _originalOffset = orgCamTransform.position - player.transform.position;
            _offset = _originalOffset;
            _originalRotation = orgCamTransform.rotation;
        }
        
        void OnDestroy()
        {
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
            }
        }

        public void SetPause(bool isPaused)
        {
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
            }
            Debug.Log("CAMERA CONTROLLER");
            _isPaused = isPaused;
            _currentTransition = StartCoroutine(isPaused ? TransitionToPauseView() : TransitionToGameView());
        }

        private IEnumerator TransitionToPauseView()
        {
            IsTransitioning = true;
            while (Vector3.Distance(transform.position, pauseViewTransform.position) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, pauseViewTransform.position, Time.unscaledDeltaTime * 3);
                transform.rotation = Quaternion.Lerp(transform.rotation, pauseViewTransform.rotation, Time.unscaledDeltaTime * 3);
                yield return null;
            }

            IsTransitioning = false;
            _currentTransition = null; 
        }

        private IEnumerator TransitionToGameView()
        {
            IsTransitioning = true;
            while (Vector3.Distance(transform.position, player.transform.position + _originalOffset) > 0.1f ||
                   Quaternion.Angle(transform.rotation, _originalRotation) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, player.transform.position + _originalOffset, Time.unscaledDeltaTime * 9);
                transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, Time.unscaledDeltaTime * 9);
                yield return null;
            }
            IsTransitioning = false;
            _currentTransition = null; 
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0)
            {
                Renderer objRenderer = other.GetComponent<Renderer>();
                if (objRenderer != null && !originalMaterials.ContainsKey(other))
                {
                    originalMaterials[other] = objRenderer.material;
                    objRenderer.material = transparentMaterial;
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0 &&
                originalMaterials.TryGetValue(other, out Material originalMaterial))
            {
                Renderer objRenderer = other.GetComponent<Renderer>();
                if (objRenderer != null)
                {
                    objRenderer.material = originalMaterial;
                    originalMaterials.Remove(other);
                }
            }
        }

        void LateUpdate()
        {
            if (_isPaused || IsTransitioning)
                return;

            transform.position = player.transform.position + _offset;
        }
    }

