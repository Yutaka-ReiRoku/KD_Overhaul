using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using UnityEditor.PackageManager;
using Unity.VisualScripting;

public class RegisterUser : PlayFabAbstract
{
    [SerializeField] protected bool registerIsSuccess;
    public bool RegisterIsSuccess => registerIsSuccess;    
    public virtual void Register(string userName, string userEmail,string userPassword)
    {
        if (!this.CheckUserName(userName)) return;
        if (!this.CheckEmail(userEmail)) return;
        if (!this.CheckPassword(userPassword)) return;
        var request = new RegisterPlayFabUserRequest
        {
            Username = userName,
            DisplayName = userName,
            Email = userEmail,
            Password = userPassword,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnregisterSucces, OnError);    
    }

    protected void OnregisterSucces(RegisterPlayFabUserResult result)
    {
        this.Message("Register in successfully");
        this.registerIsSuccess = true;        
    }
    protected override void OnError(PlayFabError error)
    {
        base.OnError(error);
        this.registerIsSuccess = false;        
    }               
}
