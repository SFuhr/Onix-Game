using System;
using UnityEngine;

namespace Level
{
    public class MainGrid : MonoBehaviour
    {
        [SerializeField] private MeshRenderer gridMesh;
        [SerializeField] private MeshRenderer cellPrefab;

        private Texture _texture;
        private Material _material;
        private SpriteRenderer _sprite;

        private void Start()
        {
            // Test();
        }

        private void Test()
        {
            _material = gridMesh.material;
            _texture = _material.mainTexture;

            var width = _texture.width;
            var height = _texture.height;
            
            Debug.Log($"Texture size: {width.ToString()}x{height.ToString()}");

            var zeroPoint = gridMesh.bounds.min;

            float step = .02f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = new Vector3(zeroPoint.x + x * step,zeroPoint.y + y * step,0);
                    Instantiate(cellPrefab, cell, Quaternion.identity,transform);
                }
            }
        }
    }
}