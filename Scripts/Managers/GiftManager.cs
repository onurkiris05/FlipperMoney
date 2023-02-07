using System;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

public class GiftManager : Singleton<GiftManager>
{
    [Header("Gift Settings")]
    [SerializeField] private int giftBaseHealth = 100;
    [SerializeField] private int giftHealthIncrement = 100;
    [SerializeField] private int giftMoneyReward = 2000;
    [SerializeField] private int giftMoneyRewardIncrement = 4000;

    [Space] [Header("Debug")]
    [SerializeField] private List<Gift> gifts = new List<Gift>();

    public int GiftBaseHealth => giftBaseHealth;

    private void Awake()
    {
        giftBaseHealth = RemoteConfigManager.GiftBaseHealth;
        giftHealthIncrement = RemoteConfigManager.GiftBaseHealthIncrement;

        giftMoneyReward = RemoteConfigManager.GiftBaseMoney;
        giftMoneyRewardIncrement = RemoteConfigManager.GiftBaseMoneyIncrement;

        if (PlayerPrefs.GetInt("SavedGame") > 0)
        {
            giftBaseHealth = PlayerPrefs.GetInt("GiftBaseHealth");
            giftMoneyReward = PlayerPrefs.GetInt("GiftMoneyReward");
        }
    }

    private void Start()
    {
        LevelController.Instance.OnLevelChange += FindAndSetGifts;
    }

    private void FindAndSetGifts()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Gift");

        gifts.Clear();
        foreach (var obj in objects) gifts.Add(obj.GetComponent<Gift>());
        foreach (Gift gift in gifts) gift.Set(giftBaseHealth, giftMoneyReward);

        PlayerPrefs.SetInt("GiftBaseHealth", giftBaseHealth);
        PlayerPrefs.SetInt("GiftMoneyReward", giftMoneyReward);

        giftBaseHealth += giftHealthIncrement;

        if (gifts[0].BaseRewardType == Gift.RewardType.BoxMoney
            || gifts[0].BaseRewardType == Gift.RewardType.CubeMoney)
        {
            giftMoneyReward += giftMoneyRewardIncrement;
        }
    }
}