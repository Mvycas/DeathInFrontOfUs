using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class CameraController : MonoBehaviour
    {
        public GameObject player;
        public LayerMask transparencyLayer; // Layer for objects to become transparent upon collision
        public LayerMask zoomLayer; // Layer for objects that trigger camera zoom upon collision
        public Material transparentMaterial;
    

        private Vector3 _offset;
        private Vector3 _originalOffset;
        public float zoomSpeed = 7f;
        public float minimumOffsetDistance = 8.5f;
        private readonly Dictionary<Collider, Material> _originalMaterials = new Dictionary<Collider, Material>();
        private bool _isObstructed;
        private int _zoomLayerCollisionCount;
    
    
        //Pause view
        public Transform pauseViewTransform; 
        private bool _isPaused;
        public bool IsTransitioning { get; private set; }
        private Quaternion _originalRotation; // org rotation OF Main cam.
        private Coroutine _currentTransition; 



        private void OnEnable()
        {
            GameEvents.instance.onPause.AddListener(HandlePauseToggle);
        }

        private void OnDisable()
        {
            GameEvents.instance.onPause.RemoveListener(HandlePauseToggle);
        }

        public void HandlePauseToggle(bool isPaused)
        {
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
            }

            this._isPaused = isPaused;
            _currentTransition = StartCoroutine(isPaused ? TransitionToPauseView() : TransitionToGameView());
        }

        void Start()
        {
            _originalOffset = transform.position - player.transform.position;
            _offset = _originalOffset;
            _originalRotation = transform.rotation; 

            BoxCollider cameraCollider = gameObject.AddComponent<BoxCollider>();
            cameraCollider.isTrigger = true;
            cameraCollider.size = new Vector3(0.5f, 0.5f, 13f);
            cameraCollider.center = new Vector3(0f, 0f, 5f);

            Rigidbody cameraRigidbody = gameObject.AddComponent<Rigidbody>();
            cameraRigidbody.isKinematic = true;
            cameraRigidbody.useGravity = false;
        
        }

        private IEnumerator TransitionToPauseView()
        {
            IsTransitioning = true;
            while (Vector3.Distance(transform.position, pauseViewTransform.position) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, pauseViewTransform.position, Time.deltaTime * 3);
                transform.rotation = Quaternion.Lerp(transform.rotation, pauseViewTransform.rotation, Time.deltaTime * 3);
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
                transform.position = Vector3.Lerp(transform.position, player.transform.position + _originalOffset, Time.deltaTime * 9);
                transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, Time.deltaTime * 9);
                yield return null;
            }
            IsTransitioning = false;
            _currentTransition = null; 
        }


        void LateUpdate()
        {
            if (_isPaused || IsTransitioning) 
                return;
            transform.position = player.transform.position + _offset;
        

            if (_isObstructed)
            {
                float distanceToZoom = Mathf.Max(minimumOffsetDistance, _offset.magnitude - (zoomSpeed * Time.deltaTime));
                _offset = _offset.normalized * distanceToZoom;
            }
            else if (_offset.magnitude < _originalOffset.magnitude)
            {
                _offset = Vector3.MoveTowards(_offset, _originalOffset, zoomSpeed * Time.deltaTime);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Check for transparency layer collision
            if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0)
            {
                Renderer objRenderer = other.GetComponent<Renderer>();
                if (objRenderer != null && !_originalMaterials.ContainsKey(other))
                {
                    _originalMaterials[other] = objRenderer.material;
                    objRenderer.material = transparentMaterial;
                }
            }

            if (((1 << other.gameObject.layer) & zoomLayer.value) != 0) {
                _zoomLayerCollisionCount++;
                _isObstructed = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Handle exiting transparency layer
            if (((1 << other.gameObject.layer) & transparencyLayer.value) != 0 && _originalMaterials.TryGetValue(other, out Material originalMaterial))
            {
                Renderer objRenderer = other.GetComponent<Renderer>();
                if (objRenderer != null)
                {
                    objRenderer.material = originalMaterial;
                    _originalMaterials.Remove(other);
                }
            }

            // Handle exiting zoom layer
            if (((1 << other.gameObject.layer) & zoomLayer.value) != 0) {
                _zoomLayerCollisionCount--;
                if (_zoomLayerCollisionCount <= 0) {
                    _isObstructed = false;
                    _zoomLayerCollisionCount = 0; 
                }
            }
        }
    }
}
