using UnityEngine;

namespace Items
{
    public interface IUsable
    {
        Vector3 Position();
        bool Usable();
        BaseItem GetItem();
        void Use();
    }
}