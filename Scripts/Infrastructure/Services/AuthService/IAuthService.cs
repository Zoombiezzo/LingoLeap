using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    public interface IAuthService : IService, IStorable
    {
        SignInType SignInType { get; }
        bool IsCanceled { get; }
        Task SignInAsGuest();
        Task SignIn();
        event Action<SignInType> OnSignInTypeChanged;
        void CancelSignIn(bool isCanceled);
    }
}