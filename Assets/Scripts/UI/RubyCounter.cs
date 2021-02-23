using System;
using Items;
using Level;
using UnityEngine;

namespace UI
{
    public class RubyCounter : MonoBehaviour
    {
        [SerializeField] private RectTransform[] elements;

        private int _count;
        
        private void Awake()
        {
            LevelManager.LevelLoaded += ResetCounter;
            LevelManager.LevelStarted += ResetCounter;
            
            Ruby.RubyAcquired += UpdateCounter;
            Ruby.ResetRubies += ResetCounter;
        }

        private void ResetCounter()
        {
            _count = -1;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            _count = _count < elements.Length ? _count + 1 : elements.Length;

            var i = 0;
            for (i = 0; i < _count; i++)
            {
                elements[i].gameObject.SetActive(true);
            }

            for (int j = i; j < elements.Length; j++)
            {
                elements[j].gameObject.SetActive(false);
            }
        }
    }
}