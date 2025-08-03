// Yutaka ReiRoku
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : IScreenController
{
    public VisualElement Root { get; private set; }

    public void Initialize(VisualElement rootElement)
    {
        Root = rootElement;
        Root.AddToClassList("mainscreen");

        var changeButton = Root.Q<Button>("change-button");

        changeButton?.RegisterCallback<ClickEvent>(evt => UIManager.Instance.ShowScreen("screen2", "mainscreen--under", "mainscreen--above"));
    }
}