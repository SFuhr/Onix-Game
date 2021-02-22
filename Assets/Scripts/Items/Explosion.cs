using UnityEngine;

namespace Items
{
    public class Explosion : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}