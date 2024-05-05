
namespace AISystem
{
    public class StateMachine<T> where T : AIState
    {
        public T CurrentState { get; set; }

        public void Initialize(T startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(T newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            newState.Enter();
        }
    }
}