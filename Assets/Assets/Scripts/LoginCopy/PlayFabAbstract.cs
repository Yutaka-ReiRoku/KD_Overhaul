using PlayFab;
using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayFabAbstract : HieuMonoBehaviour
{
    public string message;
#region Conditions
    protected virtual bool CheckEmail(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            this.Message("Please enter complete information");
            return false;
        }
        if (this.EmailHasDiacritics(userEmail))
        {
            this.Message("Email contains accents, please enter without accents");
            return false;
        }
        if (!userEmail.Contains("@gmail.com"))
        {
            this.Message("Email must contain @gmail.com");
            return false;
        }
        return true;
    }
    protected virtual bool CheckUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            this.Message("Please enter complete information");
            return false;
        }
        return true;
    }
    protected virtual bool CheckPassword(string userPassword)
    {
        if (this.PassHasDiacritics(userPassword))
        {
            this.Message("Password contains accents, please enter without accents");
            return false;
        }
        if (userPassword.Length < 6)
        {
            this.Message("Password must be at least 6 long");
            return false;
        }
        return true;
    }
    protected virtual bool EmailHasDiacritics(string userEmail)
    {
        string nomalized = userEmail.Normalize(System.Text.NormalizationForm.FormD);
        foreach (char child in nomalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(child) == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                return true;
            }
        }
        return false;
    }
    protected virtual bool PassHasDiacritics(string pass)
    {
        string nomalized = pass.Normalize(System.Text.NormalizationForm.FormD);
        foreach (char c in nomalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                return true;
            }
        }
        return false;
    }
#endregion
    protected virtual void OnError(PlayFabError error)
    {
        switch (error.Error)
        {
            #region MessageRegister
            case PlayFabErrorCode.EmailAddressNotAvailable:
                this.Message("Email already in use");
                break;
            case PlayFabErrorCode.InvalidEmailAddress:
                this.Message("Email is not valid");
                break;
            case PlayFabErrorCode.UsernameNotAvailable:
                this.Message("Username already exists");
                break;                            
            case PlayFabErrorCode.InvalidUsername:
                this.Message("Username is invalid");
                break;
            case PlayFabErrorCode.NameNotAvailable:
                this.Message("Display name already taken");
                break;
            case PlayFabErrorCode.InvalidParams:
                this.Message("One or more fields are invalid");
                break;
            #endregion           
            case PlayFabErrorCode.AccountNotFound:
                this.Message("Account not found");
                break;
            case PlayFabErrorCode.InvalidPassword:
                this.Message("Invalid password");
                break;                        
            default:
                Debug.Log(error.GenerateErrorReport());
                break;
        }
    }

    protected virtual void Message(string message)
    {
        Debug.Log("Message : " + message);
        this.message = message;        
    }
    public virtual void ResultMessage(Label label)
    {
        StopCoroutine(ResetMessage(label));
        label.text = this.message;
        label.style.display = DisplayStyle.Flex;
        StartCoroutine(ResetMessage(label));        
    }    
    protected IEnumerator ResetMessage(Label label)
    {
        yield return new WaitForSeconds(3);
        label.text = string.Empty;
    }
    
}
