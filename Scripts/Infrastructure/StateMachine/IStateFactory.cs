using _Client.Scripts.Infrastructure.StateMachine.States;

namespace _Client.Scripts.Infrastructure.StateMachine
{
    public interface IStateFactory
    {
        TState Create<TState>() where TState : class, IExitableState;
    }
}