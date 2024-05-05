using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class UIInputConfig : MonoBehaviour
    {
        private PlayerInput _playerInput;
        public InputAction menuNavigateAction;
        public InputAction menuSelectAction;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            menuNavigateAction = _playerInput.actions["Navigate"];
            menuSelectAction = _playerInput.actions["Submit"];
        }

        private void OnEnable()
        {
            // need to reformat some code and put check if the game state is paused or not...
            menuNavigateAction.Enable();
            menuSelectAction.Enable();
        }

        private void OnDisable()
        {
            menuNavigateAction.Disable();
            menuSelectAction.Disable();
        }

    }
}