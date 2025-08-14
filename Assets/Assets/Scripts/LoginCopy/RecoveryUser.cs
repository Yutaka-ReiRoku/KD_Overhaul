using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecoveryUser : PlayFabAbstract
{
    [SerializeField] protected bool recoveryIsSuccess;
    public bool RecoveryIsSuccess => recoveryIsSuccess;
    public virtual void Recovery(string userEmail)
    {       
        if (string.IsNullOrEmpty(userEmail))
        {
            this.Message("Please enter complete information");
            return;
        }
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = userEmail,
            TitleId = "8D662"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnRecoverySucces, OnErrol);
    }

    private void OnRecoverySucces(SendAccountRecoveryEmailResult result)
    {
        this.recoveryIsSuccess = true;
        this.Message("Recovery in successfully");
    }

    private void OnErrol(PlayFabError error)
    {
        base.OnError(error);
        this.recoveryIsSuccess = false;        
    }    
}
