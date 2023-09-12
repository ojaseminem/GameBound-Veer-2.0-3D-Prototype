using UnityEngine;

namespace Player
{
    public class PlayerAttackState : PlayerBaseState
    {
        private readonly int _attackBlendHash = Animator.StringToHash("AttackType");
        private readonly int _attackBlendTreeHash = Animator.StringToHash("AttackBlendTree");
        private const float AnimationDampTime = 0.1f;
        private const float CrossFadeDuration = 0.1f;

        public PlayerAttackState(PlayerStateMachine player) : base(player) { }

        public override void Enter()
        {
            ToggleSword(true);
            
            var jumpAttack = !player.controller.isGrounded;

            player.animator.CrossFadeInFixedTime(_attackBlendTreeHash, CrossFadeDuration);
            
            player.animator.SetFloat(_attackBlendHash, jumpAttack ? 1f : 0f, AnimationDampTime, Time.deltaTime);
        }

        public override void Tick()
        {
            ApplyGravity();

            if (!(player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .99f)) return;
            ToggleSword(false);
            player.inputReader.isAttacking = false;
            player.SwitchState(new PlayerMoveState(player));
        }

        public override void Exit() { }
    }
}