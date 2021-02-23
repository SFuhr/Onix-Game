using System;
using Grid;
using UnityEngine;

namespace Items
{
    [AddComponentMenu("Pickup/Ruby")]
    public class Ruby : BaseItem
    {
        public static Action ResetRubies;
        public static void OnResetRubies() => ResetRubies?.Invoke();

        public static Action RubyAcquired;
        private static void OnRubyAcquired() => RubyAcquired?.Invoke();

        protected override void UseActions()
        {
            // var grid = FindObjectOfType<GridMesh>();
            // grid.RubyAcquired();
            // GridMesh.Instance.RubyAcquired();
            OnRubyAcquired();
        }
    }
}