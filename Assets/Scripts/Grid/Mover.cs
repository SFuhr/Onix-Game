using System.Collections;
using Level;
using Miscellaneous;
using UI;
using UnityEngine;

namespace Grid
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float mouseSpeed = 100f;
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float yBorder = 100f;

        private GridMesh _grid;
        private Vector3 _defaultPosition;
        private bool _isMoving;
        private float _xAxis;

        private Vector3 Position => transform.position;

        private void Start()
        {
            _defaultPosition = Position;
            
            var cam = FindObjectOfType<FollowCamera>();
            cam.SetTarget(transform);
        }

        public void SetGrid(GridMesh grid) => _grid = grid;

        IEnumerator PerformMoving()
        {
            _grid.BeginMessing();

            while (_isMoving)
            {
                var pos = Position;
                var fall = Time.deltaTime * fallSpeed;
                pos.y -= fall;

                _xAxis = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
                
                if (pos.y < -yBorder)
                {
                    pos.y = -yBorder;
                    _isMoving = false;
                }

                transform.position = pos;

                // updating grid pixels
                _grid.UpdatePixels(_xAxis, pos.y);

                ProgressBar.Instance.SetFill(Mathf.Abs(pos.y), yBorder);
                // Updating ui
                yield return null;
            }
            LevelManager.OnLevelEnd(true);
            
            // resetting progress slider
            ProgressBar.Instance.SetFill(0, 1);
        }

        public void Stop()
        {
            if (!_isMoving) return;
            _isMoving = false;
        }
        
        public void Activate()
        {
            Reset();
            if (!_isMoving)
            {
                // begin level

                _isMoving = true;
                
                StopAllCoroutines();
                StartCoroutine(PerformMoving());
            }
        }

        public void Reset()
        {
            if (_isMoving)
            {
                StopAllCoroutines();
                _isMoving = false;
            }
            transform.position = _defaultPosition;
        }
    }
}