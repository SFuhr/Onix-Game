using Grid;
using UnityEngine;

namespace Items
{
    [AddComponentMenu("Pickup/Ruby")]
    public class Ruby : BaseItem
    {
        protected override void UseActions()
        {
            // var grid = FindObjectOfType<GridMesh>();
            // grid.RubyAcquired();
            GridMesh.Instance.RubyAcquired();
        }
    }
}