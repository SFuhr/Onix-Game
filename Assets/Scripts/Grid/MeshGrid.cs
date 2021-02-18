using Level;
using Pickups;
using UnityEngine;
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

        [Header("Pickup Prefabs")]
        [SerializeField] private Ruby ruby;
        [SerializeField] private Star star;
        [SerializeField] [Range(0, 100)] private float pickupSpawnChance = 40f; 
        [SerializeField] private Texture2D[] pickupTexture;
        
        private byte[,] _grid;
        private Color[] _colors;
        
        private Color _colorEmpty;
        private Color _colorFilled;
        private float _colorEmptyRange;
        private Renderer _renderer;
        private Texture2D _texture;
        private int _moverFirstPoint;
        private int _currentLine;
        private int _spreadReady;
        private int _tearFinalSize;
        private int _spreadEachXLines;
        private bool _initialized;
        
        private int Width => _texture.width;
        private int Height => _texture.height;
        private Color ColorEmpty => Color.Lerp(_colorEmpty, _colorEmpty * _colorEmptyRange, Random.Range(0f, 1f));
        private Color ColorFilled => _colorFilled;
        
        private Color ColorPickupRuby => Color.blue;
        private Color ColorPickupStar => Color.yellow;
        
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
            _grid = new byte[Width, Height];
            
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

            // creating obstacles and pickups
            // important hardcoded calculation gives us the first line to spawn in
            var spawnLine = _currentLine * 4;
            
            while (spawnLine < Height)
            {
                
                if (Random.Range(0, 100) < obstacleSpawnChance)
                {
                    // spawn an obstacle
                    SpawnObjectByTexture(spawnLine, ref obstacleTexture);

                } else if (Random.Range(0, 100) < pickupSpawnChance)
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

            // applying colors to the texture
            ApplyTexture();
        }

        private void SpawnObjectByTexture(int yPosition, ref Texture2D[] possibleObjects)
        {
            if (Random.Range(0, 100) < obstacleSpawnChance) return;
            
            var texture = possibleObjects[Random.Range(0, possibleObjects.Length)];
            var pixels = texture.GetPixels();
            
            // randomly flip obstacle
            // if (Random.Range(0f, 100f) < 50)
            // {
            //     // TODO: implement obstacle flipping
            // }
            
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var index = y * texture.width + x;
                    
                    var ySpawn = y + yPosition;
                    if (ySpawn >= Height) return;

                    var pixel = pixels[index];
                    
                    if (pixel == ColorPickupRuby)
                    {
                        GeneratePickUp(ruby,x,y);
                    }
                    else if (pixel == ColorPickupStar)
                    {
                        GeneratePickUp(star,x,y);
                    }
                    else if(pixel != Color.white) _grid[x, ySpawn] = 2;
                }
            }
        }

        private void GeneratePickUp(BasePickup pickup, int gridX, int gridY)
        {
            // calculate grid position to world position
            // spawn the pickup on that position
            var pos = GridToWorldPosition(gridX, gridY);
            Instantiate(pickup, pos, Quaternion.identity);
        }

        private Vector3 GridToWorldPosition(int x, int y)
        {
            // TODO: Implement this
            return  Vector3.zero;
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

        public void UpdatePixels(float moverYPos, int x)
        {
            var y = Mathf.Abs(Mathf.Round(Mathf.Min(1, moverYPos * updateAccuracy)));
            var expectedLine = (int) (Height * (y / (updateAccuracy * 100)));

            if (_currentLine >= expectedLine)
            {
                // tear has reached the end, level is completed
            }
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
                    if (_grid[x, y] == 0 || _grid[x,y] == 1)
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

            if (_grid[x, y] != 0)
            {
                _colors[colorIndex] = ColorFilled;
            }
        }
    }
}