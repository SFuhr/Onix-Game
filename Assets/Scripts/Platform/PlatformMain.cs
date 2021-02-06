using System;
using System.Collections.Generic;
using Interactable;
using Player;
using UnityEngine;

namespace Platform
{
    public class PlatformMain : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float xBorder = 5f;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lowestPoint = -95f;
        [Header("Elements")]
        [SerializeField] private Element elementPrefab;
        [SerializeField] private LayerMask scanLayer;
        [SerializeField] [Range(0,1)] private float scanTolerance = 0.9f;
        [SerializeField] private float growthSpeed = 2f;

        public static float XBorderSize { get; private set; }
        
        private bool _isRunning;
        private float _xAxis;
        private List<Element> _elements;
        private List<Element> _newElements;
        private Collider2D[] _scanResults;
        
        private void Start()
        {
            XBorderSize = xBorder;
            _scanResults = new Collider2D[10];
            var element = Instantiate(elementPrefab, transform.position, Quaternion.identity,transform);
            element.SetOwnerPlatform(this);
            element.AdjustElement(1,growthSpeed);
            _elements = new List<Element>();
            _newElements = new List<Element>();
            _elements.Add(element);
            
            PlayerController.OnSetPlatform(this);
        }

        public void Run(bool run)
        {
            _isRunning = run;
        }

        private void Update()
        {
            if(!_isRunning) return;
            
            var pos = transform.position;
            pos.x = Mathf.Clamp(pos.x + _xAxis, -xBorder,xBorder);
            pos.y -= Time.deltaTime * speed;

            transform.position = pos;
            
            UpdateElements();

            if (pos.y <= lowestPoint)
            {
                Run(false);
            }
        }

        private void UpdateElements()
        {
            var size = _elements.Count;
            for (int i = 0; i < size; i++)
            {
                var elem = _elements[i];
                elem.Tick();
                var scan = Physics2D.OverlapBoxNonAlloc(elem.GetMiddlePoint,elem.GetSize * scanTolerance,0,_scanResults,scanLayer);
                if (scan <= 0) continue;
                
                for (int j = 0; j < scan; j++)
                {
                    var col = _scanResults[j];
                    if (!col.isTrigger)
                    {
                        // separate element here
                        var elemBounds = elem.GetBounds();
                        var colBounds = col.bounds;
                        
                        if (elemBounds.min.x < colBounds.min.x)
                        {
                            // create left part
                            var midPoint = new Vector2((elemBounds.min.x + colBounds.min.x) * 0.5f,(elemBounds.min.y + elemBounds.max.y) * 0.5f);
                            var left = Instantiate(elementPrefab, midPoint, Quaternion.identity, transform);
                            left.AdjustElement(Mathf.Abs(elemBounds.min.x - colBounds.min.x),growthSpeed);
                            _newElements.Add(left);
                        }

                        // if (elemBounds.max.x > colBounds.max.x)
                        // {
                        //     // create right part
                        //     var midPoint = new Vector2((colBounds.max.x + elemBounds.max.x) * 0.5f,(colBounds.max.y + elemBounds.max.y) * 0.5f);
                        //     var right = Instantiate(elementPrefab, midPoint, Quaternion.identity, transform);
                        //     right.AdjustElement(Mathf.Abs(elemBounds.max.x - colBounds.max.x),growthSpeed);
                        //     _newElements.Add(right);
                        // }
                        _elements.Remove(elem);
                        Destroy(elem.gameObject);
                        break;
                    }
                    if(col.TryGetComponent(out IInteractable interactable))
                    {
                        // collect bonuses coins etc
                    }
                }
            }

            if (_newElements.Count > 0)
            {
                _elements.AddRange(_newElements);
                _newElements.Clear();
            }
        }

        public void SetXAxis(float x)
        {
            _xAxis = x;
        }
    }
}