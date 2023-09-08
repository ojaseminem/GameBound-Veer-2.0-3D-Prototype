using UnityEngine;

namespace Player
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State _currentState;

        public void SwitchState(State state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        private void Update()
        {
            _currentState?.Tick();
        }
    }

    public abstract class State
    {
        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }
}