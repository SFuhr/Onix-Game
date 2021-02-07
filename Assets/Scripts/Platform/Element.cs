using System;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace Platform
{
    public class Element : MonoBehaviour
    {
        private float _growth = 0.25f;
        private PlatformMain _owner;
        private SpriteRenderer _sprite;

        private void OnEnable()
        {
            _sprite = GetComponent<SpriteRenderer>();
            
            // Debug.Log($"{box.min}, {box.center}, {box.max}");
        }

        public void Deactivate()
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public void SetOwnerPlatform(PlatformMain platform)
        {
            _owner = platform;
        }

        public void AdjustElement(float width,float speed)
        {
            _sprite.size = new Vector2(width,_sprite.size.y);
            _growth = speed;
        }

        public void Tick()
        {
            var size = _sprite.size;
            size.x += Time.deltaTime * _growth;

            _sprite.size = size;
        }

        public Bounds GetBounds => _sprite.bounds;
        public Vector2 GetSize => _sprite.size;
        // public Vector2 GetMiddlePoint => _sprite.bounds.center;
        public Vector2 GetMiddlePoint => transform.position;
    }
}