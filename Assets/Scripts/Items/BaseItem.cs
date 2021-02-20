using UnityEngine;

namespace Items
{
    public abstract class BaseItem : MonoBehaviour, IUsable
    {
        private void OnTriggerEnter(Collider other)
        {
            Use();
        }

        public abstract void Use();
        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}