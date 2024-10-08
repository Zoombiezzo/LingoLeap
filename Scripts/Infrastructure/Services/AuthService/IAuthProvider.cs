using System;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    public interface IAuthProvider : IDisposable
    {
        SignInType SignInType { get; }
        Task SignInAsGuest();
        Task SignIn();
        event Action<SignInType> OnSignInTypeChanged;
    }
}