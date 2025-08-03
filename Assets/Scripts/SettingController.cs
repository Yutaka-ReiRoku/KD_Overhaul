// Yutaka ReiRoku
using UnityEngine;
using UnityEngine.UIElements;

public class SettingController : IScreenController
{
    public VisualElement Root { get; private set; }

    public void Initialize(VisualElement rootElement)
    {
        Root = rootElement;
        Root.AddToClassList("mainscreen");

        var ReturnButton = Root.Q<Button>("ReturnButton");

        ReturnButton?.RegisterCallback<ClickEvent>(evt => UIManager.Instance.ShowScreen("MainMenu", "mainscreen--above", "mainscreen--bottom"));
    }
}