// ProfileManager.cs
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    [Header("Data Container")]
    [SerializeField] private CurrentPlayerDataSO currentPlayerSO;

    [Header("Game Scene")]
    [SerializeField] private string levelSelectSceneName = "LevelSelectScene";


    private UIDocument uiDocument;
    private VisualElement loginForm;
    private VisualElement signupForm;

    // Login Fields
    private TextField loginUsernameField;
    private TextField loginPasswordField;
    private Button loginButton;
    private Label loginErrorLabel;

    // Sign Up Fields
    private TextField signupUsernameField;
    private TextField signupEmailField;
    private TextField signupPasswordField;
    private Button signupButton;
    private Label signupErrorLabel;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        loginForm = root.Q<VisualElement>("login-form");
        signupForm = root.Q<VisualElement>("signup-form");

        loginUsernameField = root.Q<TextField>("login-username");
        loginPasswordField = root.Q<TextField>("login-password");
        loginButton = root.Q<Button>("login-button");
        loginErrorLabel = root.Q<Label>("login-error-label");

        signupUsernameField = root.Q<TextField>("signup-username");
        signupEmailField = root.Q<TextField>("signup-email");
        signupPasswordField = root.Q<TextField>("signup-password");
        signupButton = root.Q<Button>("signup-button");
        signupErrorLabel = root.Q<Label>("signup-error-label");

        var switchToSignupLink = root.Q<Label>("switch-to-signup");
        var switchToLoginLink = root.Q<Label>("switch-to-login");

        loginButton.RegisterCallback<ClickEvent>(OnLoginClicked);
        signupButton.RegisterCallback<ClickEvent>(OnSignUpClicked);
        switchToSignupLink.RegisterCallback<ClickEvent>(evt => SwitchForms(false));
        switchToLoginLink.RegisterCallback<ClickEvent>(evt => SwitchForms(true));
    }

    private void SwitchForms(bool showLogin)
    {
        loginForm.style.display = showLogin ? DisplayStyle.Flex : DisplayStyle.None;
        signupForm.style.display = showLogin ? DisplayStyle.None : DisplayStyle.Flex;
        loginErrorLabel.style.display = DisplayStyle.None;
        signupErrorLabel.style.display = DisplayStyle.None;
    }

    private void OnSignUpClicked(ClickEvent evt)
    {
        string username = signupUsernameField.value;
        string email = signupEmailField.value;
        string password = signupPasswordField.value;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError(signupErrorLabel, "Username and Password cannot be empty.");
            return;
        }

        if (SaveSystem.DoesProfileExist(username))
        {
            ShowError(signupErrorLabel, "Username already exists.");
            return;
        }

        PlayerData newPlayerData = new PlayerData
        {
            playerName = username,
            email = email,
            password = password,
            persistentCoins = 50
        };

        newPlayerData.ownedTowerIDs.Add("aaa");
        newPlayerData.ownedTowerIDs.Add("bbb");

        SaveSystem.Save(newPlayerData, username);

        List<string> profiles = SaveSystem.LoadProfileList();
        profiles.Add(username);
        SaveSystem.SaveProfileList(profiles);

        Login(username);
    }

    private void OnLoginClicked(ClickEvent evt)
    {
        string username = loginUsernameField.value;
        string password = loginPasswordField.value;

        PlayerData playerData = SaveSystem.Load(username);

        if (playerData == null)
        {
            ShowError(loginErrorLabel, "Username does not exist.");
            return;
        }

        if (playerData.password != password)
        {
            ShowError(loginErrorLabel, "Incorrect password.");
            return;
        }

        Login(username);
    }

    private void Login(string username)
    {
        PlayerData playerData = SaveSystem.Load(username);

        currentPlayerSO.LoadFrom(playerData);

        Debug.Log($"Login successful! Welcome, {username}. Loading game scene...");
        SceneManager.LoadScene(levelSelectSceneName);
    }

    private void ShowError(Label errorLabel, string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}