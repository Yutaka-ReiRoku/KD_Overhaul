using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    [Header("UI Toolkit References")]
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset towersBar;
    [SerializeField] private VisualTreeAsset towersPanel;

    [Header("Camera Movement")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform initialPos; // Có thể bạn không cần biến này nếu chỉ di chuyển một chiều
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

        // Sử dụng DOTween Sequence để quản lý các hành động theo thời gian
        Sequence introSequence = DOTween.Sequence();
        introSequence.AppendInterval(2f)
                     .AppendCallback(MoveScreen)
                     .JoinCallback(ShowUIElements);
    }

    /// <summary>
    /// Khởi tạo và hiển thị các element của UI từ VisualTreeAssets.
    /// </summary>
    private void UpdateUI()
    {
        towersBarInstance = CreateAndAddUIElement(towersBar, "fullscreen");
        towersPanelInstance = CreateAndAddUIElement(towersPanel, "fullscreen");
    }

    /// <summary>
    /// Hàm phụ trợ để tạo một UI element từ asset, thêm class và add vào root.
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
    /// Làm cho các panel UI trở nên hữu hình bằng cách xóa class "--hidden".
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
    /// Di chuyển camera đến vị trí mục tiêu một cách mượt mà.
    /// </summary>
    private void MoveScreen()
    {
        cameraTransform.DOMove(targetPos.position, 2f).SetEase(Ease.InOutQuad);
    }
}