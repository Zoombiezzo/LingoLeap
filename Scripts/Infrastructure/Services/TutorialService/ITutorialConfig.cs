using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public interface ITutorialConfig
    {
        string Id { get; }
        string TutorialExecutorId { get; }
        IReadOnlyList<ITutorialStep> Steps { get; }
    }
}