namespace _Client.Scripts.Infrastructure.Services.RequirementService
{
    public interface IRequirementSystem
    {
        void Enable();
        void Disable();
        bool Check(IRequirement requirement);
    }
}