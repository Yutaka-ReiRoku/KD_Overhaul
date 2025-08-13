
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Playing, Won, Lost }
    public GameState currentState { get; private set; }

    [Header("Data Containers")]
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private SelectedLevelSO selectedLevel;
    [SerializeField] private CurrentPlayerDataSO currentPlayer;

    [Header("UI References")]
    [SerializeField] private UIDocument gameUIDocument;
    [SerializeField] private VisualTreeAsset towerCardTemplate;
    [SerializeField] private VisualTreeAsset endGamePanelTemplate;
    [SerializeField] private VisualTreeAsset pauseMenuTemplate;

    private VisualElement root;

    private VisualElement endGamePanelInstance;
    private VisualElement endGamePanelContainer;
    private Label titleLabel;
    private Button primaryButton;
    private Button secondaryButton;

    private VisualElement pauseMenuInstance;
    private VisualElement pauseMenuContainer;
    private Slider musicSlider;
    private Slider sfxSlider;
    private bool isPaused = false;

    private void Awake() { /* Singleton */ }

    private void Start()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;
        root = gameUIDocument.rootVisualElement;

        if (endGamePanelTemplate != null)
        {
            endGamePanelInstance = endGamePanelTemplate.CloneTree();
            endGamePanelInstance.style.position = Position.Absolute;
            endGamePanelInstance.style.width = new Length(100, LengthUnit.Percent);
            endGamePanelInstance.style.height = new Length(100, LengthUnit.Percent);

            endGamePanelInstance.pickingMode = PickingMode.Ignore;

            endGamePanelContainer = endGamePanelInstance.Q("end-game-panel-container");
            titleLabel = endGamePanelInstance.Q<Label>("title-label");
            primaryButton = endGamePanelInstance.Q<Button>("primary-button");
            secondaryButton = endGamePanelInstance.Q<Button>("secondary-button");
            root.Add(endGamePanelInstance);
        }

        if (pauseMenuTemplate != null)
        {
            pauseMenuInstance = pauseMenuTemplate.CloneTree();
            pauseMenuInstance.style.position = Position.Absolute;
            pauseMenuInstance.style.width = new Length(100, LengthUnit.Percent);
            pauseMenuInstance.style.height = new Length(100, LengthUnit.Percent);

            pauseMenuInstance.pickingMode = PickingMode.Ignore;

            pauseMenuContainer = pauseMenuInstance.Q("pause-menu-container");
            pauseMenuContainer.Q<Button>("resume-button").clicked += ResumeGame;
            pauseMenuContainer.Q<Button>("restart-button").clicked += RestartLevel;
            pauseMenuContainer.Q<Button>("main-menu-button").clicked += GoToMainMenu;
            musicSlider = pauseMenuInstance.Q<Slider>("music-slider");
            sfxSlider = pauseMenuInstance.Q<Slider>("sfx-slider");
            musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            sfxSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);
            root.Add(pauseMenuInstance);
            LoadAndApplyAudioSettings();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuContainer.AddToClassList("pause-menu-container--visible");

        pauseMenuInstance.pickingMode = PickingMode.Position;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuContainer.RemoveFromClassList("pause-menu-container--visible");

        pauseMenuInstance.pickingMode = PickingMode.Ignore;
    }

    private void ShowEndGamePanel(bool hasWon)
    {
        if (endGamePanelContainer == null) return;


        endGamePanelContainer.AddToClassList("end-game-panel-container--visible");

        endGamePanelInstance.pickingMode = PickingMode.Position;
    }

    #region Unchanged Code
    public void OnPause(InputAction.CallbackContext context) { if (context.performed && currentState == GameState.Playing) { TogglePause(); } }
    private void TogglePause() { isPaused = !isPaused; if (isPaused) { PauseGame(); } else { ResumeGame(); } }
    public void TriggerWin(Vector3 lastEnemyWorldPosition)
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Won;
        TowerData rewardData = selectedLevel.selectedLevel.rewardTowerData;
        if (rewardData != null)
        {
            Debug.Log("YOU WIN! Claim your reward!");
            CreateRewardCard(rewardData, lastEnemyWorldPosition);
        }
        else
        {
            Debug.Log("YOU WIN!");
            ShowEndGamePanel(true);
        }
    }
    private void CreateRewardCard(TowerData rewardData, Vector3 worldPosition)
    {
        VisualElement cardInstance = towerCardTemplate.CloneTree();
        Button rewardCard = cardInstance.Q<Button>();
        rewardCard.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(rewardData.towerIcon);
        rewardCard.Q<Label>("cost").text = "NEW!";
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition.y = Screen.height - screenPosition.y;
        Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);
        float cardWidth = 95f; float cardHeight = 105f;
        panelPosition.x = Mathf.Clamp(panelPosition.x - cardWidth / 2, 0, root.resolvedStyle.width - cardWidth);
        panelPosition.y = Mathf.Clamp(panelPosition.y - cardHeight / 2, 0, root.resolvedStyle.height - cardHeight);
        rewardCard.style.position = Position.Absolute;
        rewardCard.style.left = panelPosition.x;
        rewardCard.style.top = panelPosition.y;
        root.Add(rewardCard);
        rewardCard.RegisterCallback<ClickEvent>(evt => OnRewardCardClicked(rewardCard, rewardData));
    }
    private void OnRewardCardClicked(Button card, TowerData rewardData)
    {
        Debug.Log($"Collected reward: {rewardData.towerName}!");
        if (!currentPlayer.ownedTowerIDs.Contains(rewardData.name))
        {
            currentPlayer.ownedTowerIDs.Add(rewardData.name);
            Debug.Log($"New tower unlocked: {rewardData.name}");
        }
        LevelData completedLevel = selectedLevel.selectedLevel;
        if (levelDatabase != null)
        {
            int completedLevelIndex = levelDatabase.allLevels.IndexOf(completedLevel);
            int completedLevelNumber = completedLevelIndex + 1;
            if (completedLevelNumber >= currentPlayer.maxLevelReached)
            {
                currentPlayer.maxLevelReached = completedLevelNumber + 1;
                Debug.Log($"Max level reached is now: {currentPlayer.maxLevelReached}");
            }
        }
        SaveSystem.Save(currentPlayer.GetSaveData(), currentPlayer.playerName);
        Debug.Log("Player data saved!");
        card.RemoveFromHierarchy();
        ShowEndGamePanel(true);
    }
    public void TriggerLoss(EnemyBase intrudingEnemy)
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Lost;
        Debug.Log("GAME OVER!");
        ShowEndGamePanel(false);
        FindObjectOfType<WaveSpawner>().enabled = false;
        foreach (var tower in FindObjectsOfType<TowerBase>()) { Destroy(tower.gameObject); }
        foreach (var enemy in FindObjectsOfType<EnemyBase>())
        {
            if (enemy != intrudingEnemy) Destroy(enemy.gameObject);
        }
        if (intrudingEnemy != null) { intrudingEnemy.StartAttackingBase(); }
    }
    private void GoToNextLevel() { SceneManager.LoadScene("LevelSelectScene"); }
    private void RestartLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    private void GoToMainMenu() { SceneManager.LoadScene(0); }
    private void OnMusicVolumeChanged(ChangeEvent<float> evt) { float volume = evt.newValue; SoundManager.Instance.SetMusicVolume(volume); PlayerPrefs.SetFloat("MusicVolume", volume); }
    private void OnSFXVolumeChanged(ChangeEvent<float> evt) { float volume = evt.newValue; SoundManager.Instance.SetSFXVolume(volume); PlayerPrefs.SetFloat("SFXVolume", volume); }
    private void LoadAndApplyAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        SoundManager.Instance.SetMusicVolume(musicVolume);
        SoundManager.Instance.SetSFXVolume(sfxVolume);
    }
    #endregion
}