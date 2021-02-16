using System;
using System.Collections;
using Miscellaneous;
using Player;
using UnityEngine;

namespace Grid
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float xBorder = 10f;
        [SerializeField] private float yBorder = 100f;

        [Header("Mesh Grid")] [SerializeField] private MeshGrid grid;

        private Vector3 Position => transform.position;

        private Vector3 _defaultPosition;
        private bool _isMoving;
        private float _xAxis;

        private void Start()
        {
            _defaultPosition = Position;
            FollowCamera.OnSetFollowTarget(transform);
        }

        IEnumerator PerformMoving()
        {
            while (_isMoving)
            {
                var pos = Position;
                var fall = Time.deltaTime * fallSpeed;
                pos.x = Mathf.Clamp(pos.x + _xAxis, -xBorder, xBorder);
                // pos.y = Mathf.Clamp(pos.y - fall, -yBorder, yBorder);
                pos.y -= fall;

                if (pos.y < -yBorder)
                {
                    pos.y = -yBorder;
                    _isMoving = false;
                }

                transform.position = pos;

                grid.UpdatePixels(pos, xBorder);
                yield return null;
            }
        }

        public void Activate()
        {
            if (!_isMoving)
            {
                _isMoving = true;

                ResetPosition();
                
                StartCoroutine(PerformMoving());
            }
        }

        public void SetXAxis(float axis)
        {
            _xAxis = axis;
        }

        private void ResetPosition()
        {
            transform.position = _defaultPosition;
        }
    }
}