using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabManager : Singleton<PlayFabManager>
{

    public void RegisterUser(string username, string email, string password, Action onSuccess, Action<string> onError)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
            result => {
                Debug.Log($"PlayFab account for {username} created. Invoking success callback.");

                onSuccess?.Invoke();
            },
            error => {
                Debug.LogError($"PlayFab registration failed: {error.ErrorMessage}");

                onError?.Invoke(error.ErrorMessage);
            }
        );
    }

    public void LoginUser(string username, string password, Action onSuccess, Action<string> onError)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request,
            result => {
                Debug.Log($"PlayFab login for {username} successful. Invoking success callback.");

                onSuccess?.Invoke();
            },
            error => {
                Debug.LogError($"PlayFab login failed: {error.ErrorMessage}");

                onError?.Invoke(error.ErrorMessage);
            }
        );
    }


    public void SavePlayerData(PlayerData data, Action onSuccess, Action<string> onError)
    {
        string jsonData = JsonUtility.ToJson(data);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerData", jsonData }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => {
                Debug.Log("Player data saved to PlayFab successfully!");
                onSuccess?.Invoke();
            },
            error => {
                onError?.Invoke(error.ErrorMessage);
            }
        );
    }

    public void LoadPlayerData(Action<PlayerData> onSuccess, Action<string> onError)
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data != null && result.Data.ContainsKey("PlayerData"))
                {
                    string jsonData = result.Data["PlayerData"].Value;
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                    onSuccess?.Invoke(playerData);
                }
                else
                {
                    onSuccess?.Invoke(null);
                }
            },
            error => {
                onError?.Invoke(error.ErrorMessage);
            }
        );
    }
}