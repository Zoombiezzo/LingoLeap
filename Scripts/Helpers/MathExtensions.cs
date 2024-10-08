using UnityEngine;

namespace _Client.Scripts.Helpers
{
    public static class MathExtensions
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
    
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        public static T[] Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }    
            return array;
        }
    }
    
    public static class VectorExtensions
    {
        public static Vector2 Remap(this Vector2 value, Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static Vector3 ToRectPosition(this Vector3 value, Camera camera, RectTransform screen)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, value);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, screenPos, null, out Vector2 rectPos);
            return rectPos;
        }
        
        public static Vector3 ToWorldPosition(this Vector3 value, Camera camera, RectTransform screen)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(screen, value, camera, out var worldPos);
            return worldPos;
        }
        
        public static Vector2 Abs(this Vector2 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}