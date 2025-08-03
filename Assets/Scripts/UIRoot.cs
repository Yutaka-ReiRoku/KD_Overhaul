// Yutaka ReiRoku
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [SerializeField] private string InitialScreenName;
    void Start()
    {
        UIManager.Instance.ShowScreen(InitialScreenName, "mainscreen--bottom", "none");
    }
}