using System;
using Player;
using Grid;
using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Grid & Mover")]
        [SerializeField] private MeshGrid grid;
        [SerializeField] private Mover mover;
        
        public static Action LevelStarted;
        public static void OnLevelStart() => LevelStarted?.Invoke();
        public static Action<bool> LevelEnded;
        public static void OnLevelEnded(bool success) => LevelEnded?.Invoke(success);

        public static Action LevelReset;
        public static void OnLevelReset() => LevelReset?.Invoke();
        
        private void Start()
        {
            InitializeLevel();
            
            InputController.OnLevelLoaded(this);

            LevelStarted += () =>
            {
                // Debug.Log($"Level just started...");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                if(grid.IsMessy) grid.ResetGrid();
                mover.Activate();
            };
            LevelEnded += success =>
            {
                // var win = success ? "Win!" : "Lose :(";
                // Debug.Log($"Level ended as {win}");
                mover.Stop(false);
            };
            LevelReset += () =>
            {
                // Debug.Log("Level Was Reseted! ");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                if(grid.IsMessy) grid.ResetGrid();
                mover.Stop(true);
            };
        }
        
        private void InitializeLevel()
        {
            if (grid == null) return;
            
            grid.Initialize();
        }

        // public void SetHorizontalAxis(float horizontal)
        // {
        //     mover.SetHorizontalPosition(horizontal);
        // }
    }
}