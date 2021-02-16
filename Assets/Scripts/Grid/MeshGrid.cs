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
        [Header("Tear")]
        [Tooltip("Percentage of texture's width")]
        [SerializeField] [Range(1, 100)] private int tearStartSize = 10;
        [SerializeField] [Range(1, 100)] private int tearFinalSize = 50;
        [SerializeField] [Min(1)] private int tearStartHeight = 8;
        [SerializeField] [Range(1, 1000)] private int updateAccuracy = 1000;

        [Header("Obstacles")]
        [SerializeField] [Range(0,100)] private float obstacleSpawnChance = 30f;
        [SerializeField] private Texture2D[] obstacleTexture;

        private int Width => _texture.width;
        private int Height => _texture.height;
        private Color ColorEmpty => Color.Lerp(_colorEmpty, _colorEmpty * _colorEmptyRange, Random.Range(0f, 1f));
        private Color ColorFilled => _colorFilled;

        private Color _colorEmpty;
        private Color _colorFilled;
        private float _colorEmptyRange;
        private int[,] _grid;
        private Renderer _renderer;
        private Texture2D _texture;
        private Color[] _colors;
        private int _moverFirstPoint;
        private int _currentLine;
        private int _currentXPos;
        private int _spreadReady;
        private int _tearFinalSize;
        private int _spreadEachXLines;
        private bool _initialized;
        
        public void SetColors(Color empty, Color filled, float emptyRange = 1)
        {
            _colorEmpty = empty;
            _colorFilled = filled;
            _colorEmptyRange = emptyRange;
        }

        public void FullReset()
        {
            if (!_initialized)
            {
                _initialized = true;
                InitializeGrid();
            }
            else
            {
                GenerateCells();
            }
        }

        private void InitializeGrid()
        {
            _renderer = GetComponent<Renderer>();
            _texture = Instantiate(_renderer.material.mainTexture) as Texture2D;
            _renderer.material.mainTexture = _texture;
            _colors = new Color[Width * Height];
            _grid = new int[Width, Height];
            
            GenerateCells();
        }

        private void GenerateCells()
        {
            // deleting all cells
            WipeCells();
            
            // calculating tear sizes and another very important calculations
            var startSize = (int) Mathf.Round(Width * (tearStartSize * 0.01f));
            if (startSize % 2 != 0) startSize++;

            _tearFinalSize = (int) Mathf.Round(Width * (tearFinalSize * 0.01f));
            if (_tearFinalSize % 2 != 0) _tearFinalSize++;
            if (_tearFinalSize <= startSize) _tearFinalSize = startSize + 2;
            
            _currentLine = tearStartHeight;

            _spreadEachXLines = Height / ((_tearFinalSize - startSize) / 2);
            _spreadReady = _spreadEachXLines;

            _currentXPos = Width / 2;

            // creating obstacles
            var yGap = Height / (Width / 2);
            var obstacleYSpawn = yGap;

            while (_currentLine > obstacleYSpawn)
            {
                obstacleYSpawn += yGap;
            }
            while (obstacleYSpawn < Height)
            {
                if (Random.Range(0, 100) < obstacleSpawnChance)
                {
                    GenerateObstacle(obstacleYSpawn);
                }
                obstacleYSpawn += (Width / 2);
            }
            
            // beginning of the tear
            var startPoint = Width / 2 - startSize / 2 + 1;

            for (int y = 0; y <= _currentLine; y++)
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

            // filling color array
            var index = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _colors[index] = _grid[x, y] == 0 ? ColorEmpty : ColorFilled;
                    index++;
                }
            }

            // applying colors to the texture
            ApplyTexture();
        }

        private void GenerateObstacle(int yPosition)
        {
            var texture = obstacleTexture[Random.Range(0, obstacleTexture.Length)];
            var pixels = texture.GetPixels();
            
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var index = y * texture.width + x;
                    
                    var ySpawn = y + yPosition;
                    if (ySpawn >= Height) return;
                    
                    _grid[x, ySpawn] = pixels[index] == Color.white ? 0 : 2;
                }
            }
        }

        private void WipeCells()
        {
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _grid[x, y] = 0;
                    UpdateCellColor(x, y);
                }
            }

            ApplyTexture();
        }

        public void UpdatePixels(Vector2 moverPos, float maxMoverXPos)
        {
            var y = Mathf.Abs(Mathf.Round(Mathf.Min(1, moverPos.y * updateAccuracy)));
            var expectedLine = (int) (Height * (y / (updateAccuracy * 100)));

            var x = (int) Mathf.Round((Width / 2 * moverPos.x) / maxMoverXPos) + (Width / 2);

            // Debug.Log($"{maxMoverXPos}; {x}; {Width}");

            SetNewLines(expectedLine, x);
        }

        private void SideMover(int newXPos)
        {
            var difference = Mathf.Abs(Mathf.Abs(_currentXPos) - Mathf.Abs(newXPos));
            var y = _currentLine - 1;

            for (int i = 0; i < difference; i++)
            {
                if (_currentXPos < newXPos) // move left 
                {
                    for (int x = 1; x < Width - 1; x++)
                    {
                        if (_grid[x, y] == 0 && _grid[x + 1, y] == 1)
                        {
                            _grid[x, y] = 1;
                            _grid[x + 1, y] = 0;
                        }
                    }
                }
                else
                {
                    for (int x = Width - 1; x > 0; x--)
                    {
                        if (_grid[x, y] == 0 && _grid[x - 1, y] == 1)
                        {
                            _grid[x, y] = 1;
                            _grid[x - 1, y] = 0;
                        }
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                UpdateCellColor(x, y);
            }

            _currentXPos = newXPos;
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
                    }

                    // updating pixels
                    UpdateCellColor(x, y);
                }
            }

            // changing a x position
            if (_currentXPos != xPosition) SideMover(xPosition);

            ApplyTexture();
        }

        private void ApplyTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
        }

        private void UpdateCellColor(int x, int y)
        {
            var colorIndex = y * Width + x;

            if (_grid[x, y] != 0)
            {
                _colors[colorIndex] = ColorFilled;
            }
        }
    }
}