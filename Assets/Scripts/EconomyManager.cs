
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EconomyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyManager currencyManager;
    [Tooltip("Prefab chung cho TẤT CẢ các vật phẩm rơi ra")]
    [SerializeField] private Collectible genericCollectiblePrefab;

    [Header("Loot Table")]
    [SerializeField] private List<LootDrop> lootTable;

    [Header("Overall Drop Chance & DDA")]
    [SerializeField][Range(0f, 1f)] private float overallLootChance = 0.5f;
    [SerializeField] private int lowCurrencyThreshold = 100;
    [SerializeField][Range(0f, 1f)] private float lowCurrencyDropBonus = 0.25f;

    private void OnEnable() { EnemyBase.OnStaticDeath += OnEnemyDefeated; }
    private void OnDisable() { EnemyBase.OnStaticDeath -= OnEnemyDefeated; }

    private void OnEnemyDefeated(EnemyBase enemy)
    {
        float finalOverallChance = overallLootChance;
        if (currencyManager.GetCurrentCurrency() < lowCurrencyThreshold)
        {
            finalOverallChance += lowCurrencyDropBonus;
        }

        if (Random.value > finalOverallChance) return;


        float totalWeight = lootTable.Sum(drop => drop.dropWeight);
        float randomPoint = Random.Range(0, totalWeight);

        foreach (var drop in lootTable)
        {
            if (randomPoint <= drop.dropWeight)
            {
                Collectible newCollectible = Instantiate(genericCollectiblePrefab, enemy.transform.position, Quaternion.identity);

                newCollectible.Initialize(drop.collectibleData);

                return;
            }
            else
            {
                randomPoint -= drop.dropWeight;
            }
        }
    }
}