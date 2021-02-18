using System;
using Level;
using UnityEngine;

namespace UI
{
    public class HandPointer : MonoBehaviour
    {
        private Animator _animator;
        private Vector3 _basePos;
        private Transform _cameraTransform;
        private bool _activated;
        
        private static readonly int ShowPointer = Animator.StringToHash("Show Pointer");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _basePos = transform.position;
            _cameraTransform = Camera.main.transform;
            
            LevelManager.LevelStarted += () =>
            {
                _activated = true;
                if(_animator != null) _animator.SetTrigger(ShowPointer);
            };
            LevelManager.LevelEnded += success => _activated = false;
        }

        private void Update()
        {
            if (!_activated) return;
            
            float camY = _cameraTransform.position.y;

            transform.position = _basePos + new Vector3(0,camY,0);
        }
    }
}