using InputSystem;
using UnityEngine;

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

        private Animator _animator;
        private CharacterController _characterController;
        private PlayerInputConfig _playerInputConfig;

        private const float Gravity = -9.81f;
        private float _verticalVelocity;
        private bool _isJumping;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _playerInputConfig = GetComponent<PlayerInputConfig>();
        }

        private void Update()
        {
            HandleMovement(_playerInputConfig.MovementInput);
            HandleJump();
            HandleGravity();
        }

        private void HandleMovement(Vector2 input)
        {
            Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

            if (moveDirection.magnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                Vector3 movement = transform.forward * (maxSpeed * Time.deltaTime);
                _characterController.Move(movement);
            }

            // Update animator with movement input
            _animator.SetFloat("input_X", input.x);
            _animator.SetFloat("input_Y", input.y);
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

            // Optionally, check if the ground is within a certain layer (e.g., a ground layer)
            // bool isGrounded = hit.collider != null && groundLayers == (groundLayers | (1 << hit.collider.gameObject.layer));

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
