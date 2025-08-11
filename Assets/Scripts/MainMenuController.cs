// Yutaka ReiRoku
using System.Xml.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement root;

    private int transOrder = 0;
    private VisualElement currentPanel;

    private VisualElement[] allPanels;

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

        allPanels = new VisualElement[] {
            loginForm, registerForm, levelSelectPanel, shopPanel,
            achievementsPanel, questsPanel, settingsPanel
        };

        currentPanel = null;

        var startButton = root.Q<Button>("StartButton");
        var shopButton = root.Q<Button>("ShopButton");
        var achievementsButton = root.Q<Button>("AchievementsButton");
        var questsButton = root.Q<Button>("QuestsButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var exitButton = root.Q<Button>("ExitButton");
        var confirmExitButton = root.Q<Button>("ConfirmExitButton");
        var cancelExitButton = root.Q<Button>("CancelExitButton");
        var navPanel = root.Q<VisualElement>("NavPanel");
        var loginButton = root.Q<Button>("LoginSubmitButton");
        var logoutButton = root.Q<Button>("LogoutButton");
        var levelButtons = root.Query<Button>("LevelButton").ToList();

        startButton.RegisterCallback<ClickEvent>(evt => ShowPanel(levelSelectPanel, 3));
        shopButton.RegisterCallback<ClickEvent>(evt => ShowPanel(shopPanel, 4));
        achievementsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(achievementsPanel, 5));
        questsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(questsPanel, 6));
        settingsButton.RegisterCallback<ClickEvent>(evt => ShowPanel(settingsPanel, 7));

        root.Q("SwitchToRegister").RegisterCallback<ClickEvent>(evt => ShowPanel(registerForm, 2));
        root.Q("SwitchToLogin").RegisterCallback<ClickEvent>(evt => ShowPanel(loginForm, 1));

        exitButton.RegisterCallback<ClickEvent>(evt => exitConfirmationOverlay.RemoveFromClassList("panel-hidden"));
        confirmExitButton.RegisterCallback<ClickEvent>(evt => Application.Quit());
        cancelExitButton.RegisterCallback<ClickEvent>(evt => exitConfirmationOverlay.AddToClassList("panel-hidden"));


        loginButton.RegisterCallback<ClickEvent>(evt => { navPanel.RemoveFromClassList("nav-panel-hidden"); ShowPanel(levelSelectPanel, 3); });
        logoutButton.RegisterCallback<ClickEvent>(evt => { navPanel.AddToClassList("nav-panel-hidden"); ShowPanel(loginForm, 1); });


        foreach (var button in levelButtons)
        {
            var capturedButton = button;
            capturedButton.RegisterCallback<ClickEvent>(evt => {
                SceneManager.LoadScene(1);
            });
        }
        root.RegisterCallback<GeometryChangedEvent>(OnUIGeometryChanged);

    }

    private void OnUIGeometryChanged(GeometryChangedEvent evt)
    {
        root.schedule.Execute(() => ShowPanel(loginForm, 1)).ExecuteLater(1000);

        root.UnregisterCallback<GeometryChangedEvent>(OnUIGeometryChanged);
    }
    private void ShowPanel(VisualElement panelToShow, int order)
    {
        if (currentPanel == panelToShow)
        {
            return;
        }

        foreach (var panel in allPanels)
        {
            panel.AddToClassList("panel-hidden");
            panel.RemoveFromClassList("content-view--above");
            panel.RemoveFromClassList("content-view--bottom");
            panel.RemoveFromClassList("content-view--insta-above");
            panel.RemoveFromClassList("content-view--insta-bottom");
        }

        bool isGoingForward = order > transOrder;

        if (currentPanel != null)
        {
            currentPanel.RemoveFromClassList("panel-hidden");
            if (isGoingForward)
            {
                currentPanel.AddToClassList("content-view--above");
            }
            else
            {
                currentPanel.AddToClassList("content-view--bottom");
            }
        }

        panelToShow.RemoveFromClassList("panel-hidden");

        if (isGoingForward)
        {
            panelToShow.AddToClassList("content-view--insta-bottom");
        }
        else
        {
            panelToShow.AddToClassList("content-view--insta-above");
        }

        panelToShow.schedule.Execute(() =>
        {
            if (isGoingForward)
            {
                panelToShow.RemoveFromClassList("content-view--insta-bottom");
            }
            else
            {
                panelToShow.RemoveFromClassList("content-view--insta-above");
            }
        });

        currentPanel = panelToShow;
        transOrder = order;
    }
}