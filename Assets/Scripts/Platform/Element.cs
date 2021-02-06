using System;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace Platform
{
    public class Element : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private SpriteRenderer spriteLeft;
        [SerializeField] private SpriteRenderer spriteRight;
        
        private float _growth = 0.25f;
        private PlatformMain _owner;
        
        public void SetOwnerPlatform(PlatformMain platform)
        {
            _owner = platform;
        }

        public void AdjustElement(float width,float speed)
        {
            var size = new Vector2(width * 0.5f, spriteLeft.size.y);
            spriteLeft.size = size;
            spriteRight.size = size;
            _growth = speed;
        }

        public void Tick()
        {
            //left sprite
            var bounds = spriteLeft.bounds.min.x;
            if (Mathf.Abs(bounds) < PlatformMain.XBorderSize)
            {
                var size = spriteLeft.size;
                size.x += Time.deltaTime * _growth;
                spriteLeft.size = size;
            }
            // right sprite
            bounds = spriteRight.bounds.max.x;
            if (Mathf.Abs(bounds) < PlatformMain.XBorderSize)
            {
                var size = spriteRight.size;
                size.x += Time.deltaTime * _growth;
                spriteRight.size = size;
            }
        }

        public Bounds GetBounds()
        {
            var min = spriteLeft.bounds.min;
            var max = spriteRight.bounds.max;
            var bounds = new Bounds();
            bounds.min = min;
            bounds.max = max;
            return bounds;
        }
        public Vector2 GetSize => new Vector2(spriteLeft.size.x + spriteRight.size.x,spriteLeft.size.y);
        // public Vector2 GetMiddlePoint => _sprite.bounds.center;
        public Vector2 GetMiddlePoint => transform.position;
    }
}