using UnityEngine;

namespace Items
{
    public abstract class BaseItem : MonoBehaviour, IUsable
    {
        private bool _used;

        public BaseItem GetItem() => this;

        public void Use()
        {
            UseActions();
        }

        protected abstract void UseActions();

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}