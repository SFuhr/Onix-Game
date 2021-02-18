using System;
using GameController;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private MeshGrid mainGrid;
        [SerializeField] private bool randomEmptyColor = true;
        [SerializeField] private Color gridColorEmpty = new Color(1, 1, 1, 1);
        [SerializeField] private Color gridColorFilled = new Color(0, 0, 0, 1);
        [SerializeField] [Range(0, 1)] private float emptyColorRange = 0.85f;

        [Header("Mover")]
        [SerializeField] private Mover mover;
        
        public static Action LevelStarted;
        private static void OnLevelStarted() => LevelStarted?.Invoke();
        public static Action<bool> LevelEnded;
        public static void OnLevelEnded(bool success) => LevelEnded?.Invoke(success);
        
        private void Start()
        {
            InitializeLevel();
            
            InputController.OnLevelLoaded(this);

            // LevelStarted += () => Debug.Log("Callback: Level has started");
            // LevelFailed += () => Debug.Log("Callback: Level was failed!");
            // LevelCompleted += () => Debug.Log("Callback: Level was successfully completed!");
            
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
        
        private void InitializeLevel()
        {
            if (mainGrid == null) return;

            var emptyColor = randomEmptyColor ? Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) : gridColorEmpty;
            mainGrid.SetColors(emptyColor,gridColorFilled,emptyColorRange);
            mainGrid.FullReset();
        }

        public void RunMover()
        {
            mover.Activate(); 
            
            // triggering the level started event
            OnLevelStarted();
        }

        public void SetHorizontalAxis(float horizontal)
        {
            mover.SetHorizontalPosition(horizontal);
        }
    }
}