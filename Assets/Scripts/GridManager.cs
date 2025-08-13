
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 5;
    [SerializeField] private GameObject parent;

    [Header("Dependencies")]
    [SerializeField] private Tile tilePrefab;

    private BoxCollider2D gridArea;

    void Awake()
    {
        gridArea = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Bounds bounds = gridArea.bounds;
        float tileWidth = bounds.size.x / width;
        float tileHeight = bounds.size.y / height;

        foreach (Transform child in transform) { Destroy(child.gameObject); }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xPos = bounds.min.x + (x * tileWidth) + (tileWidth / 2f);
                float yPos = bounds.min.y + (y * tileHeight) + (tileHeight / 2f);
                Vector3 tilePosition = new Vector3(xPos, yPos, 0);

                Tile newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                newTile.name = $"Tile {x}_{y}";
                newTile.transform.parent = parent.transform;
                newTile.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);

            }
        }
        Destroy(gameObject);
    }

}