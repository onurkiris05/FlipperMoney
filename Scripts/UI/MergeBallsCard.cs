using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VP.Nest.Economy;
using VP.Nest.UI;
using VPNest.UI.Scripts.IncrementalUI;

public class MergeBallsCard : IncrementalCard
{
    [SerializeField] private GameObject maxCard;

    public override void CheckForMoney()
    {
        if (!BallManager.Instance.CanMerge())
        {
            if (maxCard.activeSelf) return;

            maxCard.SetActive(true);
        }
        else if (!GameEconomy.HasPlayerEnoughMoney(incrementalData.CurrentPrice)
                 || LevelController.Instance.IsNextLevelLoading)
        {
            if (disabledCard.activeSelf) return;

            disabledCard.SetActive(true);
            maxCard.SetActive(false);
        }
        else
        {
            disabledCard.SetActive(false);
            maxCard.SetActive(false);
        }
    }

    protected override void CurrencyPurchase()
    {
        if (maxCard.activeSelf)
        {
            OnDisabledCardClicked();
            return;
        }

        base.CurrencyPurchase();
    }
}