using System;
using Grid;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        private static Action _levelFailed;
        private static void OnLevelFailed() => _levelFailed?.Invoke();

        private static Action _levelCompleted;
        private static void OnLevelCompleted() => _levelCompleted?.Invoke();
        
        [Header("Grid")]
        [SerializeField] private MeshGrid mainGrid;
        [SerializeField] private bool randomEmptyColor = true;
        [SerializeField] private Color gridColorEmpty = new Color(1, 1, 1, 1);
        [SerializeField] private Color gridColorFilled = new Color(0, 0, 0, 1);
        [SerializeField] [Range(0, 1)] private float emptyColorRange = 0.85f;

        [Header("Mover")]
        [SerializeField] private Mover mover;

        private void Start()
        {
            PlayerController.OnSetLevelManager(this);
            InitializeLevel();
        }

        private void OnEnable()
        {
            _levelFailed += _levelFailed;
            _levelCompleted += _levelCompleted;
        }
        private void OnDisable()
        {
            _levelFailed -= _levelFailed;
            _levelCompleted -= _levelCompleted;
        }

        private void InitializeLevel()
        {
            if (mainGrid == null) return;

            var emptyColor = randomEmptyColor ? Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) : gridColorEmpty;
            mainGrid.SetColors(emptyColor,gridColorFilled,emptyColorRange);
            mainGrid.FullReset();
        }

        private void LevelFailed()
        {
            
        }

        private void LevelCompleted()
        {
            
        }

        public void ActivateMover() => mover.Activate();
        public void SetMouseXAxis(float xAxis) => mover.SetXAxis(xAxis);
    }
}