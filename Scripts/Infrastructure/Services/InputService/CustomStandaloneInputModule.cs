using UnityEngine.EventSystems;

namespace _Client.Scripts.Infrastructure.Services.InputService
{
    public class CustomStandaloneInputModule : StandaloneInputModule
    {
        public new PointerEventData GetLastPointerEventData(int id)
        {
            return base.GetLastPointerEventData(id);
        }
    }
}