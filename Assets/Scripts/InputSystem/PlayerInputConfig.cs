using System;
using GameState;
using MovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class PlayerInputConfig : MonoBehaviour
    {
        private UnityEngine.InputSystem.PlayerInput _playerInput;
        private bool _attackInput;
        private bool _specialAttackInput;
        private Vector2 _movementInput;
        private bool _jumpInput;
        private bool _changeCameraModeInput;
        private bool _esc;
        private bool _searchInput;
        private bool _flashTrigger;



        public bool AttackInput {get => _attackInput;}
        public Vector2 MovementInput {get => _movementInput;}
        public bool JumpInput { get => _jumpInput; }
        public bool SearchInput { get => _searchInput; }
        public bool FlashTrigger
        {
            get => _flashTrigger; 
        }

        private void Start()
        {
            _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        }

        private void Update()
        {
            _attackInput = _playerInput.actions["Attack"].IsPressed();
            _movementInput = _playerInput.actions["Move"].ReadValue<Vector2>();
            _jumpInput = _playerInput.actions["Jump"].triggered;
            _searchInput = _playerInput.actions["Search"].IsPressed();
            _flashTrigger = _playerInput.actions["Flash"].triggered;
            if (!_playerInput.actions["Menu"].triggered) return;
            GameStateManager.Instance.TogglePause(); 
        }
    }
}