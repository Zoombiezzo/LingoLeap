using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services
{
    public class Debugger
    {
        private static Debugger Instance => _instance ??= new Debugger();

        private static Debugger _instance;
        private bool _isEnable = false;

        public static void Initialize(bool enable)
        {
            Instance._isEnable = enable;
        }

        public static void Log(object message, Object context )
        {
            if (Instance._isEnable == false) return; 
            
            Debug.Log(message, context); 
        }
        
        public static void Log(object message)
        {
            if (Instance._isEnable == false) return; 
            
            Debug.Log(message); 
        } 
        
        public static void LogError(object message)
        {
            if (Instance._isEnable == false) return; 
            
            Debug.LogError(message); 
        }
    }
}