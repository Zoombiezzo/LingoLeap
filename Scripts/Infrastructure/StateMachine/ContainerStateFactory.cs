using _Client.Scripts.Infrastructure.StateMachine.States;
using VContainer;

namespace _Client.Scripts.Infrastructure.StateMachine
{
    public class ContainerStateFactory : IStateFactory
    {
        private readonly IObjectResolver _container;
        
        public ContainerStateFactory(IObjectResolver container)
        {
            _container = container;
        }
        
        public TState Create<TState>() where TState : class, IExitableState => _container.Resolve<TState>();
    }
}