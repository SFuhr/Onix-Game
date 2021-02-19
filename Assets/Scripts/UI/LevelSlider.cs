using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelSlider : MonoBehaviour
    {
        [SerializeField] private TextMeshPro levelCurrent;
        [SerializeField] private TextMeshPro levelNext;
        [SerializeField] private Image slider;
        [SerializeField] [Range(1, 1000)] private float accuracy = 100;

        public static LevelSlider Instance;

        private int _fillValue;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else Destroy(gameObject);
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