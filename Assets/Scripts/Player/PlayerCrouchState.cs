using UnityEngine;

namespace Player
{
    public class PlayerCrouchState : PlayerBaseState
    {
        private readonly int _crouchBlendHash = Animator.StringToHash("CrouchSpeed");
        private readonly int _crouchBlendTreeHash = Animator.StringToHash("CrouchBlendTree");
        private const float AnimationDampTime = 0.1f;
        private const float CrossFadeDuration = 0.1f;

        public PlayerCrouchState(PlayerStateMachine player) : base(player) { }

        public override void Enter()
        {
            ToggleCrouchCollider(true);
            
            player.velocity.y = 0f;

            player.animator.CrossFadeInFixedTime(_crouchBlendTreeHash, CrossFadeDuration);
        }

        public override void Tick()
        {
            ApplyGravity();
            
            var inputValue = player.inputReader.moveComposite.sqrMagnitude;
            var walking = inputValue >= .15f;

            if(walking)
            {
                CalculateMoveDirection();
                FaceMoveDirection();
                Move();
            }
            
            player.animator.SetFloat(_crouchBlendHash, walking ? 1f : 0f, AnimationDampTime, Time.deltaTime);

            if (player.inputReader.crouchCancelled)
            {
                ToggleCrouchCollider(false);
                player.SwitchState(new PlayerMoveState(player));
            }
        }

        public override void Exit() { }
    }
}