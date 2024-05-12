using System;
using System.Collections;
using InputSystem;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class crate : MonoBehaviour
{
    public PlayerInputConfig playerInput;
    private bool _containAntiViral;
    private bool _wasSearched;
    private float _searchDuration;
    public Image progressBar;
    private bool _isSearching;
    private bool _isWithinCollider;
    private GameObject _uiCanvas;

    void Awake()
    {
        _uiCanvas = GameObject.FindGameObjectWithTag("InteractableUI");
        //Debug.Log(_uiCanvas);
        //Debug.Log(_uiCanvas.IsDestroyed());
    }
    void Start()
    {
        _uiCanvas.SetActive(false);
        _searchDuration = 5f; //Random.Range(3F, 8F);
        progressBar.fillAmount = 0;
        _isSearching = false;
        _wasSearched = false;
        _isWithinCollider = false;
        _containAntiViral = true; //  _containAntiViral = Random.value > 0.5;     
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_wasSearched)
        {
           // Debug.Log(_uiCanvas);
            _uiCanvas.SetActive(true);
            _isWithinCollider = true;
            //canvasManager.DisplayInteractableCanvas(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          _uiCanvas.SetActive(false);
            _isWithinCollider = false;
            if (_isSearching)
            {
                StopCoroutine(SearchCrate());
                progressBar.fillAmount = 0;
                _isSearching = false;
            }
        }
    }

    void Update()
    {
        if (playerInput.SearchInput && !_isSearching && !_wasSearched && _isWithinCollider)
        {
            StartCoroutine(SearchCrate());
        }
    }

    private IEnumerator SearchCrate()
    {
        _isSearching = true;
        float elapsedTime = 0;
        while (elapsedTime < _searchDuration)
        {
            if (!playerInput.SearchInput || !_isWithinCollider) 
            {
                progressBar.fillAmount = 0;
                _isSearching = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            progressBar.fillAmount = elapsedTime / _searchDuration;
            yield return null;
        }
        EndSearch();
        progressBar.fillAmount = 0;
        _wasSearched = true;
        _isSearching = false;
    }

    private void EndSearch()
    {
        // here to send notification to finite state machine / game manager that controls the game state.
        Debug.Log(_containAntiViral ? "Found antivirals!" : "No antivirals here.");
    }
}