// Yutaka ReiRoku
using UnityEngine.UIElements;
public interface IScreenController
{
    VisualElement Root { get; }

    void Initialize(VisualElement rootElement);
}