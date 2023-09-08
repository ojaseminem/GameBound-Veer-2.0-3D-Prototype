using UnityEngine;

namespace Player
{
    public abstract class PlayerBaseState : State
    {
        protected readonly PlayerStateMachine player;

        protected PlayerBaseState(PlayerStateMachine player)
        {
            this.player = player;
        }

        protected void CalculateMoveDirection()
        {
            //Vector3 cameraForward = new(player.mainCamera.forward.x, 0, player.mainCamera.forward.z);
            //Vector3 cameraRight = new(player.mainCamera.right.x, 0, player.mainCamera.right.z);

           // var moveDirection = player.transform.position.normalized * player.inputReader.moveComposite.y +
                               // player.transform.position.normalized * player.inputReader.moveComposite.x;

            //player.velocity.x = moveDirection.x * player.movementSpeed;
            player.velocity.z = player.inputReader.moveComposite.x * player.movementSpeed;
        }

        protected void FaceMoveDirection()
        {
            Vector3 faceDirection = new(player.velocity.x, 0f, player.velocity.z);

            if (faceDirection == Vector3.zero)
                return;

            player.transform.rotation = Quaternion.Slerp(player.transform.rotation,
                Quaternion.LookRotation(faceDirection), player.lookRotationDampFactor * Time.deltaTime);
        }

        protected void ApplyGravity()
        {
            if (player.velocity.y > Physics.gravity.y)
            {
                player.velocity.y += Physics.gravity.y * Time.deltaTime * player.gravityMultiplier;
            }
        }

        protected void Move(float sprintMultiplier = 1f)
        {
            player.controller.Move(player.velocity * (sprintMultiplier * Time.deltaTime));
        }
    }
}