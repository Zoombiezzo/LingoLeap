namespace _Client.Scripts.Infrastructure.Services.RequirementService
{
    public interface IRequirable
    {
        bool RequirementEnabled { get; }
        void RequirementEnable();
        void RequirementDisable();
    }
}