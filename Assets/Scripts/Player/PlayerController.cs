using System;
using Platform;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private static Action<PlatformMain> _setPlatform;
        public static void OnSetPlatform(PlatformMain platform) => _setPlatform?.Invoke(platform);
        
        private PlatformMain _platform;

        private bool PlatformExists => _platform != null;

        private void Awake()
        {
            _setPlatform += platform => _platform = platform;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                // exit game
                Application.Quit();
                
                // #if UNITY_EDITOR
                // EditorApplication.ExitPlaymode();
                // #endif
            }
            
            if (!PlatformExists) return;

            if (Input.GetMouseButtonUp(0))
            {
                _platform.Run(true);
            }
            var axis = Input.GetAxis("Mouse X");
            _platform.SetXAxis(axis);
        }
    }
}
