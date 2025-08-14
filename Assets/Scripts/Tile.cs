
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color removeHoverColor;

    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    // References
    private GameUIManager gameUIManager;

    // State
    public bool IsOccupied { get; private set; } = false;
    public TowerBase TowerOnTile { get; private set; }


    public void Start()
    {
        this.gameUIManager = FindAnyObjectByType<GameUIManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    public void Initialize(GameUIManager manager)
    {
        this.gameUIManager = manager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void HandlePointerEnter()
    {
        if (gameUIManager.IsShovelSelected())
        {
            if (IsOccupied)
            {
                spriteRenderer.color = removeHoverColor;
            }
        }
        else if (gameUIManager.IsHoldingTower())
        {
            if (!IsOccupied)
            {
                spriteRenderer.color = hoverColor;
            }
        }
    }

    public void HandlePointerExit()
    {
        spriteRenderer.color = originalColor;
    }

    public void HandleClick()
    {
        gameUIManager.OnTileClicked(this);
    }

    public void SetOccupied(TowerBase tower, TowerData towerData)
    {
        IsOccupied = true;
        TowerOnTile = tower;
        spriteRenderer.color = originalColor;
        originalColor = spriteRenderer.color;
    }
    public void SetEmpty()
    {
        IsOccupied = false;
        TowerOnTile = null;
        spriteRenderer.color = originalColor;
    }
}