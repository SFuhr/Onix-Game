using System;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelCurrent;
        [SerializeField] private TextMeshProUGUI levelNext;
        [SerializeField] private Image slider;
        [SerializeField] [Range(1, 1000)] private float accuracy = 100;

        public static ProgressBar Instance;

        private int _fillValue;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void SetCurrentLevel(int level)
        {
            levelCurrent.text = level.ToString();
            levelNext.text = (level + 1).ToString();
        }

        public void SetFill(float current, float max)
        {
            var newFill = (int)Mathf.Round((current / max) * accuracy);
            if (_fillValue != newFill)
            {
                _fillValue = newFill;
                slider.fillAmount = _fillValue / accuracy;
            }
            
        }
    }
}