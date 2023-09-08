using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        
        [SerializeField] private float walkSpeed = 10f;
        [SerializeField] private float runSpeedIncrement = 5f;
        [SerializeField] private float jumpForce = 10f;

        [SerializeField] private Transform groundDetector;
        [SerializeField] private float groundCheckRadius = .2f;
        [SerializeField] private LayerMask groundCheckLayer;

        [SerializeField] private bool isGrounded;

        [SerializeField] private bool canMove;
        [SerializeField] private bool isSprinting;
        [SerializeField] private bool isCrouched;
        //[SerializeField] private bool isJumping;
        
        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;

        private PlayerInputActions _playerInputActions;
        
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int RunJump = Animator.StringToHash("Run_Jump");
        private static readonly int JumpAnim = Animator.StringToHash("Jump");
        private static readonly int CrouchIdle = Animator.StringToHash("Crouch_Idle");
        private static readonly int CrouchWalk = Animator.StringToHash("Crouch_Walk");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Jump.performed += Jump;
            _playerInputActions.Player.Movement.performed += MovementPerformed;
        }

        private void Start()
        {
            anim.SetBool(Idle, true);
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundDetector.position, groundCheckRadius, groundCheckLayer);
            
            if(_rigidbody.velocity.x is > 0.05f or < -0.05f && !isCrouched)
            {
                canMove = false;
                anim.SetBool(Idle, true);
            }
        }

        private void FixedUpdate()
        {
            if(!canMove) return;
            
            var inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
            var speed = walkSpeed;
            
            ResetAnimBool();
            anim.SetBool(Walk, true);
            
            if (isSprinting)
            {
                anim.SetBool(Run, true);
                speed += runSpeedIncrement;
            }

            var velocity = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(velocity.x, velocity.y, -inputVector.x) * speed;
        }

        public void MovementPerformed(InputAction.CallbackContext context)
        {
            Debug.Log($"Moving for :: {context}");

            canMove = true;
            var inputVector = context.ReadValue<Vector2>();
            transform.rotation = inputVector.x switch
            {
                < 0 => Quaternion.Euler(new Vector3(0, 0, 0)),
                >= 0 => Quaternion.Euler(new Vector3(0, 180, 0)),
                _ => Quaternion.Euler(new Vector3(0, 180, 0))
            };
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            Debug.Log("Sprint");

            isSprinting = true;
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (!isGrounded) return;

            Debug.Log("Jump");
            
            /*var velocity = _rigidbody.velocity;
            velocity = new Vector3(velocity.x, jumpForce, velocity.z);
            _rigidbody.velocity = velocity;*/
            //isJumping = true;
            ResetAnimBool();
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }

        private void ResetAnimBool()
        {
            Debug.Log("All anims reset");

            anim.SetBool(Idle, false);
            anim.SetBool(Walk, false);
            anim.SetBool(Run, false);
            anim.SetBool(RunJump, false);
            anim.SetBool(JumpAnim, false);
            anim.SetBool(CrouchIdle, false);
            anim.SetBool(CrouchWalk, false);
        }
        
        private void JumpCompleted()
        {
            Debug.Log("Jump completed");
            //isJumping = false;
            ResetAnimBool();
            anim.SetBool(Idle, true);
        }
    }
}