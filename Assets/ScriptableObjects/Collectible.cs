// Collectible.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Collectible : MonoBehaviour, ICollectible
{
    private CollectibleData data;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// </summary>
    public void Initialize(CollectibleData collectibleData)
    {
        this.data = collectibleData;

        spriteRenderer.sprite = data.itemSprite;
        boxCollider.size = data.colliderSize;
        Destroy(gameObject, data.despawnTime);
    }

    public void Collect()
    {
        if (data == null) return;

        CurrencyManager.Instance.AddCurrency(data.currencyValue);
        Destroy(gameObject);
    }
}