using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VP.Nest.Analytics;
using VP.Nest.Haptic;

public class BallManager : Singleton<BallManager>
{
    [Header("Ball Settings")]
    [SerializeField] private Transform spawnPos;

    [Space] [Header("Merge Settings")]
    [SerializeField] private int mergeSize = 3;
    [SerializeField] private GameObject mergeParent;
    [SerializeField] private Transform[] mergePoints;

    public List<Ball> Balls = new List<Ball>();

    private int mergeEvent;
    private int mergeLevel;
    private int maxBallLevel = 1;
    private bool isMerging;

    private void Start()
    {
        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.AddBall).OnCurrencyPurchase += AddBall;
        IncrementalManager.Instance.GetUpgradeCard(UpgradeType.MergeBalls).OnCurrencyPurchase += MergeBalls;

        mergeEvent = PlayerPrefs.GetInt("MergeEvent", 0);
    }

    #region Ball Methods

    public void LoadBalls()
    {
        if (PlayerPrefs.GetInt("SavedGame") > 0)
        {
            for (int i = 1; i <= Balls.Count;) Balls[0].Kill();

            //Load balls
            int ballCount = PlayerPrefs.GetInt("BallCount");

            for (int i = 0; i < ballCount; i++)
            {
                Ball ball = BallPool.Instance.Pool.Get();
                ball.Set(PlayerPrefs.GetInt($"Ball_{i}_level"), PlayerPrefs.GetInt($"Ball_{i}_damage"));
                Balls.Add(ball);
            }

            //Set balls
            foreach (Ball ball in Balls)
            {
                ball.State(false);
                ball.transform.position = spawnPos.position;
            }

            StartCoroutine(ProcessLoadBalls());
        }
    }

    IEnumerator ProcessLoadBalls()
    {
        //Spawn balls in order
        // foreach (Ball ball in Balls)
        // {
        //     ball.State(true);
        //     ball.Release(spawnPos);
        //     yield return new WaitForSeconds(0.3f);
        // }

        for (int i = 0; i < Balls.Count; i++)
        {
            Balls[i].State(true);
            Balls[i].Release(spawnPos);
            yield return new WaitForSeconds(0.3f);
        }
    }

    //Add level 1 ball if there is no ball in the game
    public void CheckAndSaveBalls()
    {
        //This check for if last ball was killed
        if (Balls.Count < 1) AddBall();

        //Save balls
        PlayerPrefs.SetInt("BallCount", Balls.Count);

        for (int i = 0; i < Balls.Count; i++)
        {
            PlayerPrefs.SetInt($"Ball_{i}_level", Balls[i].Level);
            PlayerPrefs.SetInt($"Ball_{i}_damage", Balls[i].Damage);
        }
    }

    //Add ball with desired level, damage and position to be spawn
    public void AddBall(int level, Transform target, int damage = 1)
    {
        Ball ball = BallPool.Instance.Pool.Get();
        ball.Set(level, damage);
        ball.transform.position = target.position;
        ball.transform.forward = target.forward;
        ball.PlayMergeFX();
        Balls.Add(ball);
        CheckAndSaveBalls();

        if (level > maxBallLevel) maxBallLevel = level;
    }

    //Add level 1 ball from desired position
    public void AddBall(Transform target)
    {
        Ball ball = BallPool.Instance.Pool.Get();
        ball.Set(1, 1);
        ball.transform.position = target.position;
        ball.transform.forward = target.forward;
        Balls.Add(ball);
        CheckAndSaveBalls();
    }

    //Add level 1 ball from ball pipe
    private void AddBall()
    {
        Ball ball = BallPool.Instance.Pool.Get();
        ball.Set(1, 1);
        ball.Release(spawnPos);
        Balls.Add(ball);
        CheckAndSaveBalls();
    }

    private List<Ball> GetBalls()
    {
        List<Ball> mergeBalls = new List<Ball>();

        mergeLevel = 1;

        //Iterate all balls along maxBallLevel
        for (int i = 0; i < maxBallLevel; i++)
        {
            //Find balls that has same level
            mergeBalls = Balls.FindAll(x => x.Level.Equals(mergeLevel));

            //Increase merge level if founded ball list size lower then mergeSize
            if (mergeBalls.Count < mergeSize) mergeLevel++;

            //Subtract excess ones if founded ball list size higher then mergeSize
            else if (mergeBalls.Count > mergeSize)
            {
                int subtractNum = mergeBalls.Count - mergeSize;

                for (int j = 0; j < subtractNum; j++)
                    mergeBalls.Remove(mergeBalls[0]);

                break;
            }
            else break;
        }

        //If there are not enough same balls at all, return null
        if (mergeBalls.Count < mergeSize) return null;

        return mergeBalls;
    }

    #endregion

    #region Merge Methods

    public bool CanMerge()
    {
        if (GetBalls() != null && !isMerging) return true;

        return false;
    }

    private void MergeBalls()
    {
        List<Ball> mergeBalls = GetBalls();

        if (mergeBalls == null) return;

        ProcessMerge(mergeBalls);
    }

    private void ProcessMerge(List<Ball> mergeBalls)
    {
        if (isMerging) return;
        isMerging = true;

        //Set incremental button
        IncrementalManager.Instance.CheckMergeButton();

        //Calculate next level and damage
        int upgradedLevel = mergeBalls[0].Level + 1;
        int damage = (mergeBalls[0].Damage * mergeBalls.Count) + 1;

        //Adjust event for max reached ball level
        if (mergeEvent < upgradedLevel)
        {
            mergeEvent = upgradedLevel;
            PlayerPrefs.SetInt("MergeEvent", mergeEvent);
            AnalyticsManager.CustomEvent($"ball-max-level-achieved_{mergeEvent}", mergeEvent);
        }

        //Move balls to animation points and set parent
        for (int i = 0; i < mergeBalls.Count; i++)
        {
            Balls.Remove(mergeBalls[i]);

            mergeBalls[i].State(false);
            mergeBalls[i].transform.parent = mergePoints[i];
            mergeBalls[i].transform.DOMove(mergePoints[i].position, 0.5f).SetEase(Ease.OutBack);
        }

        //Set animations for merge animation parent
        mergeParent.transform.DORotate(new Vector3(-180, -90, 0), 0.7f).SetEase(Ease.InExpo)
            .SetLoops(-1, LoopType.Incremental);
        mergeParent.transform.DOScale(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.InElastic)
            .OnComplete(() =>
            {
                //Destroy merged balls
                foreach (Ball ball in mergeBalls) Destroy(ball.gameObject);

                //Reset merge parent game object
                mergeParent.transform.DOKill();
                mergeParent.transform.localScale = Vector3.one;
                mergeParent.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));

                //Add upgraded ball
                AddBall(upgradedLevel, mergeParent.transform, damage);

                //Set incremental button
                isMerging = false;
                IncrementalManager.Instance.CheckMergeButton();

                HapticManager.Haptic(HapticType.MediumImpact);
            });
    }

    #endregion
}