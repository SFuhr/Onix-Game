using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Platform
{
    [AddComponentMenu("Platform/Platform Brain")]
    public class PlatformBrain : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float xBorders = 10;

        [Header("Elements")]
        [SerializeField] private PlatformElement elementPrefab;

        private bool _isRunning;
        private float _xPosition;
        private List<PlatformElement> _elements;
        
        private void Start()
        {
            _elements = new List<PlatformElement>();
            PlayerController.OnSetPlatform(this);
            CreateElement(transform.position,1f);
        }
        
        private void Update()
        {
            if(!_isRunning) return;
            
            var position = transform.position;
            var move = Time.deltaTime * speed;
            
            transform.position = new Vector3(_xPosition,position.y - move,position.z);

            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Tick();
            }
        }
        
        public void Run()
        {
            _isRunning = true;
        }
        
        public void SetMouseXAxis(float axis)
        {
            _xPosition = Mathf.Clamp(_xPosition + axis, -xBorders, xBorders);
        }

        private void CreateElement(Vector2 spawnPoint, float xSize)
        {
            var element = Instantiate(elementPrefab, spawnPoint, Quaternion.identity,transform);
            element.Init(xSize);
            _elements.Add(element);
        }

        private void DestroyAllElements()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                Destroy(_elements[i].gameObject);
            }
            _elements.Clear();
        }
    }
}