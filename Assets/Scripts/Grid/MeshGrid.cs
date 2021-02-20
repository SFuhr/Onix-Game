using System.Collections.Generic;
using Level;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid
{
    public class MeshGrid : MonoBehaviour
    {
        [Header("Tear")] [Tooltip("Percentage of texture's width")] [SerializeField] [Range(1, 100)]
        private int tearStartSize = 10;

        [SerializeField] [Range(1, 100)] private int tearFinalSize = 50;
        [SerializeField] [Min(1)] private int tearStartHeight = 8;
        [SerializeField] [Range(1, 1000)] private int updateAccuracy = 1000;

        [Header("Pixel Colors")] [SerializeField]
        private Color tearColor = new Color(0, 0, 0, 0);

        [SerializeField] [Range(0, 1)] private float randomColorRange = 0.85f;

        [Header("Obstacles")] [SerializeField] [Range(0, 100)]
        private float obstacleSpawnChance = 30f;

        [SerializeField] private Texture2D[] obstacleTexture;

        [Header("Pickup Prefabs")]
        [SerializeField] private Ruby ruby;
        [SerializeField] private Star star;
        [SerializeField] [Range(0, 100)] private float pickupSpawnChance = 40f;
        [SerializeField] private Texture2D[] pickupTexture;

        private byte[,] _grid;
        private Color[] _colors;
        private List<BaseItem> _items;

        private Renderer _renderer;
        private Texture2D _texture;
        private Color _colorEmpty;
        private int _moverFirstPoint;
        private int _currentLine;
        private int _spreadReady;
        private int _tearFinalSize;
        private int _spreadEachXLines;
        private bool _messyGrid;

        private int Width => _texture.width;
        private int Height => _texture.height;
        private Vector3 Size => _renderer.bounds.size;

        private Color ColorEmpty => Color.Lerp(_colorEmpty, _colorEmpty * randomColorRange, Random.Range(0f, 1f));
        private Color ColorFilled => tearColor;
        private Color ColorPickupRuby => Color.blue;
        private Color ColorPickupStar => Color.green;

        public bool IsMessy => _messyGrid;

        public void Initialize()
        {
            InitializeGrid();
        }

        public void ResetGrid()
        {
            GenerateCells();
        }

        public void BeginMessing()
        {
            _messyGrid = true;
        }

        private void InitializeGrid()
        {
            GetComponent<MeshFilter>();
            _renderer = GetComponent<Renderer>();
            _texture = Instantiate(_renderer.material.mainTexture) as Texture2D;
            _renderer.material.mainTexture = _texture;

            _colors = new Color[Width * Height];
            _grid = new byte[Width, Height];
            _items = new List<BaseItem>();

            GenerateCells();
        }

        private void GenerateCells()
        {
            // deleting all cells
            WipeEverything();

            // setting random color to the grid
            _colorEmpty = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            // calculating tear sizes and another very important calculations
            var startSize = (int) Mathf.Round(Width * (tearStartSize * 0.01f));
            if (startSize % 2 != 0) startSize++;

            _tearFinalSize = (int) Mathf.Round(Width * (tearFinalSize * 0.01f));
            if (_tearFinalSize % 2 != 0) _tearFinalSize++;
            if (_tearFinalSize <= startSize) _tearFinalSize = startSize + 2;

            _currentLine = tearStartHeight;

            _spreadEachXLines = Height / ((_tearFinalSize - startSize) / 2);
            _spreadReady = _spreadEachXLines;

            // creating obstacles and pickups
            // important hardcoded calculation gives us the first line to spawn obstacles or pickups in
            var spawnLine = _currentLine * 3;

            while (spawnLine < (Height - Width / 2))
            {
                if (Random.Range(0, 100) < obstacleSpawnChance)
                {
                    // spawn an obstacle
                    SpawnObjectByTexture(spawnLine, ref obstacleTexture);
                }
                else if (Random.Range(0, 100) < pickupSpawnChance)
                {
                    // spawn a pickup
                    SpawnObjectByTexture(spawnLine, ref pickupTexture);
                }

                spawnLine += (Width / 2);
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

            // Generating Stars and Rubies
            GeneratePickups();

            // applying colors to the texture
            ApplyTexture();
        }

        private void SpawnObjectByTexture(int yPosition, ref Texture2D[] possibleObjects)
        {
            if (Random.Range(0, 100) < obstacleSpawnChance) return;

            var texture = possibleObjects[Random.Range(0, possibleObjects.Length)];
            var pixels = texture.GetPixels();

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var index = y * texture.width + x;

                    var ySpawn = y + yPosition;
                    if (ySpawn >= Height) return;

                    var pixel = pixels[index];

                    if (pixel == Color.black) _grid[x, ySpawn] = 2;
                    else
                    {
                        // 3 = ruby; 4 = star
                        if (pixel == ColorPickupRuby)
                        {
                            _grid[x, ySpawn] = 3;
                        }
                        else if (pixel == ColorPickupStar)
                        {
                            _grid[x, ySpawn] = 4;
                        }
                    }
                }
            }
        }

        private void GeneratePickups()
        {
            // 3 = ruby; 4 = star
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = _grid[x, y];

                    if (cell < 3) continue;

                    var pos = GridToWorldPosition(x, y);
                    BaseItem item;
                    
                    if (cell == 3)
                    {
                        item = Instantiate(ruby, pos, Quaternion.identity);
                    }
                    else
                    {
                        item = Instantiate(star, pos, Quaternion.identity);
                    }
                    _items.Add(item);
                }
            }
        }

        private void WipeEverything()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _grid[x, y] = 0;
                    UpdateCellColor(x, y);
                }
            }

            if (_items.Count > 0)
            {
                foreach (var item in _items)
                {
                    item.Remove();
                }
            }
            _items.Clear();
            
            _messyGrid = false;
            ApplyTexture();
        }

        private Vector3 GridToWorldPosition(int gridX, int gridY)
        {
            var vec = Vector3.zero;

            float x = Width - gridX;
            vec.x = x / Width * Size.x;

            float y = gridY;
            vec.y = y / Height * -Size.y;

            return vec;
        }

        public void UpdatePixels(float moverYPos, int x)
        {
            if (!_messyGrid) return;

            var y = Mathf.Abs(Mathf.Round(Mathf.Min(1, moverYPos * updateAccuracy)));
            var expectedLine = (int) (Height * (y / (updateAccuracy * 100)));

            if (_currentLine >= expectedLine) return;
            if (_currentLine >= Height) return;

            UpdateTearCells(expectedLine, x);
        }

        private void UpdateTearCells(int expectedLine, int xAxis)
        {
            // changing grid map
            var hasActiveCells = false;
            for (int y = _currentLine; y < expectedLine; y++, _currentLine++)
            {
                _spreadReady = _spreadReady > 0 ? _spreadReady - 1 : _spreadEachXLines;
                for (int x = 0; x < Width; x++)
                {
                    if (_grid[x, y] == 0 || _grid[x, y] == 1)
                    {
                        if (_grid[x, y - 1] == 1)
                        {
                            _grid[x, y] = 1;
                            hasActiveCells = true;
                        }
                        else _grid[x, y] = 0;
                    }

                    if (_spreadReady == 0 && x > 0 && x < Width - 1)
                    {
                        if (_grid[x, y - 1] == 0 && (_grid[x - 1, y - 1] == 1 || _grid[x + 1, y - 1] == 1))
                        {
                            _grid[x, y] = 1;
                            hasActiveCells = true;
                        }
                    }

                    // updating pixels
                    UpdateCellColor(x, y);
                }
            }

            // changing a x position
            if (xAxis != 0) TearSideMover(xAxis);

            ApplyTexture();

            if (!hasActiveCells)
            {
                // no active tear left, level is lost
                LevelManager.OnLevelEnded(false);
            }
        }

        private void TearSideMover(int xAxis)
        {
            var y = _currentLine - 1;

            for (int i = 0; i < Mathf.Abs(xAxis); i++)
            {
                if (xAxis > 0) // moving right 
                {
                    for (int x = 1; x < Width - 1; x++)
                    {
                        if (_grid[x + 1, y] != 1) continue;

                        if (_grid[x, y] == 0) _grid[x, y] = 1;
                        _grid[x + 1, y] = 0;
                    }
                }
                else
                {
                    for (int x = Width - 1; x > 0; x--) // moving left
                    {
                        if (_grid[x - 1, y] != 1) continue;

                        if (_grid[x, y] == 0) _grid[x, y] = 1;
                        _grid[x - 1, y] = 0;
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                UpdateCellColor(x, y);
            }
        }

        private void ApplyTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
        }

        // function updates the color in array based on grid value
        private void UpdateCellColor(int x, int y)
        {
            var colorIndex = y * Width + x;
            var cell = _grid[x, y];

            if (cell == 0) return;
            _colors[colorIndex] = ColorFilled;
        }
    }
}