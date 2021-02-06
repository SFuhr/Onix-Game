using System;
using UnityEngine;

namespace Platform
{
    public class PlatformElement : MonoBehaviour
    {
        [SerializeField] private float growSpeed = 0.25f;
        
        private SpriteRenderer _sprite;

        private void OnEnable()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Init(float xSize)
        {
            _sprite.size = new Vector2(xSize,_sprite.size.y);
        }

        public void Tick()
        {
            var add = new Vector2(Time.deltaTime * growSpeed,0);
            var size = _sprite.size;
            
            size += add;
            _sprite.size = size;
        }
    }
}