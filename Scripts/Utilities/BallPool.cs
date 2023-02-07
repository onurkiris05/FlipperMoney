using UnityEngine;
using UnityEngine.Pool;

public class BallPool : Singleton<BallPool>
{
    [SerializeField] Ball ballPrefab;

    public IObjectPool<Ball> Pool => ballPool;

    IObjectPool<Ball> ballPool;

    void Awake()
    {
        ballPool = new ObjectPool<Ball>
        (
            CreateBall,
            ActionOnGet,
            ActionOnRelease
        );
    }

    Ball CreateBall()
    {
        Ball ball = Instantiate(ballPrefab, transform);
        return ball;
    }

    void ActionOnGet(Ball ball)
    {
        ball.gameObject.SetActive(true);
    }

    void ActionOnRelease(Ball ball)
    {
        ball.gameObject.SetActive(false);
        ball.transform.position = transform.position;
    }
}