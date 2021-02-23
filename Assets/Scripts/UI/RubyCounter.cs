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
        private int _saved;

        private const int LowestValue = -1;
        private void Awake()
        {
            _count = LowestValue;
            
            LevelManager.LevelLoaded += WipeCounter;
            LevelManager.LevelStarted += WipeCounter;
            
            Ruby.RubyAcquired += UpdateCounter;
            Ruby.ResetRubies += ResetCounter;
        }

        private void WipeCounter()
        {
            _count = LowestValue;
            _saved = LowestValue;
            UpdateCounter();
        }
        private void ResetCounter()
        {
            _count = _saved;
            _saved = LowestValue;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            // _count = _count < elements.Length ? _count + 1 : elements.Length;
            if (_count < elements.Length)
            {
                _count++;
            }
            else
            {
                _saved++;
            }

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