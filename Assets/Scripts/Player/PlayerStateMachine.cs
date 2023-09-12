using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(InputReader))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerStateMachine : StateMachine
    {
        public Vector3 velocity;
        public GameObject swordEquipped, swordSheathed;

        public CharacterColliderSettings normalSettings, crouchedSettings;
        
        public float movementSpeed { get; private set; } = 20f;
        public float sprintMultiplier { get; private set; } = 2.5f;
        public float jumpForce { get; private set; } = 15f;
        public float sprintJumpMultiplier { get; private set; } = 1f;
        public float gravityMultiplier { get; private set; } = 5f;
        public float lookRotationDampFactor { get; private set; } = 10f;
        //public Transform mainCamera { get; private set; }
        public InputReader inputReader { get; private set; }
        public Animator animator { get; private set; }
        public CharacterController controller { get; private set; }

        private void Start()
        {
            //if (Camera.main != null) mainCamera = Camera.main.transform;

            inputReader = GetComponent<InputReader>();
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();

            SwitchState(new PlayerMoveState(this));
        }
    }

    [Serializable]
    public struct CharacterColliderSettings
    {
        public float height;
        public Vector3 center;
    }
}