using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ElephantSDK;
using UnityEngine;

public class BouncerManager : Singleton<BouncerManager>
{
    [SerializeField] private List<Bouncer> bouncers;
    [SerializeField] private int[] baseBouncerIncomes = { 100, 200, 500 };
    [SerializeField] private int[] bouncerIncomeIncrements = { 50, 100, 200 };

    private void Start()
    {
        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Income).OnCurrencyPurchase += IncreaseBouncerIncomes;
        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Income).OnCurrencyPurchase += AnimateBouncers;
        LevelController.Instance.OnLevelChange += FindAndSetBouncers;

        if (PlayerPrefs.GetInt("SavedGame") > 0)
        {
            for (int i = 0; i < baseBouncerIncomes.Length; i++)
                baseBouncerIncomes[i] = PlayerPrefs.GetInt($"BaseBouncerIncome{i}", RemoteConfigManager.BaseBouncerIncomes[i]);
        }
        else baseBouncerIncomes = RemoteConfigManager.BaseBouncerIncomes;

        bouncerIncomeIncrements = RemoteConfigManager.BaseBouncerIncomeIncrements;
    }

    public void IncreaseWallet(int bouncerIncome, Vector3 textPos)
    {
        EconomyManager.Instance.AddMoney(bouncerIncome);

        AddFloatingText(textPos, bouncerIncome);
    }

    private void AddFloatingText(Vector3 textPos, int value)
    {
        FloatingText text = FloatingTextPool.Instance.Pool.Get();
        text.SetText(value, textPos);
    }

    private void FindAndSetBouncers()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Bouncer");

        bouncers.Clear();
        foreach (var obj in objects) bouncers.Add(obj.GetComponent<Bouncer>());

        for (int i = 0; i < bouncers.Count; i++) bouncers[i].Set(baseBouncerIncomes[i]);
    }

    private void IncreaseBouncerIncomes()
    {
        for (int i = 0; i < baseBouncerIncomes.Length; i++) baseBouncerIncomes[i] += bouncerIncomeIncrements[i];
        for (int i = 0; i < bouncers.Count; i++) bouncers[i].Set(baseBouncerIncomes[i]);
        for (int i = 0; i < baseBouncerIncomes.Length; i++)
        {
            PlayerPrefs.SetInt($"BaseBouncerIncome{i}", baseBouncerIncomes[i]);
        }
    }

    private void AnimateBouncers()
    {
        for (int i = 0; i < bouncers.Count; i++)
        {
            bouncers[i].transform.DOComplete();
            bouncers[i].transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).From();
        }
    }
}