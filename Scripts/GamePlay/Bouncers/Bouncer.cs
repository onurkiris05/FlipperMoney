using System;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using VP.Nest.Haptic;

public class Bouncer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private float explosionStrength = 100f;
    [SerializeField] private ParticleSystem bounceFX;

    [Space] [Header("Debug")]
    [SerializeField] private int bouncerIncome;

    public void Set(int bouncerIncome)
    {
        this.bouncerIncome = bouncerIncome;
        headerText.text = $"${this.bouncerIncome}";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.rigidbody.AddExplosionForce(explosionStrength, transform.position, 1f);

            bounceFX.Play();
            transform.DOComplete();
            transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.1f).From();

            BouncerManager.Instance.IncreaseWallet(bouncerIncome, transform.position);
        }
    }
}