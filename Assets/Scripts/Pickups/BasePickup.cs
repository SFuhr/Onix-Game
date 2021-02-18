using UnityEngine;

namespace Pickups
{
    public abstract class BasePickup : MonoBehaviour, IPickable
    {
        private void OnTriggerEnter(Collider other)
        {
            PickUp();
        }

        public abstract void PickUp();
    }
}