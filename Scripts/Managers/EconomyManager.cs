using UnityEngine;
using VP.Nest.UI.Currency;

public class EconomyManager : Singleton<EconomyManager>
{
    [SerializeField] private CurrencyUI currencyUI;

    public void AddMoney(int value)
    {
        currencyUI.AddMoney(value, false);
    }
}
