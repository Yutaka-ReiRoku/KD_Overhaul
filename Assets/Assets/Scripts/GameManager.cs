using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerTemp : Singleton<GameManager>
{
    [Header("UI Toolkit References")]
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset towersBar;
    [SerializeField] private VisualTreeAsset towersPanel;

    [Header("Camera Movement")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform initialPos;
    [SerializeField] private Transform targetPos;

    private VisualElement rootVisualElement;
    private VisualElement loadingPanel;
    private TemplateContainer towersBarInstance;
    private TemplateContainer towersPanelInstance;


    private void OnEnable()
    {
        rootVisualElement = document.rootVisualElement;

        loadingPanel = rootVisualElement.Q("LoadingPanel");

        rootVisualElement.schedule.Execute(() => loadingPanel.RemoveFromClassList("loading-panel-active")).ExecuteLater(1500);
    }
    private void Start()
    {

        UpdateUI();

        Sequence introSequence = DOTween.Sequence();
        introSequence.AppendInterval(2f)
                     .AppendCallback(MoveScreen)
                     .JoinCallback(ShowUIElements);
    }

    /// <summary>
    /// </summary>
    private void UpdateUI()
    {
        towersBarInstance = CreateAndAddUIElement(towersBar, "fullscreen");
        towersPanelInstance = CreateAndAddUIElement(towersPanel, "fullscreen");
    }

    /// <summary>
    /// </summary>
    private TemplateContainer CreateAndAddUIElement(VisualTreeAsset visualTree, string className)
    {
        var newInstance = visualTree.CloneTree();
        newInstance.AddToClassList(className);
        newInstance.pickingMode = PickingMode.Ignore;
        rootVisualElement.Add(newInstance);
        return newInstance;
    }

    /// <summary>
    /// </summary>
    private void ShowUIElements()
    {
        var squadPanel = towersPanelInstance.Q<VisualElement>("squad-selection-panel");
        squadPanel?.schedule.Execute(() =>
        {
            squadPanel.RemoveFromClassList("squad-selection-panel--hidden");
        });

        var towerBar = towersBarInstance.Q<VisualElement>("tower-selection-bar");
        towerBar?.schedule.Execute(() =>
        {
            towerBar.RemoveFromClassList("tower-selection-bar--hidden");
        });
    }

    /// <summary>
    /// </summary>
    private void MoveScreen()
    {
        cameraTransform.DOMove(targetPos.position, 2f).SetEase(Ease.InOutQuad);
    }
}