using UnityEngine;

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

        public bool AttackInput {get => _attackInput;}
        public bool SpecialAttackInput {get => _specialAttackInput;}
        public Vector2 MovementInput {get => _movementInput;}
        public bool JumpInput { get => _jumpInput; }
        public bool ChangeCameraModeInput {get => _changeCameraModeInput;}

        private void Start()
        {
            _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        }

        private void Update()
        {
            _attackInput = _playerInput.actions["Attack"].IsPressed();
            //_specialAttackInput = Input.GetMouseButtonDown(1);

            _movementInput = _playerInput.actions["Move"].ReadValue<Vector2>();
            _jumpInput = _playerInput.actions["Jump"].triggered;
            //_changeCameraModeInput = Input.GetKeyDown(KeyCode.F);
        }
    }
}