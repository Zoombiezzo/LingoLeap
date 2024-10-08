#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Helpers
{
    public static class AssetHelper<T> where T : UnityEngine.Object
    {
        public static IEnumerable<T> GetAsset(string path = "Assets")
        {
            string[] objectsGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            T[] objects = new T[objectsGuids.Length];

            for (int i = 0; i < objectsGuids.Length; i++)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(objectsGuids[i]);
                var subObjects = AssetDatabase.LoadAllAssetsAtPath(objectPath);
                var subObjects2 = AssetDatabase.LoadAllAssetRepresentationsAtPath(objectPath);
                objects[i] = AssetDatabase.LoadAssetAtPath<T>(objectPath);
            }

            return objects;
        }

        public static IEnumerable<T> LoadAllAssetRepresentationsAtPath(string path = "")
        {
            string[] objectsGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            List<T> objects = new List<T>(objectsGuids.Length);

            for (int i = 0; i < objectsGuids.Length; i++)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(objectsGuids[i]);
                var subObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(objectPath);
                foreach (var subObject in subObjects)
                {
                    if (subObject is T obj)
                    {
                        objects.Add(obj);
                    }
                }
            }

            return objects;
        }

    public static IEnumerable<T> GetAssetsContains<TC>(string path = "") where TC : Component
        {
            string[] objectsGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            List<T> objects = new List<T>(objectsGuids.Length);

            for (int i = 0; i < objectsGuids.Length; i++)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(objectsGuids[i]);
                var targetObject = AssetDatabase.LoadAssetAtPath<T>(objectPath);

                var gameObject = targetObject as GameObject;
                if(gameObject == null) continue;
                if(gameObject.GetComponent<TC>() == null) continue;

                objects.Add(AssetDatabase.LoadAssetAtPath<T>(objectPath));
            }

            return objects;
        }
    }
}
#endif