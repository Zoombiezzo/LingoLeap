using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.RequirementService
{
    public interface IRequirementService : IService
    {
        void Register(IRequirable item, IReadOnlyList<IRequirement> requirements);
        void Register(IRequirable item, IRequirement requirement);
        void Unregister(IRequirable item);
        bool Check(IRequirement requirement);
        bool CheckAll(IRequirable requirable);
        void RegisterRequirementSystem<T>(IRequirementSystem requirementSystem) where T : IRequirement;
        void CheckRequirementsByType<T>() where T : IRequirement;
        void EnableSystems();
        void DisableSystems();
    }
}