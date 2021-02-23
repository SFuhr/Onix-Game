using System;
using System.Collections.Generic;
using Level;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Items
{
    [Serializable]
    public class ItemsManager
    {
        [SerializeField] private Ruby ruby;
        [SerializeField] private Star[] stars;
        [SerializeField] [Range(0, 100)] private float bigStarSpawnChance = 20;
        [SerializeField] private LayerMask itemLayer;
        [SerializeField] private Explosion explosion;

        private List<BaseItem> _activeItems;
        private Vector3[] _pointsToScan;
        private int _scanId;
        private Collider[] _scanResult;

        private const int PointsNeeded = 2;
        private const int RubyId = 3;
        private const int StarId = 4;

        public void Initialize()
        {
            _activeItems = new List<BaseItem>();
            _pointsToScan = new Vector3[PointsNeeded];
            
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
                    var id = Random.Range(0, 100) < bigStarSpawnChance ? 1 : 0;
                    InstantiateItem(stars[id],worldPosition);
                    break;
            }
        }

        private void InstantiateItem(BaseItem baseItem, Vector3 pos)
        {
            var newItem = Object.Instantiate(baseItem, pos, Quaternion.identity);
            _activeItems.Add(newItem);
        }

        private void RemoveSpecificItem(BaseItem baseItem)
        {
            if (!_activeItems.Contains(baseItem)) return;
            _activeItems.Remove(baseItem);
        }

        public void WipeActiveItems()
        {
            _scanId = 0;
            if (_activeItems.Count == 0) return;

            foreach (var item in _activeItems)
            {
                item.RemoveInstantly();
            }
            _activeItems.Clear();
        }

        private void UseAndRemoveItem(IUsable usable)
        {
            if (!usable.Usable()) return;
            
            _activeItems.Remove(usable.GetItem());
            usable.Use();

            Object.Instantiate(explosion, usable.Position(), Quaternion.identity);
        }

        private void ScanForItemsByBounds(Vector3 center, Vector3 extends)
        {
            if (Physics.OverlapBoxNonAlloc(center, extends * 0.5f, _scanResult,Quaternion.identity, itemLayer) <= 0) return;

            for (int i = 0; i < _scanResult.Length; i++)
            {
                var col = _scanResult[i];
                if (col == null) return;

                if (!col.TryGetComponent(out IUsable usable)) continue;
                UseAndRemoveItem(usable);
            }
        }

        public void PrepareToScan(Vector3 pos)
        {
            _pointsToScan[_scanId] = pos;
            
            _scanId++;
            if (_scanId >= PointsNeeded)
            {
                _scanId = 0;

                var scale = _pointsToScan[1] - _pointsToScan[0];
                scale.x = Mathf.Abs(scale.x);
                scale.y = Mathf.Abs(scale.y);
                scale.z = Mathf.Abs(scale.z);
                var center = (_pointsToScan[1] + _pointsToScan[0]) * 0.5f;
                
                ScanForItemsByBounds(center,scale);
            }
        }
    }
}