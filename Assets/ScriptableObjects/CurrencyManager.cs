
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "InGameCurrencyManager", menuName = "Tower Defense/In-Game Currency Manager")]
public class CurrencyManager : ScriptableObject
{
    public static CurrencyManager Instance { get; private set; }
    

    [Header("In-Game Settings")]
    [SerializeField] private int startingCurrency = 150;

    [NonSerialized]
    private int _currentCurrency;

    public event Action<int> OnCurrencyChanged;

    private void OnEnable()
    {
        Instance = this;
        _currentCurrency = startingCurrency;
    }

    public void UpdateInitalCurrency()
    {
        _currentCurrency = startingCurrency;
    }
    public int GetCurrentCurrency()
    {
        return _currentCurrency;
    }

    public void AddCurrency(int amount)
    {
        _currentCurrency += amount;
        OnCurrencyChanged?.Invoke(_currentCurrency);
    }

    public bool SpendCurrency(int amount)
    {
        if (_currentCurrency >= amount)
        {
            _currentCurrency -= amount;
            OnCurrencyChanged?.Invoke(_currentCurrency);
            return true;
        }
        return false;
    }

    public bool CanAfford(int cost)
    {
        return _currentCurrency >= cost;
    }
}