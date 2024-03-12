using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridDrawer : MonoBehaviour
{
    private int gridSize;
    private Texture2D gridTexture;

    public static GridDrawer Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gridSize = GridAuthoring.gridSize;

        transform.position = new Vector3(gridSize/2, gridSize/2, 0);
        transform.localScale = new Vector3(gridSize, gridSize, 1);
        Camera.main.transform.position = new Vector3(gridSize / 2, gridSize / 2, -10);

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

    public void SetPixel(int x, int y, Color color)
    {
        gridTexture.SetPixel(x, y, color);
        gridTexture.Apply();
        Sprite sprite = Sprite.Create(gridTexture, new Rect(0.0f, 0.0f, gridTexture.width, gridTexture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

}
