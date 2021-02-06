using System;
using UnityEngine;

namespace Platform
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private int elementsSize = 40;
        [SerializeField] private Element element;

        private Element[] _elements;

        private void OnEnable()
        {
            if (element != null) SpawnElements();
        }

        void SpawnElements()
        {
            _elements = new Element[elementsSize];
            _elements[0] = element;
            
            var position = element.transform.position;

            for (int i = 1; i < elementsSize; i++)
            {
                position.x += .25f;
                var e = Instantiate(element, position, Quaternion.identity,transform);
            }
        }
    }
}