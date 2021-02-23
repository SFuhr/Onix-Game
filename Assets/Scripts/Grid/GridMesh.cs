using System;
using Level;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid
{
    public class GridMesh : MonoBehaviour
    {
        [Header("Tear")]
        [Tooltip("Percentage of texture's width")]
        [SerializeField] [Range(1, 100)] private int tearStartSize = 10;

        [SerializeField] [Range(1, 100)] private int tearFinalSize = 50;
        [SerializeField] [Min(1)] private int tearStartHeight = 8;
        [SerializeField] [Range(1, 1000)] private int updateAccuracy = 1000;

        [Tooltip("The total number of lines needed to spread is divided by this value when rubies effect is active.")]
        [SerializeField] [Range(1, 10)] private int improvedSpreadValue = 4;

        [Header("Pixel Colors")]
        [SerializeField] private Color tearColor = new Color(0, 0, 0, 0);

        [SerializeField] [Range(0, 1)] private float randomColorRange = 0.85f;
        [SerializeField] private bool useColorBorder = true;
        [SerializeField] private Color borderColor = new Color(0, 0, 0, 1);

        [Header("Obstacles")]
        [SerializeField] [Range(0, 100)] private float obstacleSpawnChance = 30f;

        [SerializeField] private Texture2D[] obstacleTexture;

        [Header("Items")]
        [SerializeField] private Texture2D[] pickupTexture;
        [SerializeField] [Range(0, 100)] private float pickupSpawnChance;
        [SerializeField] private ItemsManager itemsManager;

        private byte[,] _grid;

        private Color[] _colors;
        // private List<BaseItem> _items;

        private Renderer _renderer;
        private Texture2D _texture;
        private Color _colorEmpty;
        private float _expectedHorizontalStep;
        private int _horizontalMoveValue;
        private int _moverFirstPoint;
        private int _currentLine;
        private int _spreadReady;
        private int _tearFinalSize;
        private int _spreadEachLines;
        private int _spreadEachLinesImproved;
        private int _rubiesAcquired;
        private bool _messyGrid;
        private bool _improvedSpreadEnabled;
        private bool _hasActiveCells;

        private const int RubySpreadRubiesNeeded = 3;
        private int Width => _texture.width;
        private int Height => _texture.height;
        private Vector3 Size => _renderer.bounds.size;
        private Color ColorEmpty => Color.Lerp(_colorEmpty, _colorEmpty * randomColorRange, Random.Range(0f, 1f));
        private Color ColorFilled => tearColor;
        private Color ColorBorder => borderColor;
        private Color ColorPickupRuby => Color.blue;
        private Color ColorPickupStar => Color.green;
        public bool IsMessy => _messyGrid;

        private void OnEnable()
        {
            Ruby.RubyAcquired += () =>
            {
                _rubiesAcquired++;
                if (_rubiesAcquired >= RubySpreadRubiesNeeded)
                {
                    _rubiesAcquired = 0;
                    _improvedSpreadEnabled = true;
                }
            };
        }

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

            itemsManager.Initialize();

            GenerateCells();
        }

        private void GenerateCells()
        {
            // deleting all cells
            WipeEverything();

            // setting random color to the grid
            _colorEmpty = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            // calculating tear sizes and another very important calculations
            var startSize = Mathf.RoundToInt(Width * (tearStartSize * 0.01f));
            if (startSize % 2 != 0) startSize++;

            _tearFinalSize = Mathf.RoundToInt(Width * (tearFinalSize * 0.01f));
            if (_tearFinalSize % 2 != 0) _tearFinalSize++;
            if (_tearFinalSize <= startSize) _tearFinalSize = startSize + 2;

            _currentLine = tearStartHeight;

            _spreadEachLines = Height / ((_tearFinalSize - startSize) / 2);
            _spreadReady = _spreadEachLines;

            _spreadEachLinesImproved = Mathf.RoundToInt((float) _spreadEachLines / improvedSpreadValue);

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

            // creating obstacles and pickups
            SpawnObjects();

            // filling the color array
            FillColorArray();

            // Generating Stars and Rubies
            GeneratePickups();

            // applying colors to the texture
            ApplyTexture();
        }

        private void SpawnObjects()
        {
            // important hardcoded calculation gives us the first line to spawn obstacles or pickups in
            var spawnLine = _currentLine * 3;
            var obstacleOrder = false;

            while (spawnLine < (Height - Width / 2))
            {
                obstacleOrder = !obstacleOrder;
                if (obstacleOrder)
                {
                    if (Random.Range(0, 100) < obstacleSpawnChance)
                    {
                        // spawn an obstacle
                        SpawnObjectByTexture(spawnLine, ref obstacleTexture);
                    }
                }
                else
                {
                    if (Random.Range(0, 100) < pickupSpawnChance)
                    {
                        // spawn a pickup
                        SpawnObjectByTexture(spawnLine, ref pickupTexture);
                    }
                }

                spawnLine += (Width / 2);
            }
        }

        private void SpawnObjectByTexture(int yPosition, ref Texture2D[] possibleObjects)
        {
            var texture = possibleObjects[Random.Range(0, possibleObjects.Length)];
            var pixels = texture.GetPixels();

            // rotating pixels array by 180 degrees, in order to match its png
            var a = 0;
            var b = pixels.Length - 1;

            while (a < b)
            {
                var temp = pixels[a];
                pixels[a] = pixels[b];
                pixels[b] = temp;

                a++;
                b--;
            }

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
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = _grid[x, y];

                    if (cell < 3) continue;

                    _grid[x, y] = 0;
                    var pos = GridToWorldPosition(x, y);
                    itemsManager.SpawnItem(cell, pos);
                }
            }
        }

        private void FillColorArray()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    UpdateCellColor(x, y, true);
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
                }
            }

            itemsManager.WipeActiveItems();

            _rubiesAcquired = 0;
            _improvedSpreadEnabled = false;
            _hasActiveCells = false;
            _messyGrid = false;
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

        public void UpdatePixels(float moverXPos, float moverYPos)
        {
            if (!_messyGrid) return;

            _expectedHorizontalStep += moverXPos;
            var newHorizontal = Mathf.RoundToInt(_expectedHorizontalStep);

            var y = Mathf.Abs(Mathf.Round(Mathf.Min(1, moverYPos * updateAccuracy)));
            var expectedLine = (int) (Height * (y / (updateAccuracy * 100)));

            if (_currentLine >= expectedLine) return;
            if (_currentLine >= Height)
            {
                if (_hasActiveCells) LevelManager.OnLevelEnd(true);
                return;
            }

            UpdateTearCells(expectedLine, newHorizontal);
        }

        private void UpdateTearCells(int expectedLine, int horizontal)
        {
            // changing the grid map
            _hasActiveCells = false;
            for (int y = _currentLine; y < expectedLine; y++, _currentLine++)
            {
                // define if the tear is spreading this time
                if (_spreadReady > 0)
                {
                    _spreadReady--;
                }
                else
                {
                    _spreadReady = !_improvedSpreadEnabled ? _spreadEachLines : _spreadEachLinesImproved;
                }

                for (int x = 0; x < Width; x++)
                {
                    if (_grid[x, y] == 0 || _grid[x, y] == 1)
                    {
                        if (_grid[x, y - 1] == 1)
                        {
                            FillCell(x, y);
                        }
                        else _grid[x, y] = 0;
                    }

                    if (_spreadReady == 0 && x > 0 && x < Width - 1)
                    {
                        if (_grid[x, y - 1] == 0 && (_grid[x - 1, y - 1] == 1 || _grid[x + 1, y - 1] == 1))
                        {
                            FillCell(x, y);
                        }
                    }

                    // updating pixels
                    UpdateCellColor(x, y, false);
                }
            }

            // move tear horizontally
            if (_horizontalMoveValue != horizontal)
            {
                TearSideMover(horizontal);
            }

            // check if fast spread must be stopped
            if (_improvedSpreadEnabled)
            {
                if (_grid[0, _currentLine - 1] == 1 || _grid[Width - 1, _currentLine - 1] == 1)
                {
                    Ruby.OnResetRubies();
                    _improvedSpreadEnabled = false;
                }
            }

            // apply colors to the texture
            ApplyTexture();

            if (!_hasActiveCells)
            {
                // no active tears left, level is lost
                LevelManager.OnLevelEnd(false);
            }
        }

        private void FillCell(int x, int y)
        {
            if (!_hasActiveCells) _hasActiveCells = true;

            _grid[x, y] = 1;
            itemsManager.PrepareToScan(GridToWorldPosition(x, y));
        }

        private void TearSideMover(int moverXPos)
        {
            var difference = _horizontalMoveValue - moverXPos;
            _horizontalMoveValue = moverXPos;

            var y = _currentLine - 1;
            difference *= -1;

            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                if (difference > 0) // moving right 
                {
                    for (int x = 0; x < Width - 1; x++)
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
        }

        private void ApplyTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
        }

        // function updates the color in array based on grid value
        private void UpdateCellColor(int x, int y, bool updateEmpty)
        {
            var colorIndex = y * Width + x;
            var cell = _grid[x, y];

            if (cell == 0)
            {
                if (updateEmpty)
                {
                    _colors[colorIndex] = ColorEmpty;
                }

                if (!useColorBorder) return;
                if (x - 1 <= 0 || !CellIsFilled(x - 1, y)) return;

                _colors[colorIndex] = ColorBorder;
            }
            else
            {
                _colors[colorIndex] = ColorFilled;
            }
        }

        private bool CellIsFilled(int x, int y)
        {
            // 0 = filled; 3,4 - filled too but with items
            // 1 - player's tear; 2 - obstacle -> both must be empty
            return _grid[x, y] == 1 || _grid[x, y] == 2;
        }
    }
}