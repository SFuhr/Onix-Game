using System;
using Items;
using UnityEngine;

namespace Player
{
    public class Score : MonoBehaviour
    {
        private int _score;

        public static Action<BaseItem> ItemAcquired;
        public static void OnItemAcquired(BaseItem baseItem) => ItemAcquired?.Invoke(baseItem);

        private void Awake()
        {
            ItemAcquired += item =>
            {
                
            };
        }
    }
}