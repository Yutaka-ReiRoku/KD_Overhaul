// LevelSelectManager.cs
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelSelectManager : Singleton<LevelSelectManager>
{
    [Header("Data & Templates")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset levelButtonTemplate;

    [Header("Databases & Containers")]
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private CurrentPlayerDataSO currentPlayer;
    [SerializeField] private SelectedLevelSO selectedLevelContainer;


    public void LevelButtonLoad()
    {
        if (levelButtonTemplate == null)
        {
            Debug.LogError("Chưa gán Level Button Template trong Inspector!");
            return;
        }
        var root = uiDocument.rootVisualElement;
        var levelGrid = root.Q<VisualElement>("LevelButtonContainer");
        levelGrid.Clear();

        int maxLevelReached = currentPlayer.maxLevelReached;

        for (int i = 0; i < levelDatabase.allLevels.Count; i++)
        {
            LevelData levelData = levelDatabase.allLevels[i];
            int levelIndex = i + 1; // Level 1, 2, 3...

            VisualElement buttonInstance = levelButtonTemplate.CloneTree();
            Button levelButton = buttonInstance.Q<Button>("LevelButton");
            levelButton.style.display = DisplayStyle.Flex;

            levelButton.text = levelIndex.ToString();
            levelButton.RegisterCallback<ClickEvent>(evt => OnLevelButtonClick(levelData));

            bool isUnlocked = levelIndex <= maxLevelReached;
            levelButton.SetEnabled(isUnlocked);

            if (!isUnlocked)
            {
                levelButton.AddToClassList("level-button--locked");
            }

            levelGrid.Add(levelButton);
        }
    }

    private void OnLevelButtonClick(LevelData levelToLoad)
    {
        Debug.Log($"Preparing to load level: {levelToLoad.name}");

        selectedLevelContainer.selectedLevel = levelToLoad;

        MainMenuController.Instance.DisplayLoadingScreen();
    }
}