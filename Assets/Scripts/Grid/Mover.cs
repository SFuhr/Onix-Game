using System;
using Player;
using UnityEngine;

namespace Grid
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float xBorder = 10f;
        [SerializeField] private float lowestYPoint = -150;
        
        [Header("Mesh Grid")]
        [SerializeField] private MeshGrid grid;

        private bool _isMoving;
        private float _xAxis;
        private void Start()
        {
            PlayerController.OnSetMover(this);
        }

        public void Update()
        {
            if (!_isMoving) return;
            
            var pos = transform.position;
            pos.x = Mathf.Clamp(pos.x + _xAxis, -xBorder,xBorder);
            pos.y -= Time.deltaTime * fallSpeed;

            transform.position = pos;

            grid.UpdatePixels(pos);
        }

        public void Activate()
        {
            _isMoving = true;
        }

        public void SetXAxis(float axis)
        {
            _xAxis = axis;
        }
    }
}