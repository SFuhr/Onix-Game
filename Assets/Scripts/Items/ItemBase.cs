using UnityEngine;

namespace Items
{
    public abstract class ItemBase : MonoBehaviour, IUsable
    {
        public abstract void Use();
        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}