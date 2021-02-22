using System;
using Items;
using UnityEngine;

namespace Player
{
    public class Score : MonoBehaviour
    {
        private int _score;

        public static Action<ItemBase> ItemAcquired;
        public static void OnItemAcquired(ItemBase item) => ItemAcquired?.Invoke(item);

        private void Awake()
        {
            ItemAcquired += item =>
            {
                
            };
        }
    }
}