using System;
using Player;
using UnityEngine;

namespace Grid
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float xBorder = 10f;
        [SerializeField] private float yBorder = 100f;
        
        [Header("Mesh Grid")]
        [SerializeField] private MeshGrid grid;

        private Vector3 Position => transform.position;
        
        private bool _isMoving;
        private float _xAxis;
        private void Start()
        {
            PlayerController.OnSetMover(this);
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
 
            GUIStyle style = new GUIStyle();
 
            Rect rect = new Rect(0, 0, w, h * 4 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color (1f, 0f, 0f, 1f);
            
            string text = $"Mover Position\nX: {Position.x.ToString("G")} Y: {Position.y.ToString("G")}";
            GUI.Label(rect, text, style);
        }

        public void Update()
        {
            if (!_isMoving) return;
            
            var pos = Position;
            var fall = Time.deltaTime * fallSpeed;
            pos.x = Mathf.Clamp(pos.x + _xAxis, -xBorder,xBorder);
            pos.y = Mathf.Clamp(pos.y - fall,-yBorder,yBorder);

            transform.position = pos;

            grid.UpdatePixels(pos,xBorder);
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