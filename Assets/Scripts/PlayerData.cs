// PlayerData.cs
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string email;
    public string password;


    public List<string> ownedTowerIDs;
    public int persistentCoins;

    public int maxLevelReached = 1;

    public PlayerData()
    {
        ownedTowerIDs = new List<string>();
        persistentCoins = 0;
    }
}