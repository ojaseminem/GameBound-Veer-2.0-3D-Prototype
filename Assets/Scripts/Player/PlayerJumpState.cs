using UnityEngine;

namespace Player
{
    public class PlayerJumpState : PlayerBaseState
    {
        private readonly int _jumpBlendHash = Animator.StringToHash("JumpBlend");
        private readonly int _jumpBlendTreeHash = Animator.StringToHash("JumpBlendTree");
        private const float AnimationDampTime = 0.1f;
        private const float CrossFadeDuration = 0.1f;

        public PlayerJumpState(PlayerStateMachine player) : base(player) { }

        public override void Enter()
        {
            var force = player.jumpForce;
            if (player.inputReader.isSprinting) force *= player.sprintJumpMultiplier;
            player.velocity = new Vector3(player.velocity.x, force, player.velocity.z);

            player.animator.CrossFadeInFixedTime(_jumpBlendTreeHash, CrossFadeDuration);
            
            player.inputReader.OnAttackPerformed += SwitchToAttackState;
        }

        public override void Tick()
        {
            ApplyGravity();

            if (player.velocity.y <= 0f && player.controller.isGrounded)
            {
                player.SwitchState(new PlayerMoveState(player));
            }

            FaceMoveDirection();

            var sprint = player.inputReader.isSprinting;
            
            Move(sprint ? player.sprintMultiplier : 1f);

            player.animator.SetFloat(_jumpBlendHash, sprint ? 1f : 0f, AnimationDampTime, Time.deltaTime);
        }

        public override void Exit()
        {
            player.inputReader.OnAttackPerformed -= SwitchToAttackState;
        }
        
        private void SwitchToAttackState() => player.SwitchState(new PlayerAttackState(player));

    }
}