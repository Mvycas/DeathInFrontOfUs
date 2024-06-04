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
    public bool containAntiViral;
    public bool wasSearched;
    private float _searchDuration;
    public Image progressBar;
    private bool _isSearching;
    private bool _isWithinCollider;
    private GameObject _uiCanvas;

    void Awake()
    {
        _uiCanvas = GameObject.FindGameObjectWithTag("InteractableUI");
    }
    void Start()
    {
        _uiCanvas.SetActive(false);
        _searchDuration = Random.Range(3F, 8F);
        progressBar.fillAmount = 0;
        _isSearching = false;
        wasSearched = false;
        _isWithinCollider = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasSearched)
        {
            _uiCanvas.SetActive(true);
            _isWithinCollider = true;
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
        if (playerInput.SearchInput && !_isSearching && !wasSearched && _isWithinCollider)
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
        wasSearched = true;
        _isSearching = false;
    }

    private void EndSearch()
    {
        if (containAntiViral)
        {
            GameStateManager.Instance.Victory();
        }
        Debug.Log(containAntiViral ? "Found antivirals!" : "No antivirals here.");
    }
}