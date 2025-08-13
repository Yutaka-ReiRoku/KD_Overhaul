using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string PROFILE_LIST_FILENAME = "profiles.json";

    // Profile List Management
    public static void SaveProfileList(List<string> profileNames)
    {
        ProfileList list = new ProfileList { profileNames = profileNames };
        string json = JsonUtility.ToJson(list, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, PROFILE_LIST_FILENAME), json);
    }

    public static List<string> LoadProfileList()
    {
        string path = Path.Combine(Application.persistentDataPath, PROFILE_LIST_FILENAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ProfileList list = JsonUtility.FromJson<ProfileList>(json);
            return list.profileNames;
        }
        return new List<string>();
    }

    // Player Data Management
    private static string GetPathForPlayer(string playerName)
    {
        return Path.Combine(Application.persistentDataPath, $"Player_{playerName}.json");
    }

    public static void Save(PlayerData data, string playerName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPathForPlayer(playerName), json);
    }

    public static PlayerData Load(string playerName)
    {
        string path = GetPathForPlayer(playerName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }

    public static bool DoesProfileExist(string playerName)
    {
        return File.Exists(GetPathForPlayer(playerName));
    }
}