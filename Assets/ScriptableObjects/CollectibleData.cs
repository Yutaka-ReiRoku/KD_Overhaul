// CollectibleData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectibleData", menuName = "Tower Defense/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    [Header("Display")]
    public Sprite itemSprite;
    public Vector2 colliderSize = new Vector2(0.5f, 0.5f);

    [Header("Gameplay")]
    public int currencyValue;
    public float despawnTime = 8f;
}