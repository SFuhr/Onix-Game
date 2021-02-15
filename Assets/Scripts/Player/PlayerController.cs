using System;
using Grid;
using Level;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private static Action<LevelManager> _setLevelManager;
        public static void OnSetLevelManager(LevelManager manager) => _setLevelManager?.Invoke(manager);

        [SerializeField] [Range(0,2)] private float mouseSpeedMultiplier = 0.5f;

        private LevelManager _levelManager;
        
        private void Awake()
        {
            _setLevelManager += manager => { _levelManager = manager;};
            
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
            
            if (Input.GetMouseButtonUp(0))
            {
                _levelManager.ActivateMover();
            }
            var axis = Input.GetAxis("Mouse X") * mouseSpeedMultiplier;
            _levelManager.SetMouseXAxis(axis);
        }
    }
}
