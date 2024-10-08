namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadState<TPayload> : IState
    {
        void Enter(TPayload payload);
    }
    
    public interface IExitableState
    {
        void Exit();
    }
}