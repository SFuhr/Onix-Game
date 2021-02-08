using System;
using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using Random = UnityEngine.Random;

namespace Grid
{
    public class MeshGrid : MonoBehaviour
    {
        [Header("Mover")] [SerializeField] private int moverSize = 16;
        [SerializeField] private int spreadEachXLines = 4;
        [SerializeField] private int moverHeight = 8;
        // [SerializeField] private int spreadEachXRows = 1;
        
        [Header("Pixels")] [SerializeField] private Color colorEmpty = new Color(1, 1, 1, 1);
        [SerializeField] private Color colorFilled = new Color(0, 0, 0, 1);
        [SerializeField] [Range(1, 1000)] private int updateAccuracy = 1000;
        
        private int Width => _texture.width;
        private int Height => _texture.height;

        private int[,] _grid;
        private Renderer _renderer;
        private Texture2D _texture;
        private Color[] _colors;
        private int _moverFirstPoint;
        private int _currentLine;
        private int _spreadReady;
        private int _xOffset;

        private void OnEnable()
        {
            _renderer = GetComponent<Renderer>();
            _texture = Instantiate(_renderer.material.mainTexture) as Texture2D;
            _renderer.material.mainTexture = _texture;
            _colors = new Color[Width * Height];

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            _currentLine = moverHeight;
            _spreadReady = spreadEachXLines;
            _xOffset = Width / 2;
            
            _grid = new int[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _grid[x, y] = 0;
                }
            }

            moverSize = Mathf.Clamp(moverSize % 2 == 0 ? moverSize : moverSize + 1, 0, Width);

            // var size = moverSize;
            var startPoint = Width / 2 - moverSize / 2 + 1;

            for (int y = 0; y < moverHeight; y++)
            {
                var size = moverSize;
                for (int x = 0; x < Width; x++)
                {
                    if (x < startPoint) continue;
                    if (size > 0)
                    {
                        _grid[x, y] = 1;
                        size--;
                    }
                }
            }

            var index = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _colors[index] = _grid[x, y] == 0 ? colorEmpty : colorFilled;
                    index++;
                }
            }

            _texture.SetPixels(_colors);
            _texture.Apply();

            // StartCoroutine(Test());
        }

        public void UpdatePixels(Vector2 moverPos)
        {
            var moverYPos = Mathf.Abs(Mathf.Round(Mathf.Min(1,moverPos.y * updateAccuracy)));
            var expectedLine = (int)(Height * (moverYPos / (updateAccuracy * 100)));

            var moverXPos = Mathf.Round(Mathf.Clamp(moverPos.x, -Width, Width) * updateAccuracy);
            var offset = (int)(Width * (moverXPos / (updateAccuracy * 100)));
            
            Debug.Log($"Mover y pos = {moverYPos}; x offset = {offset}");
            SetNewLines(expectedLine,offset);
        }

        private void SetNewLines(int expectedLine, int xOffset)
        {
            if (_currentLine >= expectedLine) return;
            if (_currentLine >= Height) return;
            
            // changing grid map
            for (int y = _currentLine; y < expectedLine; y++, _currentLine++)
            {
                _spreadReady = _spreadReady > 0 ? _spreadReady - 1 : spreadEachXLines;
                for (int x = 0; x < Width; x++)
                {
                    _grid[x, y] = _grid[x, y - 1] == 1 ? 1 : 0;

                    if (_spreadReady == 0 && x > 0 && x < Width - 1)
                    {
                        if (_grid[x, y - 1] == 00 && (_grid[x - 1, y - 1] == 1 || _grid[x + 1, y - 1] == 1))
                        {
                            _grid[x, y] = 1;
                        }
                    }
                    
                    // updating pixels
                    var colorIndex = y * Width + x;
                    _colors[colorIndex] = _grid[x, y] == 0 ? colorEmpty : colorFilled;
                }
            }
            
            _texture.SetPixels(_colors);
            _texture.Apply();
        }
    }
}