using System;
using Grid;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private static Action<Mover> _setMover;
        public static void OnSetMover(Mover platform) => _setMover?.Invoke(platform);

        [SerializeField] [Range(0,2)] private float mouseSpeed = 0.5f;
        
        private Mover _mover;

        private bool PlatformExists => _mover != null;

        private void Awake()
        {
            _setMover += platform => _mover = platform;

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
                _mover.Activate();
            }
            var axis = Input.GetAxis("Mouse X") * mouseSpeed;
            _mover.SetXAxis(axis);
        }
    }
}
