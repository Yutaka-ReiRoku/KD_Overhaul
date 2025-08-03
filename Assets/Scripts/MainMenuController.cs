// Yutaka ReiRoku
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : IScreenController
{
    public VisualElement Root { get; private set; }

    public void Initialize(VisualElement rootElement)
    {
        Root = rootElement;
        Root.AddToClassList("mainscreen");

        var UsernameInput = Root.Q<TextField>("UsernameInput");
        var PasswordInput = Root.Q<TextField>("PasswordInput");
        var LoginButton = Root.Q<Button>("LoginButton");
        var SignUpButton = Root.Q<Button>("SignUpButton");
        var PlayButton = Root.Q<Button>("PlayButton");
        var ShopButton = Root.Q<Button>("ShopButton");
        var AchievementButton = Root.Q<Button>("AchievementButton");
        var SettingButton = Root.Q<Button>("SettingButton");
        var LogoutButton = Root.Q<Button>("LogoutButton");

        if (GameManager.Instance.Loggedin == true)
        {
            UsernameInput.AddToClassList("hidden");
            PasswordInput.AddToClassList("hidden");
            LoginButton.AddToClassList("shrink");
            SignUpButton.AddToClassList("shrink");
            PlayButton.RemoveFromClassList("shrink");
            ShopButton.RemoveFromClassList("shrink");
            AchievementButton.RemoveFromClassList("shrink");
            SettingButton.RemoveFromClassList("shrink");
            LogoutButton.RemoveFromClassList("shrink");
        }

        LoginButton?.RegisterCallback<ClickEvent>(evt =>
        {
            GameManager.Instance.Loggedin = true;
            UsernameInput.AddToClassList("hidden");
            PasswordInput.AddToClassList("hidden");
            LoginButton.AddToClassList("shrink");
            SignUpButton.AddToClassList("shrink");
            PlayButton.RemoveFromClassList("shrink");
            ShopButton.RemoveFromClassList("shrink");
            AchievementButton.RemoveFromClassList("shrink");
            SettingButton.RemoveFromClassList("shrink");
            LogoutButton.RemoveFromClassList("shrink");
        }
        );
        SignUpButton?.RegisterCallback<ClickEvent>(evt =>
        {
            GameManager.Instance.Loggedin = true;
            UsernameInput.AddToClassList("hidden");
            PasswordInput.AddToClassList("hidden");
            LoginButton.AddToClassList("shrink");
            SignUpButton.AddToClassList("shrink");
            PlayButton.RemoveFromClassList("shrink");
            ShopButton.RemoveFromClassList("shrink");
            AchievementButton.RemoveFromClassList("shrink");
            SettingButton.RemoveFromClassList("shrink");
            LogoutButton.RemoveFromClassList("shrink");
        }
        );
        PlayButton?.RegisterCallback<ClickEvent>(evt => { });
        ShopButton?.RegisterCallback<ClickEvent>(evt => UIManager.Instance.ShowScreen("Shop", "mainscreen--right", "mainscreen--left"));
        AchievementButton?.RegisterCallback<ClickEvent>(evt => UIManager.Instance.ShowScreen("Achievement", "mainscreen--left", "mainscreen--right"));
        SettingButton?.RegisterCallback<ClickEvent>(evt => UIManager.Instance.ShowScreen("Setting", "mainscreen--bottom", "mainscreen--above"));
        LogoutButton?.RegisterCallback<ClickEvent>(evt =>
        {
            GameManager.Instance.Loggedin = false;
            UsernameInput.RemoveFromClassList("hidden");
            PasswordInput.RemoveFromClassList("hidden");
            LoginButton.RemoveFromClassList("shrink");
            SignUpButton.RemoveFromClassList("shrink");
            PlayButton.AddToClassList("shrink");
            ShopButton.AddToClassList("shrink");
            AchievementButton.AddToClassList("shrink");
            SettingButton.AddToClassList("shrink");
            LogoutButton.AddToClassList("shrink");
        }
        );
    }
}