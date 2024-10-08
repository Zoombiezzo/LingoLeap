using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Helpers
{
    public static class ConfigsHelper<T> where T : ScriptableObject
    {
        public static IEnumerable<T> GetConfigs()
        {
            return Resources.FindObjectsOfTypeAll<T>();
        }
    }
}