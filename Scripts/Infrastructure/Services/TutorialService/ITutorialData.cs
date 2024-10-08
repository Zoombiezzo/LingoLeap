namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public interface ITutorialData
    {
        string Id { get; }
        bool IsCompleted { get; }
        void CompleteStep(string id);
        void CompleteTutorial();
        bool IsStepCompleted(string id);
        void Reset();
    }
}