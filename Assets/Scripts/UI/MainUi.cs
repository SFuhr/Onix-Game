using System;
using UnityEngine;

namespace UI
{
    public class MainUi : MonoBehaviour
    {
        private static Action<bool> _showBlackScreen;
        private static void OnShowBlackScreen(bool show) => _showBlackScreen?.Invoke(show);
        
        private Animator _animator;
        private static readonly int BlackScreen = Animator.StringToHash("Black Screen");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _showBlackScreen += show => _animator.SetBool(BlackScreen, show);
        }
    }
}