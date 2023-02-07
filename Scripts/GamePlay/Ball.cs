using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject colliderBody;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private ParticleSystem mergeFX;

    [Space] [Header("Debug")]
    [SerializeField] private int damage = 1;
    [SerializeField] private int level;

    public int Level => level;
    public int Damage => damage;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void ReduceLevel()
    {
        level--;

        if (level < 1)
        {
            Kill();
            BallManager.Instance.CheckAndSaveBalls();
        }
        else
        {
            int reducedDamage = (damage - 1) / 3;
            damage = reducedDamage;
            meshFilter.mesh = meshes[level - 1];
        }
    }

    public void Set(int level,int damage)
    {
        this.level = level;
        this.damage = damage;
        meshFilter.mesh = meshes[level - 1];
    }

    public void State(bool state)
    {
        colliderBody.SetActive(state);
        rigidbody.useGravity = state;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void Release(Transform spawnPos)
    {
        transform.position = spawnPos.position;
        transform.forward = spawnPos.forward;

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(transform.forward * 10f);
    }

    public void Kill()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        BallManager.Instance.Balls.Remove(this);
        BallPool.Instance.Pool.Release(this);
    }

    public void PlayMergeFX()
    {
        mergeFX.Play();
    }
}