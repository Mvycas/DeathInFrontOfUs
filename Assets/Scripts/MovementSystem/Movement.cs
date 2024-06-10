using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputConfig))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private float gravityMultiplier = 3.0f;
        [SerializeField] private Camera playerCamera; 


        private Animator _animator;
        private CharacterController _characterController;
        private PlayerInputConfig _playerInputConfig;

        private const float Gravity = -9.81f;
        private float _verticalVelocity;
        private bool _isJumping;
        private bool _isPaused;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _playerInputConfig = GetComponent<PlayerInputConfig>();
        }

        private void Update()
        {
            if (_isPaused)
                return;

            HandleMovement(_playerInputConfig.MovementInput);
            HandleJump();
            HandleGravity();
            HandleMouseRotation();
        }
        
        private void HandleMouseRotation()
        {
            if (playerCamera == null) return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float distanceToPlane = Mathf.Abs(playerCamera.transform.position.y - transform.position.y);
            Vector3 mouseWorldPosition = playerCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToPlane));
            Vector3 direction = (mouseWorldPosition - transform.position);
            direction.y = 0;  

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        private void HandleMovement(Vector2 input)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * input.y + cameraRight * input.x;
            moveDirection.Normalize(); 

            Vector3 localMoveDirection = transform.InverseTransformDirection(moveDirection);
            float forwardAmount = localMoveDirection.z;
            float rightAmount = localMoveDirection.x;

            if (moveDirection.magnitude > 0.01f)
            {
                _characterController.Move(moveDirection * (maxSpeed * Time.deltaTime));
                _animator.SetFloat("input_X", rightAmount);
                _animator.SetFloat("input_Y", forwardAmount);
            }
            else
            {
                _animator.SetFloat("input_X", 0);
                _animator.SetFloat("input_Y", 0);
            }

            Debug.Log($"input: {input.x}, {input.y} - move_dir: {moveDirection.magnitude}");
        }

        private void HandleJump()
        {
            bool isCurrentlyGrounded = CheckIsGrounded();
    
            // Handle the start of the jump
            if (_playerInputConfig.JumpInput && isCurrentlyGrounded && !_isJumping)
            {
                _verticalVelocity = jumpSpeed;
                _isJumping = true;
                if (!_animator.GetBool("jump")) // Only set if not already jumping
                {
                    _animator.SetBool("jump", true); // Start the jump animation
                }
            }
            else if (_isJumping && isCurrentlyGrounded) // Handle the end of the jump
            {
                _isJumping = false;
                if (_animator.GetBool("jump")) // Only set if currently jumping
                {
                    _animator.SetBool("jump", false); // End the jump animation
                }
            }
        }

        private bool CheckIsGrounded()
        {
            // Raycast down to check for ground 
            RaycastHit hit;
            float checkDistance = 0.35f; // Should be slightly more than the height of the character's step offset
            bool isGrounded = Physics.Raycast(transform.position, -Vector3.up, out hit, checkDistance);

            return isGrounded;
        }

        private void HandleGravity()
        {
            if (_characterController.isGrounded && !_isJumping)
            {
                _verticalVelocity = 0f;
            }
            else
            {
                _verticalVelocity += Gravity * gravityMultiplier * Time.deltaTime;
            }

            Vector3 gravityMovement = new Vector3(0f, _verticalVelocity, 0f);
            _characterController.Move(gravityMovement * Time.deltaTime);
        }
    }
}
