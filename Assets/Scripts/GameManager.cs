using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static TowersManager;

public class GameManager : Singleton<GameManager>
{
    public UIDocument document;
    public Transform Camera;
    public Transform InitialPos;
    public Transform TargetPos;
    public VisualTreeAsset towersBar;
    public VisualTreeAsset towersPanel;

    public VisualElement rootVisualElement;

    private TemplateContainer newScreenInstance;

    private TemplateContainer new2ScreenInstance;
    private void Start()
    {
        rootVisualElement = document.rootVisualElement;
        updateUI();
        Invoke("moveScreen", 1f);
        Invoke("removeSelectorClass", 1f);
    }

    private void updateUI()
    {
        newScreenInstance = towersBar.CloneTree();
        newScreenInstance.AddToClassList("fullscreen");
        rootVisualElement.Add(newScreenInstance);
        newScreenInstance.pickingMode = PickingMode.Ignore;
        new2ScreenInstance = towersPanel.CloneTree();
        new2ScreenInstance.AddToClassList("fullscreen");
        rootVisualElement.Add(new2ScreenInstance);
        new2ScreenInstance.pickingMode = PickingMode.Ignore;
    }

    private void removeSelectorClass()
    {
        new2ScreenInstance.Q<VisualElement>("squad-selection-panel").schedule.Execute(() =>
        {
            new2ScreenInstance.Q<VisualElement>("squad-selection-panel").RemoveFromClassList("squad-selection-panel--hidden");
        });
        newScreenInstance.Q<VisualElement>("tower-selection-bar").schedule.Execute(() =>
        {
            newScreenInstance.Q<VisualElement>("tower-selection-bar").RemoveFromClassList("tower-selection-bar--hidden");
        });
    }
    
    private void moveScreen()
    {
        Camera.DOMove(TargetPos.position, 1f);
    }
}
