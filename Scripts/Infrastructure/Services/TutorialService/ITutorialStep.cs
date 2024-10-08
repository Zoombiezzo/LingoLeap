using System;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public interface ITutorialStep : IDisposable
    {
       string Id { get; }
       bool IsComplete { get; }
       bool IsPossibleStart { get; }
       Task Initialize(IInitializeData data = null); 
       Task Create();
       Task Show();
       Task Hide();
       void Delete();
    }
}