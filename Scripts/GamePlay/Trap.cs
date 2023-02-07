using System;
using DG.Tweening;
using UnityEngine;
using VP.Nest.Analytics;
using VP.Nest.Haptic;

public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private float explosionStrength = 10f;
    [SerializeField] private float activeDuration = 4f;
    [SerializeField] private float disableDuration = 4f;
    [SerializeField] private Vector3 targetPos, startPos;
    [SerializeField] private GameObject body;
    [SerializeField] private ParticleSystem killFX;

    private int trapTriggerCount;

    private void Start()
    {
        transform.localPosition = startPos;

        Rotate();
        Move();
    }

    private void Move()
    {
        transform.DOMove(targetPos, 2f).SetDelay(activeDuration)
            .OnComplete(() => { transform.DOMove(startPos, 2f).SetDelay(disableDuration).OnComplete(Move); });
    }

    private void Rotate()
    {
        body.transform.DOLocalRotate(new Vector3(0, 90, -180), 3f)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            trapTriggerCount = PlayerPrefs.GetInt("TrapTriggerCount", 0);

            other.rigidbody.AddExplosionForce(explosionStrength, transform.position, 1f);
            other.transform.GetComponent<Ball>().ReduceLevel();

            killFX.Play();
            DOTween.Complete("Scale");
            transform.DOScale(new Vector3(2f, 2f, 2f), 0.2f).From().SetId("Scale");

            PlayerPrefs.SetInt("TrapTriggerCount", trapTriggerCount + 1);
            trapTriggerCount = PlayerPrefs.GetInt("TrapTriggerCount");
            AnalyticsManager.CustomEvent("thorn-pressed", trapTriggerCount);
            HapticManager.Haptic(HapticType.MediumImpact);
        }
    }
}