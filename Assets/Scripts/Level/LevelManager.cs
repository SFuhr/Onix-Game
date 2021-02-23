using System;
using Grid;
using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Grid & Mover")] [SerializeField]
        private GridMesh grid;

        [SerializeField] private Mover mover;

        public static Action LevelLoaded;
        private static void OnLevelLoaded() => LevelLoaded?.Invoke();
        
        public static Action LevelStarted;
        public static void OnLevelStart() => LevelStarted?.Invoke();
        public static Action<bool> LevelEnded;
        public static void OnLevelEnd(bool success) => LevelEnded?.Invoke(success);

        private void Start()
        {
            InitializeLevel();

            SetEvents();
            
            OnLevelLoaded();
        }

        private void InitializeLevel()
        {
            if (grid == null || mover == null) Debug.LogError("No Grid or Mover available");

            mover.SetGrid(grid);
            grid.Initialize();
        }

        private void SetEvents()
        {
            LevelStarted += () =>
            {
                // Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;

                if(grid.IsMessy) grid.ResetGrid();
                mover.Activate();
            };
            LevelEnded += b => mover.Stop();
        }
    }
}