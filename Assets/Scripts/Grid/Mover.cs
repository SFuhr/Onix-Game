using System.Collections;
using Level;
using Miscellaneous;
using UI;
using UnityEngine;

namespace Grid
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float yBorder = 100f;

        [Header("Mesh Grid")]
        [SerializeField] private MeshGrid grid;
        
        private Vector3 _defaultPosition;
        private bool _isMoving;
        private int _horizontalPosition;
        
        private Vector3 Position => transform.position;

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
                pos.y -= fall;

                if (pos.y < -yBorder)
                {
                    pos.y = -yBorder;
                    _isMoving = false;
                }

                transform.position = pos;
                
                // updating grid pixels
                grid.UpdatePixels(pos.y, _horizontalPosition);
                
                // Updating ui
                LevelSlider.Instance.SetFill(Mathf.Abs(pos.y),yBorder);
                yield return null;
            }
        }

        public void Activate()
        {
            if (!_isMoving)
            {
                // begin level
                
                _isMoving = true;

                ResetPosition();
                
                StartCoroutine(PerformMoving());
            }
        }

        public void SetHorizontalPosition(float horPos)
        {
            _horizontalPosition = (int)Mathf.Round(horPos);
        }

        private void ResetPosition()
        {
            transform.position = _defaultPosition;
        }
    }
}