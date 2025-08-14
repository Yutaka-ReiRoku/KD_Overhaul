// Yutaka ReiRoku
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : Singleton<MainMenuController>
{
    private const string PANEL_HIDDEN_CLASS = "panel-hidden";
    private const string NAV_PANEL_HIDDEN_CLASS = "nav-panel-hidden";
    private const string VIEW_ABOVE_CLASS = "content-view--above";
    private const string VIEW_BOTTOM_CLASS = "content-view--bottom";
    private const string INSTA_ABOVE_CLASS = "content-view--insta-above";
    private const string INSTA_BOTTOM_CLASS = "content-view--insta-bottom";

    [System.Serializable]
    public class PanelInfo
    {
        public string Name;
        public int Order;
        [HideInInspector] public VisualElement Element;
    }

    [SerializeField] private CurrentPlayerDataSO currentPlayerSO;

    [SerializeField]
    private List<PanelInfo> panels = new List<PanelInfo>
    {
        new PanelInfo { Name = "LoginForm", Order = 1 },
        new PanelInfo { Name = "RegisterForm", Order = 2 },
        new PanelInfo { Name = "LevelSelectPanel", Order = 3 },
        new PanelInfo { Name = "ShopPanel", Order = 4 },
        new PanelInfo { Name = "AchievementsPanel", Order = 5 },
        new PanelInfo { Name = "QuestsPanel", Order = 6 },
        new PanelInfo { Name = "SettingsPanel", Order = 7 }
    };

    private VisualElement root;
    private VisualElement currentPanel;
    private int currentOrder = 0;

    private VisualElement exitConfirmationOverlay;
    private VisualElement navPanel;
    private VisualElement loadingPanel;


    private TextField loginUsernameField;
    private TextField loginPasswordField;

    private TextField signupUsernameField;
    private TextField signupEmailField;
    private TextField signupPasswordField;

    private Label ErrorLabel;
    private Label SignupErrorLabel;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        InitializeElements();

        RegisterButtonCallbacks();

        root.schedule.Execute(() => loadingPanel.RemoveFromClassList("loading-panel-active")).ExecuteLater(1500);
        root.schedule.Execute(() => ShowPanelByOrder(1)).ExecuteLater(2500);
    }

    private void InitializeElements()
    {
        foreach (var panelInfo in panels)
        {
            panelInfo.Element = root.Q(panelInfo.Name);
        }
        exitConfirmationOverlay = root.Q("ExitConfirmationOverlay");
        navPanel = root.Q("NavPanel");
        loadingPanel = root.Q("LoadingPanel");


        loginUsernameField = root.Q<TextField>("LoginUsername");
        loginPasswordField = root.Q<TextField>("LoginPassword");

        signupUsernameField = root.Q<TextField>("RegisterUsername");
        signupEmailField = root.Q<TextField>("RegisterEmail");
        signupPasswordField = root.Q<TextField>("RegisterPassword");

        ErrorLabel = root.Q<Label>("ErrorLabel");
        SignupErrorLabel = root.Q<Label>("SignupErrorLabel");
    }

    private void RegisterButtonCallbacks()
    {
        root.Q<Button>("StartButton").clicked += () => ShowPanelByOrder(3);
        root.Q<Button>("ShopButton").clicked += () => ShowPanelByOrder(4);
        root.Q<Button>("AchievementsButton").clicked += () => ShowPanelByOrder(5);
        root.Q<Button>("QuestsButton").clicked += () => ShowPanelByOrder(6);
        root.Q<Button>("SettingsButton").clicked += () => ShowPanelByOrder(7);

        root.Q("SwitchToRegister").RegisterCallback<ClickEvent>(evt => ShowPanelByOrder(2));
        root.Q("SwitchToLogin").RegisterCallback<ClickEvent>(evt => ShowPanelByOrder(1));

        root.Q<Button>("ExitButton").clicked += () => exitConfirmationOverlay.RemoveFromClassList(PANEL_HIDDEN_CLASS);
        root.Q<Button>("ConfirmExitButton").clicked += () => Application.Quit();
        root.Q<Button>("CancelExitButton").clicked += () => exitConfirmationOverlay.AddToClassList(PANEL_HIDDEN_CLASS);

        root.Q<Button>("LoginSubmitButton").clicked += OnLoginClicked;
        root.Q<Button>("LogoutButton").clicked += PerformLogout;
        root.Q<Button>("RegisterSubmitButton").clicked += OnSignUpClicked;

        


    }

    public void DisplayLoadingScreen()
    { 
        loadingPanel.RegisterCallback<TransitionEndEvent>(StartGame);
        loadingPanel.AddToClassList("loading-panel-active"); 
    }

    private void StartGame(TransitionEndEvent evt)
    {
        Time.timeScale = 1f;
        loadingPanel.UnregisterCallback<TransitionEndEvent>(StartGame);
        SceneManager.LoadScene(1);
    }
    private void PerformLogin()
    {
        navPanel.RemoveFromClassList(NAV_PANEL_HIDDEN_CLASS);
        ShowPanelByOrder(3);
        loginUsernameField.value = "";
        loginPasswordField.value = "";
        signupUsernameField.value = "";
        signupEmailField.value = "";
        signupPasswordField.value = "";
    }

    private void PerformLogout()
    {
        navPanel.AddToClassList(NAV_PANEL_HIDDEN_CLASS);
        ShowPanelByOrder(1);
    }

    private void ShowPanelByOrder(int order)
    {
        var panelInfo = panels.Find(p => p.Order == order);
        if (panelInfo != null)
        {
            ShowPanel(panelInfo.Element, panelInfo.Order);
        }
    }

    private void ShowPanel(VisualElement panelToShow, int order)
    {
        ErrorLabel.style.display = DisplayStyle.None;
        SignupErrorLabel.style.display = DisplayStyle.None;
        if (currentPanel == panelToShow || panelToShow == null)
        {
            return;
        }

        foreach (var panelInfo in panels)
        {
            panelInfo.Element?.AddToClassList(PANEL_HIDDEN_CLASS);
            panelInfo.Element?.RemoveFromClassList(VIEW_ABOVE_CLASS);
            panelInfo.Element?.RemoveFromClassList(VIEW_BOTTOM_CLASS);
            panelInfo.Element?.RemoveFromClassList(INSTA_ABOVE_CLASS);
            panelInfo.Element?.RemoveFromClassList(INSTA_BOTTOM_CLASS);
        }

        bool isGoingForward = order > currentOrder;

        if (currentPanel != null)
        {
            currentPanel.RemoveFromClassList(PANEL_HIDDEN_CLASS);
            currentPanel.AddToClassList(isGoingForward ? VIEW_ABOVE_CLASS : VIEW_BOTTOM_CLASS);
        }

        panelToShow.RemoveFromClassList(PANEL_HIDDEN_CLASS);
        panelToShow.AddToClassList(isGoingForward ? INSTA_BOTTOM_CLASS : INSTA_ABOVE_CLASS);

        panelToShow.schedule.Execute(() =>
        {
            panelToShow.RemoveFromClassList(isGoingForward ? INSTA_BOTTOM_CLASS : INSTA_ABOVE_CLASS);
        });

        currentPanel = panelToShow;
        currentOrder = order;
    }



    private void OnSignUpClicked()
    {
        string username = signupUsernameField.value;
        string email = signupEmailField.value;
        string password = signupPasswordField.value;
        LoginManager.Instance.RegisterUser.Register(username,email,password);        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))            
        {
            ShowError(SignupErrorLabel, "Username and Password cannot be empty.");
            return;
        }

        if (SaveSystem.DoesProfileExist(username))
        {
            ShowError(SignupErrorLabel, "Username already exists.");
            return;
        }        
        if (!LoginManager.Instance.RegisterUser.RegisterIsSuccess)
        {
            ShowError(SignupErrorLabel, LoginManager.Instance.RegisterUser.message);
            return;
        }
        PlayerData newPlayerData = new PlayerData
        {
            playerName = username,
            email = email,
            password = password,
            persistentCoins = 50
        };

        newPlayerData.ownedTowerIDs.Add("Archer");
        newPlayerData.ownedTowerIDs.Add("Knight");
        newPlayerData.ownedTowerIDs.Add("Soldier");

        newPlayerData.maxLevelReached = 1;

        SaveSystem.Save(newPlayerData, username);

        List<string> profiles = SaveSystem.LoadProfileList();
        profiles.Add(username);
        SaveSystem.SaveProfileList(profiles);

        Login(username);

    }

    private void OnLoginClicked()
    {
        string username = loginUsernameField.value;
        string password = loginPasswordField.value;
        LoginManager.Instance.LoginUser.Login(username,password);      
        PlayerData playerData = SaveSystem.Load(username);        
        if (playerData == null)
        {
            ShowError(ErrorLabel, "Username does not exist.");
            return;
        }

        if (playerData.password != password)
        {
            ShowError(ErrorLabel, "Incorrect password.");
            return;
        }
        if (!LoginManager.Instance.LoginUser.LoginIsSuccess)
        {
            ShowError(ErrorLabel, LoginManager.Instance.LoginUser.message);
            return;
        }
        Login(username);
    }

    private void Login(string username)
    {
        PlayerData playerData = SaveSystem.Load(username);

        currentPlayerSO.LoadFrom(playerData);
        PerformLogin();
        LevelSelectManager.Instance.LevelButtonLoad();
    }

    private void ShowError(Label errorLabel, string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}