using System;
using Player;
using Grid;
using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Grid & Mover")] [SerializeField]
        private GridMesh grid;

        [SerializeField] private Mover mover;

        public static Action LevelStart;
        public static void OnLevelStart() => LevelStart?.Invoke();
        public static Action<bool> LevelEnd;
        public static void OnLevelEnd(bool success) => LevelEnd?.Invoke(success);

        public static Action LevelReset;
        public static void OnLevelReset() => LevelReset?.Invoke();

        private void Start()
        {
            InitializeLevel();

            SetEvents();
        }

        #region TEST

#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                OnLevelEnd(false);
                OnLevelReset();
            }
        }

#endif

        #endregion

        private void InitializeLevel()
        {
            if (grid == null || mover == null) Debug.LogError("No Grid or Mover available");

            mover.SetGrid(grid);
            grid.Initialize();
        }

        private void SetEvents()
        {
            LevelStart += () =>
            {
                // Debug.Log($"Level just started...");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (grid.IsMessy) grid.ResetGrid();
                mover.Activate();
            };
            LevelEnd += success =>
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

                if (grid.IsMessy) grid.ResetGrid();
                mover.Stop(true);
            };
        }
    }
}