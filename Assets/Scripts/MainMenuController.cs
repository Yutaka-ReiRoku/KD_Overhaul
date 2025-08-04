// Yutaka ReiRoku
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement root;

    private VisualElement loginForm;
    private VisualElement registerForm;
    private VisualElement levelSelectPanel;
    private VisualElement shopPanel;
    private VisualElement achievementsPanel;
    private VisualElement questsPanel;
    private VisualElement settingsPanel;
    private VisualElement exitConfirmationOverlay;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        loginForm = root.Q("LoginForm");
        registerForm = root.Q("RegisterForm");
        levelSelectPanel = root.Q("LevelSelectPanel");
        shopPanel = root.Q("ShopPanel");
        achievementsPanel = root.Q("AchievementsPanel");
        questsPanel = root.Q("QuestsPanel");
        settingsPanel = root.Q("SettingsPanel");
        exitConfirmationOverlay = root.Q("ExitConfirmationOverlay");



        var startButton = root.Q<Button>("StartButton");
        var shopButton = root.Q<Button>("ShopButton");
        var achievementsButton = root.Q<Button>("AchievementsButton");
        var questsButton = root.Q<Button>("QuestsButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var exitButton = root.Q<Button>("ExitButton");
        var confirmExitButton = root.Q<Button>("ConfirmExitButton");
        var cancelExitButton = root.Q<Button>("CancelExitButton");


        startButton.RegisterCallback<ClickEvent>(evt => ShowPanel(levelSelectPanel));
        shopButton.RegisterCallback<ClickEvent>(evt => ShowPanel(shopPanel));
        achievementsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(achievementsPanel));
        questsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(questsPanel));
        settingsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(settingsPanel));
        exitButton.RegisterCallback<ClickEvent>(evt =>
        {
            exitConfirmationOverlay.RemoveFromClassList("panel-hidden");
        });
        confirmExitButton.RegisterCallback<ClickEvent>(evt =>
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        });
        cancelExitButton.RegisterCallback<ClickEvent>(evt =>
        {
            exitConfirmationOverlay.AddToClassList("panel-hidden");
        });
        root.Q("SwitchToRegister").RegisterCallback<ClickEvent>(evt => ShowPanel(registerForm));
        root.Q("SwitchToLogin").RegisterCallback<ClickEvent>(evt => ShowPanel(loginForm));
    }

    private void ShowPanel(VisualElement panelToShow)
    {
        loginForm.AddToClassList("panel-hidden");
        registerForm.AddToClassList("panel-hidden");
        levelSelectPanel.AddToClassList("panel-hidden");
        shopPanel.AddToClassList("panel-hidden");
        achievementsPanel.AddToClassList("panel-hidden");
        questsPanel.AddToClassList("panel-hidden");
        settingsPanel.AddToClassList("panel-hidden");

        panelToShow.RemoveFromClassList("panel-hidden");
    }
}