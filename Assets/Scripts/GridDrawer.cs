using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateBefore(typeof(InitializeSystem))]
public class GridDrawer : MonoBehaviour
{
    private Texture2D gridTexture;
    public int gridSize = 1000;

    public static GridDrawer Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        gridTexture = new Texture2D(gridSize, gridSize);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gridTexture.SetPixel(i, j, Color.black);
            }
        }
        gridTexture.filterMode = FilterMode.Point;
        gridTexture.Apply();

        Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void setGridSize(int newSize)
    {
        gridSize = newSize;
        gridTexture = new Texture2D(gridSize, gridSize);
        gridTexture.filterMode = FilterMode.Point;
    }

    public void SetPixel(int x, int y, Color color)
    {
        gridTexture.SetPixel(x, y, color);
        gridTexture.Apply();
        Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetPixel(Color[] color)
    {
        if(color.Length == gridSize * gridSize)
        {
            gridTexture.SetPixels(color);
            gridTexture.Apply();
            Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}
