using System;
using _Client.Scripts.Infrastructure.Services;
using _Client.Scripts.Infrastructure.StateMachine.States;

namespace _Client.Scripts.Infrastructure.StateMachine
{
    public interface IGameStateMachine : IService
    {
        event Action<IState> OnStateEnter;
        event Action<IState> OnStateExit;
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
        bool IsState<TState>() where TState : class, IState;
    }
}