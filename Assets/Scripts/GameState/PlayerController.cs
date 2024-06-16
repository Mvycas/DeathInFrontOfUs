using MovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameState
{
    public class PlayerController : MonoBehaviour
    {
        public Movement movementComponent;
        public PlayerInput inputComponent;
        private InputAction _mouseClickAction;

        protected void Awake()
        {
            movementComponent = GetComponent<Movement>();
            inputComponent = GetComponent<PlayerInput>();
            _mouseClickAction = inputComponent.actions["Attack"];

        }

        public void EnableControls(bool enable)
        {
            movementComponent.enabled = enable;
            
            if (enable)
                 _mouseClickAction.Enable();
            else
                 _mouseClickAction.Disable();
        }
    }
}