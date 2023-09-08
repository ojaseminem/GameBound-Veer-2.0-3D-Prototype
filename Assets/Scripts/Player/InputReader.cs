using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputReader : MonoBehaviour, PlayerInputActions.IPlayerActions
    {
        public Vector2 mouseDelta;
        public Vector2 moveComposite;

        public bool isSprinting;
        public bool crouchCancelled;
        
        public Action OnJumpPerformed;
        public Action OnCrouchPerformed;
        public Action OnAttackPerformed;

        private PlayerInputActions _playerInputActions;

        private void OnEnable()
        {
            if (_playerInputActions != null)
                return;

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.SetCallbacks(this);
            _playerInputActions.Player.Enable();
        }

        public void OnDisable()
        {
            _playerInputActions.Player.Disable();
        }

        /*public void OnLook(InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
    }*/

        public void OnMovement(InputAction.CallbackContext context)
        {
            moveComposite = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            OnJumpPerformed?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            isSprinting = !context.canceled;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            crouchCancelled = context.canceled;
            
            if(!context.performed) return;
            
            OnCrouchPerformed?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            
            OnAttackPerformed?.Invoke();
        }
    }
}