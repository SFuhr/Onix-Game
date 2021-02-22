using System.Collections;
using UnityEngine;

namespace Items
{
    public abstract class BaseItem : MonoBehaviour, IUsable
    {
        [SerializeField] [Min(0)] private Vector2 fall;
        [SerializeField] private Collider col;
        [SerializeField] private float lifetimeAfterUse = 1.5f;

        private bool _used;

        public BaseItem GetItem() => this;
        public Vector3 Position() => transform.position;
        public bool Usable() => !_used;

        public void Use()
        {
            if (_used) return;

            _used = true;
            col.enabled = false;

            UseActions();
            
            StartCoroutine(ItemUsed());
        }

        protected abstract void UseActions();

        IEnumerator ItemUsed()
        {
            var timer = lifetimeAfterUse;
            while (timer > 0)
            {
                var pos = transform.position;
                pos.y -= fall.y * Time.deltaTime;
                pos.x -= Random.Range(0, fall.x) * Time.deltaTime;
                
                transform.position = pos;

                timer -= Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        public void RemoveInstantly() => Destroy(gameObject);
        
    }
}