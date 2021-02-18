using System;
using Level;
using UnityEngine;

namespace GameController
{
    public class InputController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] [Range(1,10)] private int mouseSpeed = 4;

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
            
            var horizontal = Input.GetAxisRaw("Mouse X") * mouseSpeed;
            _level.SetHorizontalAxis(horizontal);
        }

        public void RunLevel()
        {
            if (!LevelExists) return;
            
            _level.RunMover();
        }
    }
}