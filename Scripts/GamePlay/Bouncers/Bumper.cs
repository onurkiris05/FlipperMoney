using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] private float explosionStrength = 100f;
    [SerializeField] private Transform body;
    [SerializeField] private ParticleSystem bounceFX;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.rigidbody.AddExplosionForce(explosionStrength, transform.position, 1f);

            bounceFX.Play();
            body.DOComplete();
            body.DOScaleX(1.5f, 0.3f).From();

            AudioManager.Instance.PLayBallBounceSFX();
        }
    }
}
