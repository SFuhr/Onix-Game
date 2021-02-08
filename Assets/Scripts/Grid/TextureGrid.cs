using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using Random = UnityEngine.Random;

namespace Grid
{
    public class TextureGrid : MonoBehaviour
    {
        [Header("Mover")]
        [SerializeField] private int moverSize = 16;
        [SerializeField] private int moverHeight = 8;
        
        [Header("Colors")]
        [SerializeField] private Color colorEmpty = new Color(1,1,1,1);
        [SerializeField] private Color colorFilled = new Color(0,0,0,1);
        
        private int Width => _texture.width;
        private int Height => _texture.height;

        private int[,] _grid;
        private Renderer _renderer;
        private Texture2D _texture;
        private Color[] _colors;
        private int _moverFirstPoint;

        private void OnEnable()
        {
            _renderer = GetComponent<Renderer>();
            _texture = Instantiate(_renderer.material.mainTexture) as Texture2D;
            _renderer.material.mainTexture = _texture;
            _colors = new Color[Width * Height];
            
            Debug.Log($"Width = {Width.ToString()}; Heigth = {Height.ToString()}");
            
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            _grid = new int[Width,Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _grid[x, y] = 0;
                }
            }
            moverSize = Mathf.Clamp(moverSize % 2 == 0 ? moverSize : moverSize + 1,0,Width);

            var size = moverSize;
            var startPoint = Width / 2 - size / 2 + 1;
            
            Debug.Log($"Start point: {startPoint.ToString()}; Mover size = {size.ToString()}");
            
            for (int x = 0; x < Width; x++)
            {
                if(x < startPoint) continue;
                if (size > 0)
                {
                    _grid[x, 0] = 1;
                    size--;
                }
            }
            
            // applying texture colors
            // for (int x = 0; x < Width; x++)
            // {
            //     for (int y = 0; y < Height; y++)
            //     {
            //         var clrIndex = x > 0 ? x : 1;
            //         clrIndex *= y > 0 ? y : 1;
            //         
            //         _colors[clrIndex] = _grid[x, y] == 0 ? White : Black;
            //     }
            // }
            for (int x = 0; x < Width; x++)
            {
                _colors[x] = _grid[x, 0] == 0 ? colorEmpty : colorFilled;
            }
            
            _texture.SetPixels(_colors);
            _texture.Apply();
        }

        // private void Start()
        // {
            // Renderer rend = GetComponent<Renderer>();

            // duplicate the original texture and assign to the material
            // Texture2D texture = Instantiate(rend.material.mainTexture) as Texture2D;
            // rend.material.mainTexture = texture;

            // colors used to tint the first 3 mip levels
            // Color[] colors = new Color[3];
            // colors[0] = Color.red;
            // colors[1] = Color.green;
            // colors[2] = Color.blue;
            //
            // Debug.Log(_texture.width + " " + _texture.height);
            // for (int x = 0; x < 128; x++)
            // {
            //     Color[] cols = _texture.GetPixels(_texture.mipmapCount);
            //     for (int y = 0; y < 1024; y++)
            //     {
            //         // texture.SetPixel(x,y,colors[Random.Range(0,3)]);
            //         cols[y] = colors[Random.Range(0, 3)];
            //     }
            //     _texture.SetPixels(cols);
            // }
            // _texture.Apply();
            //
            // int mipCount = Mathf.Min(1, _texture.mipmapCount);
            // StartCoroutine(DrawOnTexture(colors,mipCount));
            // // tint each mip level
            // for (int mip = 0; mip < mipCount; ++mip)
            // {
            //     Color[] cols = texture.GetPixels(mip);
            //     for (int i = 0; i < cols.Length; ++i)
            //     {
            //         // cols[i] = Color.Lerp(cols[i], colors[mip], 0.33f);
            //         cols[i] = colors[Random.Range(0,3)];
            //         // cols[i] = Color.Lerp(Black, White, 1 / (float)i);
            //     }
            //     texture.SetPixels(cols, mip);
            //     texture.Apply();
            // }
            // // actually apply all SetPixels, don't recalculate mip levels
            // // texture.Apply(false);
        // }

        // IEnumerator DrawOnTexture(Color[] colors, int minCount)
        // {
        //     int mip = 0;
        //     while (mip < minCount)
        //     {
        //         Color[] cols = _texture.GetPixels(mip);
        //         for (int i = 0; i < cols.Length; ++i)
        //         {
        //             int id = Random.Range(0, 3);
        //             cols[i] = colors[id];
        //             cols[i] = Color.Lerp(Black, White, 1 / Random.Range(0,1.01f));
        //             if (i == cols.Length - 1)
        //             {
        //                 Debug.Log(i);
        //             }
        //         }
        //
        //         Debug.Log(mip + " ");
        //         yield return null;
        //         _texture.SetPixels(cols, mip);
        //         _texture.Apply();
        //         mip++;
        //     }
        // }
    }
}