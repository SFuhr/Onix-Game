using System;
using Level;
using UnityEngine;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] [Range(50,500)] private int mouseSpeed = 200;

        private LevelManager _level;
        
        private static Action<LevelManager> _levelLoaded;
        public static void OnLevelLoaded(LevelManager level) => _levelLoaded?.Invoke(level);

        private bool LevelExists => _level != null;
        
        private void Awake()
        {
            _levelLoaded += level => _level = level;
        }
        private void Update()
        {
            if (!LevelExists) return;
            
            // var horizontal = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            // _level.SetHorizontalAxis(horizontal);
            
            // test
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                LevelManager.OnLevelReset();
                LevelManager.OnLevelEnded(false);
            }
        }

        public void RunLevel()
        {
            if (!LevelExists) return;
            
            LevelManager.OnLevelStart();
        }
    }
}