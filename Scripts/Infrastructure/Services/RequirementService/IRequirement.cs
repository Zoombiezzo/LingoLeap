namespace _Client.Scripts.Infrastructure.Services.RequirementService
{
    public interface IRequirement
    {
#if UNITY_EDITOR
        public string RequirementName { get; }
#endif
    }

}