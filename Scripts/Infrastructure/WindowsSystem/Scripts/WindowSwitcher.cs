using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Scripts
{
    public class WindowSwitcher : MonoBehaviour
    {
        [SerializeField] private string _windowId;

        public void Show()
        {
            if (WindowsService.IsOpen(_windowId))
            {
                Debug.Log("Window already opened!");
            }
            else
            {
                Debug.Log("Window first opened!");
            }
            
            WindowsService.Show(_windowId);
        }

        public void Hide()
        {
            WindowsService.Hide(_windowId);
        }
    }
}
