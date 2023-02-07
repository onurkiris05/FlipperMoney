using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainBouncer : MonoBehaviour
{
    [SerializeField] private float explosionStrength = 100f;
    [SerializeField] private Transform animationBody;
    [SerializeField] private ParticleSystem bounceFX;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.rigidbody.AddExplosionForce(explosionStrength, transform.position, 1f, 0f, ForceMode.VelocityChange);

            bounceFX.Play();
            animationBody.DOComplete();
            animationBody.DOScaleZ(1.5f, 0.2f).From();

            AudioManager.Instance.PLayBallBounceSFX();
        }
    }
}