using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateBefore(typeof(InitializeSystem))]
public class GridDrawer : MonoBehaviour
{
    private Texture2D gridTexture;
    public int gridSize = 1000;

    public GameObject background;

    public static GridDrawer Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        gridTexture = new Texture2D(gridSize, gridSize, TextureFormat.Alpha8, false);
        gridTexture.filterMode = FilterMode.Point;
        gridTexture.Apply();
        Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;

        Texture2D backgroundTexture = new Texture2D(gridSize, gridSize);
        backgroundTexture.Apply();
        Sprite backgroundSprite = Sprite.Create(backgroundTexture, new Rect(0.0f, 0.0f, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
        background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
    }

    public void setGridSize(int newSize)
    {
        gridSize = newSize;
        gridTexture = new Texture2D(gridSize, gridSize, TextureFormat.Alpha8, false);
        gridTexture.filterMode = FilterMode.Point;
        Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;

        Texture2D backgroundTexture = new Texture2D(gridSize, gridSize);
        backgroundTexture.Apply();
        Sprite backgroundSprite = Sprite.Create(backgroundTexture, new Rect(0.0f, 0.0f, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
        background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
    }

    public void SetPixels(NativeArray<byte> color)
    {
        gridTexture.LoadRawTextureData<byte>(color);
        gridTexture.Apply();
    }
}
