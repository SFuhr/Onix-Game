using System;
using System.Collections.Generic;
using Level;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Items
{
    [Serializable]
    public class ItemsManager
    {
        [SerializeField] private Ruby ruby;
        [SerializeField] private Star star;
        [SerializeField] private LayerMask itemLayer;

        private List<ItemBase> _activeItems;
        private Collider[] _scanResult;

        private const byte RubyId = 3;
        private const byte StarId = 4;

        public void Initialize()
        {
            _activeItems = new List<ItemBase>();
            
            // we can't have more than 10 items per row, so this array has a capacity of 10
            _scanResult = new Collider[10];
        }

        public void SpawnItem(byte cellValue, Vector3 worldPosition)
        {
            switch (cellValue)
            {
                case RubyId:
                    InstantiateItem(ruby,worldPosition);
                    break;
                case StarId:
                    InstantiateItem(star,worldPosition);
                    break;
            }
        }

        private void InstantiateItem(ItemBase item, Vector3 pos)
        {
            var newItem = Object.Instantiate(item, pos, Quaternion.identity);
            _activeItems.Add(newItem);
        }

        private void RemoveSpecificItem(ItemBase item)
        {
            if (!_activeItems.Contains(item)) return;
            _activeItems.Remove(item);
        }

        public void WipeActiveItems()
        {
            if (_activeItems.Count == 0) return;

            foreach (var item in _activeItems)
            {
                item.Remove();
            }
            _activeItems.Clear();
        }

        public void ScanForItemsByBounds(Bounds bounds)
        {
            if (Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents / 2, _scanResult,Quaternion.identity, itemLayer) <= 0) return;

            foreach (var col in _scanResult)
            {
                if (!col.TryGetComponent(out ItemBase item)) continue;
                item.Use();
            }
        }
    }
}