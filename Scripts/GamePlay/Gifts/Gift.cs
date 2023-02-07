using System.Collections;
using DG.Tweening;
using ElephantSDK;
using TMPro;
using UnityEngine;
using VP.Nest.Haptic;

public class Gift : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private RewardType rewardType;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject collider;
    [SerializeField] private TextMeshPro healthText;

    [Space] [Header("Box Gift Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform moneyModel;
    [SerializeField] private Transform boxBallModel;
    [SerializeField] private ParticleSystem confettiFX;
    [SerializeField] private ParticleSystem confettiMiniFX;

    [Space] [Header("Cube Gift Settings")]
    [SerializeField] private GameObject fractionBody;
    [SerializeField] private GameObject cubeMoneyBody;
    [SerializeField] private MeshRenderer cubeMoneyRenderer;
    [SerializeField] private MeshRenderer cubeBallBodyRenderer;
    [SerializeField] private MeshRenderer cubeBallMesh;

    public RewardType BaseRewardType => rewardType;

    private int firstHealth;
    private int health;
    private int spawnPoint;
    private int rewardValue;

    private void OnEnable()
    {
        body.transform.DORotate(new Vector3(0, 180f, 0f), 2f)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    public void Set(int health, int rewardValue)
    {
        this.health = health;
        firstHealth = this.health;

        if (PlayerPrefs.GetInt("SavedGame") > 0)
        {
            this.health = PlayerPrefs.GetInt("GiftHealth", RemoteConfigManager.GiftBaseHealth);
            AdjustAlpha();
        }

        this.rewardValue = rewardValue;
        healthText.text = $"{this.health}";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            int damage = other.transform.GetComponent<Ball>().Damage;

            health -= damage;
            PlayerPrefs.SetInt("GiftHealth", health);

            if (health < 1)
            {
                PlayerPrefs.SetInt("GiftHealth", GiftManager.Instance.GiftBaseHealth);
                healthText.enabled = false;
                Reward();
            }
            else
            {
                //Lower color alpha per hit for ice cubes
                AdjustAlpha();

                healthText.text = $"{health}";
                AddFloatingText($"-{damage}");
                DOTween.Complete("Scale");
                body.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).From().SetId("Scale");

                PlayMiniConfettiFX();
            }
        }
    }

    #region Reward Method

    private void Reward()
    {
        HapticManager.Haptic(HapticType.Success);
        LevelController.Instance.IsNextLevelLoading = true;
        IncrementalManager.Instance.CheckMergeButton();

        switch (rewardType)
        {
            case RewardType.BoxMoney:
                StartCoroutine(ProcessBoxMoney());
                break;

            case RewardType.BoxBall:
                StartCoroutine(ProcessBoxBall());
                break;

            case RewardType.CubeMoney:
                StartCoroutine(ProcessCubeMoney());
                break;

            case RewardType.CubeBall:
                StartCoroutine(ProcessCubeBall());
                break;
        }

        //Set next level index
        int gameLevel = PlayerPrefs.GetInt("GameLevel", 1);
        PlayerPrefs.SetInt("GameLevel", gameLevel + 1);
    }

    #endregion

    #region Reward Process Methods

    IEnumerator ProcessBoxMoney()
    {
        Deactivate();
        confettiFX.Play();
        transform.DOPunchScale(Vector3.one, 0.4f, 1);
        animator.SetFloat("Speed", 2);

        yield return new WaitForSeconds(0.6f);

        AddFloatingText(rewardValue);
        moneyModel.DOLocalMoveY(0.4f, 2f)
            .OnComplete(() =>
            {
                EconomyManager.Instance.AddMoney(rewardValue);
                transform.DOScale(Vector3.zero, 0.6f).OnComplete(() =>
                {
                    body.transform.DOKill();
                    LevelController.Instance.LoadNextLevel();
                });
            });
    }

    IEnumerator ProcessBoxBall()
    {
        Deactivate();
        confettiFX.Play();
        transform.DOPunchScale(Vector3.one, 0.4f, 1);
        animator.SetFloat("Speed", 2);

        yield return new WaitForSeconds(0.6f);

        AddFloatingText("+1 Ball");
        boxBallModel.DOLocalMoveY(0.4f, 2f)
            .OnComplete(() =>
            {
                BallManager.Instance.AddBall(transform);
                transform.DOScale(Vector3.zero, 0.6f).OnComplete(() =>
                {
                    body.transform.DOKill();
                    LevelController.Instance.LoadNextLevel();
                });
            });
    }

    IEnumerator ProcessCubeMoney()
    {
        Deactivate();
        cubeMoneyRenderer.enabled = false;
        fractionBody.SetActive(true);

        AddFloatingText(rewardValue);
        EconomyManager.Instance.AddMoney(rewardValue);

        cubeMoneyBody.transform.DOLocalMoveY(0.4f, 2f)
            .OnComplete(() =>
            {
                fractionBody.SetActive(false);
                transform.DOScale(Vector3.zero, 0.6f).OnComplete(() =>
                {
                    body.transform.DOKill();

                    //Reset material alpha
                    Color color = cubeMoneyRenderer.material.color;
                    color.a = 1;
                    cubeMoneyRenderer.material.color = color;
                    cubeBallBodyRenderer.material.color = color;

                    LevelController.Instance.LoadNextLevel();
                });
            });

        yield return null;
    }

    IEnumerator ProcessCubeBall()
    {
        Deactivate();
        cubeBallBodyRenderer.enabled = false;
        fractionBody.SetActive(true);

        AddFloatingText("+1 Ball");
        cubeBallMesh.transform.DOLocalMoveY(0.4f, 2f)
            .OnComplete(() =>
            {
                cubeBallMesh.enabled = false;
                fractionBody.SetActive(false);
                BallManager.Instance.AddBall(cubeBallMesh.transform);

                cubeBallBodyRenderer.transform.DOScale(Vector3.zero, 0.6f).OnComplete(() =>
                {
                    body.transform.DOKill();

                    //Reset material alpha
                    Color color = cubeMoneyRenderer.material.color;
                    color.a = 1;
                    cubeMoneyRenderer.material.color = color;
                    cubeBallBodyRenderer.material.color = color;

                    LevelController.Instance.LoadNextLevel();
                });
            });

        yield return null;
    }

    #endregion

    #region Sub Methods

    private void Deactivate()
    {
        collider.SetActive(false);
    }

    private void PlayMiniConfettiFX()
    {
        if (rewardType == RewardType.BoxMoney || rewardType == RewardType.BoxBall) confettiMiniFX.Play();
    }

    private void AdjustAlpha()
    {
        if (rewardType == RewardType.CubeBall || rewardType == RewardType.CubeMoney)
        {
            Color color = cubeMoneyRenderer.material.color;
            color.a = (float)health / firstHealth;
            cubeMoneyRenderer.material.color = color;
            cubeBallBodyRenderer.material.color = color;
        }
    }

    private void AddFloatingText(int value)
    {
        FloatingText text = FloatingTextPool.Instance.Pool.Get();
        text.SetText(value, transform.position, 2f);
    }

    private void AddFloatingText(string textValue)
    {
        FloatingText text = FloatingTextPool.Instance.Pool.Get();
        text.SetText(textValue, transform.position, 2f);
    }

    #endregion

    public enum RewardType
    {
        BoxMoney,
        BoxBall,
        CubeMoney,
        CubeBall
    }
}