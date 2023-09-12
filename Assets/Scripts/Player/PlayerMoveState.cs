using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerBaseState
    {
        private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private readonly int _moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
        private const float AnimationDampTime = 0.1f;
        private const float CrossFadeDuration = 0.1f;

        public PlayerMoveState(PlayerStateMachine player) : base(player) { }

        public override void Enter()
        {
            player.velocity.y = Physics.gravity.y;

            player.animator.CrossFadeInFixedTime(_moveBlendTreeHash, CrossFadeDuration);

            player.inputReader.OnJumpPerformed += SwitchToJumpState;
            player.inputReader.OnCrouchPerformed += SwitchToCrouchState;
            player.inputReader.OnAttackPerformed += SwitchToAttackState;
        }

        public override void Tick()
        {
            ApplyGravity();
            
            var t = player.transform.position;
            t.x = Mathf.Lerp(t.x, 0, 1f);
            player.transform.position = t;
            
            CalculateMoveDirection();
            FaceMoveDirection();
            
            var inputValue = player.inputReader.moveComposite.sqrMagnitude;
            var maxRange = .5f;
            
            switch (player.inputReader.isSprinting)
            {
                case true:
                    maxRange = 1f;
                    Move(player.sprintMultiplier);
                    break;
                default:
                    Move();
                    break;
            }

            var blendValue = Mathf.Clamp01(inputValue) * maxRange;

            player.animator.SetFloat(_moveSpeedHash, blendValue, AnimationDampTime, Time.deltaTime);
        }

        public override void Exit()
        {
            player.inputReader.OnJumpPerformed -= SwitchToJumpState;
            player.inputReader.OnCrouchPerformed -= SwitchToCrouchState;
        }

        private void SwitchToJumpState() => player.SwitchState(new PlayerJumpState(player));
        private void SwitchToCrouchState() => player.SwitchState(new PlayerCrouchState(player));
        private void SwitchToAttackState() => player.SwitchState(new PlayerAttackState(player));
    }
}