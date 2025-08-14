using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LoginUser : PlayFabAbstract
{
    [SerializeField] protected bool loginIsSuccess;
    public bool LoginIsSuccess => loginIsSuccess;
    
    public void Login(string emailUser, string passwordUser)
    {        
        //if (this.CheckPassword(passwordUser)) return;
        //var request = new LoginWithEmailAddressRequest
        //{
        //    Email = emailUser,
        //    Password = passwordUser
        //};
        //PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSucces, OnErrol);
        Debug.Log("thanhcon");
        var request = new LoginWithPlayFabRequest
        {            
            Username = emailUser,
            Password = passwordUser
        };
        PlayFabClientAPI.LoginWithPlayFab(request,OnLoginSucces,OnErrol);
    }

    private void OnErrol(PlayFabError error)
    {        
        base.OnError(error);
        this.loginIsSuccess = false;       
    }

    private void OnLoginSucces(LoginResult result)
    {        
        this.Message("Login is successfully");
        this.loginIsSuccess = true;                
    }    
}
