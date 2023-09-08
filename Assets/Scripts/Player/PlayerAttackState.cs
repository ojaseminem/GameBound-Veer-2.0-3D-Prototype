using UnityEngine;

namespace Player
{
    public class PlayerAttackState : PlayerBaseState
    {
        private readonly int _attackBlendHash = Animator.StringToHash("attackType");
        private readonly int _attackBlendTreeHash = Animator.StringToHash("attackBlendTree");
        private const float AnimationDampTime = 0.1f;
        private const float CrossFadeDuration = 0.1f;

        public PlayerAttackState(PlayerStateMachine player) : base(player) { }

        public override void Enter()
        {
            player.velocity.y = 0f;

            player.animator.CrossFadeInFixedTime(_attackBlendTreeHash, CrossFadeDuration);
        }

        public override void Tick()
        {
            ApplyGravity();

            var jumpAttack = !player.controller.isGrounded;

            player.animator.SetFloat(_attackBlendHash, jumpAttack ? 1f : 0f, AnimationDampTime, Time.deltaTime);

            if (player.inputReader.crouchCancelled)
            {
                player.SwitchState(new PlayerMoveState(player));
            }
        }

        public override void Exit() { }
    }
}