using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VP.Nest.Analytics;
using VP.Nest.Economy;
using VP.Nest.Haptic;
using VP.Nest.UI;

namespace VPNest.UI.Scripts.IncrementalUI
{
    public class IncrementalCard : MonoBehaviour
    {
        [SerializeField] private Button currencyButton;
        [SerializeField] private TextMeshProUGUI priceTMP;
        [SerializeField] private TextMeshProUGUI priceTMPDisabled;
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] protected GameObject disabledCard;

        public IncrementalData incrementalData;

        public event Action OnCurrencyPurchase;

        protected void Awake()
        {
            currencyButton.onClick.AddListener(CurrencyPurchase);
            UpdateCardData();
        }

        private void OnEnable()
        {
            GameEconomy.OnPlayerMoneyUpdate += CheckForMoney;
        }

        private void OnDisable()
        {
            GameEconomy.OnPlayerMoneyUpdate -= CheckForMoney;
        }

        public virtual void CheckForMoney()
        {
            if (!GameEconomy.HasPlayerEnoughMoney(incrementalData.CurrentPrice))
            {
                if (disabledCard.activeSelf) return;

                disabledCard.SetActive(true);
            }
            else
            {
                disabledCard.SetActive(false);
            }
        }


        public IncrementalCard UpdateCardData()
        {
            CheckForMoney();

            priceTMP.text = incrementalData.CurrentPrice.FormatMoney();
            priceTMPDisabled.text = incrementalData.CurrentPrice.FormatMoney();
            SetLevel();
            return this;
        }

        public virtual IncrementalCard OnCurrency(Action onCurrencyPurchase)
        {
            this.OnCurrencyPurchase = onCurrencyPurchase;
            return this;
        }

        protected virtual void OnDisabledCardClicked()
        {
            FailClickAnim();
        }

        private void FailClickAnim()
        {
            transform.DOComplete();
            transform.DOShakeRotation(.2f, 20, 6, 10).SetLink(gameObject);
            HapticManager.Haptic(HapticType.Warning);
        }

        protected virtual void SuccessClickAnim()
        {
            currencyButton.transform.DOComplete();
            currencyButton.transform.DOLocalMoveY(-90f, .25f).From().SetEase(Ease.Linear);
            HapticManager.Haptic(HapticType.SoftImpact);
            AudioManager.Instance.PlayButtonClickSFX();
        }

        protected virtual void CurrencyPurchase()
        {
            if (disabledCard.activeSelf)
            {
                OnDisabledCardClicked();
                return;
            }

            OnCurrencyPurchase?.Invoke();
            UIManager.Instance.CurrencyUI.SpendMoney(incrementalData.CurrentPrice);
            incrementalData.CurrentIndex++;
            SuccessClickAnim();
            UpdateCardData();
            TriggerEvents();

            if (incrementalData.upgradeType == UpgradeType.AddBall) FTUEManager.Instance.Step2?.Invoke();
        }

        protected virtual void SetLevel()
        {
            levelTMP.text = "Lv." + (incrementalData.CurrentIndex + 1);
        }

        private void TriggerEvents()
        {
            switch (incrementalData.upgradeType)
            {
                case UpgradeType.AddBall:
                    AnalyticsManager.CustomEvent("add-ball", incrementalData.CurrentIndex);
                    break;

                case UpgradeType.MergeBalls:
                    AnalyticsManager.CustomEvent("merge-ball", incrementalData.CurrentIndex);
                    break;

                case UpgradeType.Income:
                    AnalyticsManager.CustomEvent("income-pressed", incrementalData.CurrentIndex);
                    break;
            }
        }
    }
}