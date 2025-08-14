using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class LoginManager : HieuSingleton<LoginManager>
{
    [SerializeField] protected LoginUser loginUser;
    public LoginUser LoginUser => loginUser;

    [SerializeField] protected RegisterUser registerUser;
    public RegisterUser RegisterUser => registerUser;

    [SerializeField] protected RecoveryUser recoveryUser;
    public RecoveryUser RecoveryUser => recoveryUser;


    protected override void LoadComponents()
    {
        base.LoadComponents();    
        this.LoadLoginUser();
        this.LoadRegisterUser();
        this.LoadRecoveryUser();        
    }
    protected virtual void LoadLoginUser()
    {
        if (this.loginUser != null) return;
        this.loginUser = GetComponentInChildren<LoginUser>();
    }
    protected virtual void LoadRegisterUser()
    {
        if (this.registerUser != null) return;
        this.registerUser = GetComponentInChildren<RegisterUser>();
    }
    protected virtual void LoadRecoveryUser()
    {
        if (this.recoveryUser != null) return;
        this.recoveryUser = GetComponentInChildren<RecoveryUser>();
    }    
}
