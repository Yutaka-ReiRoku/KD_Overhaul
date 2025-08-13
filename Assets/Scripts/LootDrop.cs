// LootDrop.cs
using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public CollectibleData collectibleData;

    [Tooltip("Trọng số rơi. Càng cao, tỷ lệ càng lớn so với các vật phẩm khác.")]
    public float dropWeight = 100f;
}