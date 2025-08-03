// Yutaka ReiRoku
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Document & Screens")]
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private List<ScreenRegistration> _screenRegistrations;

    private Dictionary<string, VisualTreeAsset> _screenAssets = new Dictionary<string, VisualTreeAsset>();
    private IScreenController _currentScreenController;
    private VisualElement _rootVisualElement;

    [Serializable]
    public struct ScreenRegistration
    {
        public string id;
        public VisualTreeAsset asset;
    }

    protected override void Awake()
    {
        base.Awake();
        foreach (var registration in _screenRegistrations)
        {
            _screenAssets[registration.id] = registration.asset;
        }
    }

    private void OnEnable()
    {
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned in UIManager!");
            return;
        }
        _rootVisualElement = _uiDocument.rootVisualElement;
    }


    public void ShowScreen(string screenId, string newScreenClassName, string oldScreenClassName)
    {
        if (!_screenAssets.ContainsKey(screenId))
        {
            Debug.LogError($"Screen with ID '{screenId}' not found!");
            return;
        }

        IScreenController oldScreenController = _currentScreenController;

        var newScreenAsset = _screenAssets[screenId];
        var newScreenInstance = newScreenAsset.CloneTree();
        newScreenInstance.AddToClassList("mainscreen");
        IScreenController newController = CreateControllerForScreen(screenId);
        newController.Initialize(newScreenInstance);
        _currentScreenController = newController;

        newScreenInstance.AddToClassList(newScreenClassName);


        if (oldScreenController != null)
        {
            var oldScreenRoot = oldScreenController.Root;
            oldScreenRoot.AddToClassList(oldScreenClassName);
            oldScreenRoot.RegisterCallback<TransitionEndEvent>(evt => _rootVisualElement.Remove(oldScreenRoot));
        }

        newScreenInstance.schedule.Execute(() =>
        {
            newScreenInstance.RemoveFromClassList(newScreenClassName);
        });
        _rootVisualElement.Add(newScreenInstance);
    }

    private IScreenController CreateControllerForScreen(string screenId)
    {
        switch (screenId)
        {
            case "screen1":
                return new MainMenuController();
            default:
                Debug.LogError($"No controller defined for screen ID '{screenId}'");
                return null;
        }
    }
}