using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.StateMachine.States;

namespace _Client.Scripts.Infrastructure.StateMachine
{
    public class GameStateMachine : IGameStateMachine
    {
        private IState _activeState;
        private readonly IStateFactory _stateFactory;
        private readonly Dictionary<Type, IExitableState> _states = new(16);
        
        public event Action<IState> OnStateEnter; 
        public event Action<IState> OnStateExit; 

        public GameStateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        
        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>();

            state.Enter();

            OnStateEnter?.Invoke(state);
        }
        
        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            TState state = ChangeState<TState>();

            state.Enter(payload);
            
            OnStateEnter?.Invoke(state);
        }

        public bool IsState<TState>() where TState : class, IState => _activeState is TState;

        private TState ChangeState<TState>() where TState : class, IState
        {
            if (_activeState != null)
            {
                _activeState.Exit();
                OnStateExit?.Invoke(_activeState);
            }

            TState state = GetState<TState>();
            _activeState = state;
            
            return state;
        }

        private TState GetState<TState>() where TState : class, IState
        {
            if (_states.TryGetValue(typeof(TState), out IExitableState state) == false)
            {
                state = _stateFactory.Create<TState>();
                _states.Add(typeof(TState), state);
            }
            
            return state as TState;
        }
    }
}