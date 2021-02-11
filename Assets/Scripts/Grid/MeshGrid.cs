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
        [Header("Mover")] [SerializeField] [Tooltip("Percentage of texture's width")] [Range(1, 100)]
        private int moverStartSize = 10;

        [SerializeField] [Tooltip("Percentage of texture's width")] [Range(1, 100)]
        private int moverFinalSize = 50;

        // [SerializeField] private int spreadEachXLines = 4;
        [SerializeField] private int moverHeight = 8;

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
        private int _currentXPos;
        private int _spreadReady;
        private int _moverFinalSize;
        private int _spreadEachXLines;

        private void OnEnable()
        {
            _renderer = GetComponent<Renderer>();
            _texture = Instantiate(_renderer.material.mainTexture) as Texture2D;
            _renderer.material.mainTexture = _texture;
            _colors = new Color[Width * Height];

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            var startSize = (int) Mathf.Round(Width * (moverStartSize * 0.01f));
            if (startSize % 2 != 0) startSize++;

            _moverFinalSize = (int) Mathf.Round(Width * (moverFinalSize * 0.01f));
            if (_moverFinalSize % 2 != 0) _moverFinalSize++;
            if (_moverFinalSize <= startSize) _moverFinalSize = startSize + 2;

            _currentLine = moverHeight;

            _spreadEachXLines = Height / ((_moverFinalSize - startSize) / 2);
            _spreadReady = _spreadEachXLines;

            _currentXPos = 0;

            _grid = new int[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _grid[x, y] = 0;
                }
            }

            var startPoint = Width / 2 - startSize / 2 + 1;

            for (int y = 0; y < moverHeight; y++)
            {
                var lineSize = startSize;
                for (int x = 0; x < Width; x++)
                {
                    if (x < startPoint) continue;
                    if (lineSize > 0)
                    {
                        _grid[x, y] = 1;
                        lineSize--;
                    }
                }
            }
            
            for (int x = 0; x < Width; x++)
            {
                _grid[x, 200] = Random.Range(0f, 100f) > 50 ? 2 : 0;
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
        }

        public void UpdatePixels(Vector2 moverPos, float maxMoverXPos)
        {
            var y = Mathf.Abs(Mathf.Round(Mathf.Min(1, moverPos.y * updateAccuracy)));
            var expectedLine = (int) (Height * (y / (updateAccuracy * 100)));

            var x = (int) Mathf.Round((Width / 2 * moverPos.x) / maxMoverXPos);

            // Debug.Log($"{maxMoverXPos}; {x}; {Width}");

            SetNewLines(expectedLine, x);
        }

        private void SideMover(int xOffset)
        {
            var difference = Mathf.Abs(_currentXPos) - Mathf.Abs(xOffset);
            var y = _currentLine;
            for (int i = 0; i < difference; i++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x + 1 < Width)
                    {
                        if (_grid[x, y] == 0 && _grid[x + 1, y] == 1)
                        {
                            _grid[x, y] = 1;
                            _grid[x + 1, y] = 0;
                        }
                    }
                }
            }
            _currentXPos = xOffset;
        }

        private void SetNewLines(int expectedLine, int xPosition)
        {
            if (_currentLine >= expectedLine) return;
            if (_currentLine >= Height) return;

            // changing grid map
            for (int y = _currentLine; y < expectedLine; y++, _currentLine++)
            {
                _spreadReady = _spreadReady > 0 ? _spreadReady - 1 : _spreadEachXLines;
                for (int x = 0; x < Width; x++)
                {
                    if (_grid[x, y] == 0)
                    {
                        _grid[x, y] = _grid[x, y - 1] == 1 ? 1 : 0;
                    }

                    if (_spreadReady == 0 && x > 0 && x < Width - 1)
                    {
                        if (_grid[x, y - 1] == 0 && (_grid[x - 1, y - 1] == 1 || _grid[x + 1, y - 1] == 1))
                        {
                            _grid[x, y] = 1;
                        }

                        if (xPosition != _currentXPos) SideMover(xPosition);
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