// CurrentPlayerDataSO.cs
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "CurrentPlayerData", menuName = "Tower Defense/Current Player Data")]
public class CurrentPlayerDataSO : ScriptableObject
{
    [Header("Player Info")]
    public string playerName;

    [Header("Player Stats")]
    public int persistentCoins;
    public List<string> ownedTowerIDs;

    public int maxLevelReached;


    public event Action OnDataLoaded;

    public void LoadFrom(PlayerData data)
    {
        this.playerName = data.playerName;
        this.persistentCoins = data.persistentCoins;

        this.maxLevelReached = data.maxLevelReached;

        this.ownedTowerIDs = new List<string>(data.ownedTowerIDs);

        Debug.Log($"Data loaded into SO for player: {playerName}");

        OnDataLoaded?.Invoke();
    }

    public PlayerData GetSaveData()
    {
        return new PlayerData
        {
            playerName = this.playerName,
            persistentCoins = this.persistentCoins,

            maxLevelReached = this.maxLevelReached,

            ownedTowerIDs = this.ownedTowerIDs
        };
    }
}