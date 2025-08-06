// Yutaka ReiRoku
using System.Xml.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement root;
    private int transOrder = 1;
    private VisualElement currentPanel;

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

        currentPanel = loginForm;


        var startButton = root.Q<Button>("StartButton");
        var shopButton = root.Q<Button>("ShopButton");
        var achievementsButton = root.Q<Button>("AchievementsButton");
        var questsButton = root.Q<Button>("QuestsButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var exitButton = root.Q<Button>("ExitButton");
        var confirmExitButton = root.Q<Button>("ConfirmExitButton");
        var cancelExitButton = root.Q<Button>("CancelExitButton");
        var levelButtons = root.Query<Button>("LevelButton").ToList();


        startButton.RegisterCallback<ClickEvent>(evt => ShowPanel(levelSelectPanel, 3));
        shopButton.RegisterCallback<ClickEvent>(evt => ShowPanel(shopPanel, 4));
        achievementsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(achievementsPanel, 5));
        questsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(questsPanel, 6));
        settingsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(settingsPanel, 7));
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
        root.Q("SwitchToRegister").RegisterCallback<ClickEvent>(evt => ShowPanel(registerForm, 1));
        root.Q("SwitchToLogin").RegisterCallback<ClickEvent>(evt => ShowPanel(loginForm, 2));
        Debug.Log(levelButtons.Count);
        foreach (var button in levelButtons)
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                SceneManager.LoadScene(1);

            });
        }
    }

    private void ShowPanel(VisualElement panelToShow, int order)
    {

        if (order > transOrder)
        {
            panelToShow.AddToClassList("content-view--insta-bottom");
            panelToShow.schedule.Execute(() =>
            {
                panelToShow.RemoveFromClassList("panel-hidden");
                panelToShow.RemoveFromClassList("content-view--insta-bottom");
            });
            currentPanel.AddToClassList("content-view--above");
            currentPanel.RegisterCallback<TransitionEndEvent>(evt =>
            {
                currentPanel.AddToClassList("panel-hidden");
                currentPanel.RemoveFromClassList("content-view--above");
                currentPanel = panelToShow;
                transOrder = order;
            });
        }
        else if (order < transOrder)
        {
            panelToShow.AddToClassList("content-view--insta-above");
            panelToShow.schedule.Execute(() =>
            {
                panelToShow.RemoveFromClassList("panel-hidden");
                panelToShow.RemoveFromClassList("content-view--insta-above");
            });
            currentPanel.AddToClassList("content-view--bottom");
            currentPanel.RegisterCallback<TransitionEndEvent>(evt =>
            {
                currentPanel.AddToClassList("panel-hidden");
                currentPanel.RemoveFromClassList("content-view--bottom");
                currentPanel = panelToShow;
                transOrder = order;
            });
        }
    }
}