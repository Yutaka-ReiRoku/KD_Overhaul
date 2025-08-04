using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public UIDocument document;
    public Camera Camera;
    public GameObject InitialPos;
    public GameObject TargetPos;
    public VisualTreeAsset towersBar;
    public VisualTreeAsset towersPanel;

    public VisualElement rootVisualElement;

    private void Start()
    {
        rootVisualElement = document.rootVisualElement;
        Invoke("moveScreen", 1f);
    }

    private void updateUI()
    {
        var newScreenInstance = towersBar.CloneTree();

    }
    
    private void moveScreen()
    {

    }
}
