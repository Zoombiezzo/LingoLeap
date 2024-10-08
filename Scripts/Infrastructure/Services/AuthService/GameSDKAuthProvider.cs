using System;
using System.Threading.Tasks;
using GameSDK.Authentication;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    public class GameSDKAuthProvider : IAuthProvider
    {
        public SignInType SignInType => Auth.SignInType switch 
        {
            GameSDK.Authentication.SignInType.Guest => SignInType.Guest,
            GameSDK.Authentication.SignInType.Account => SignInType.Account,
            _ => SignInType.None
        };

        public GameSDKAuthProvider()
        {
            Auth.OnSignIn += OnSignIn;
        }

        private void OnSignIn(GameSDK.Authentication.SignInType obj)
        {
            OnSignInTypeChanged?.Invoke(SignInType);
        }

        public Task SignInAsGuest()
        {
            Debug.Log($"[AuthService]: Sign in as guest account");
            return Auth.SignInAsGuest();
        }

        public Task SignIn()
        {
            Debug.Log($"[AuthService]: Sign in account");
            return Auth.SignIn();
        }

        public event Action<SignInType> OnSignInTypeChanged;

        public void Dispose()
        {
            Auth.OnSignIn -= OnSignIn;
        }
    }
}