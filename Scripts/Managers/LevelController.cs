using System;
using DG.Tweening;
using UnityEngine;
using VP.Nest.Analytics;
using VP.Nest.UI.InGame;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private InGameUI inGameUI;
    [SerializeField] private GameObject[] levels;

    public Action OnLevelChange;
    public bool IsNextLevelLoading;

    private GameObject currentLevel;
    private GameObject oldLevel;
    private int levelIndex;

    public void LoadNextLevel()
    {
        AnalyticsManager.LogLevelCompleteEvent(levelIndex);
        Debug.Log($"//////////////////   {levelIndex}.level completed");

        PlayerPrefs.SetInt("NextLevel", levelIndex + 1);
        LoadLevel();

        IsNextLevelLoading = false;
        IncrementalManager.Instance.CheckMergeButton();
    }

    public void LoadLevel()
    {
        //Set level header text
        int gameLevel = PlayerPrefs.GetInt("GameLevel", 1);
        inGameUI.SetLevelText(gameLevel);

        levelIndex = PlayerPrefs.GetInt("NextLevel", 0) % levels.Length;

        AnalyticsManager.LogLevelStartEvent(levelIndex);
        Debug.Log($"//////////////////   {levelIndex}.level started");

        BallManager.Instance.LoadBalls();

        DOTween.KillAll(true);

        if (currentLevel != null)
        {
            currentLevel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                currentLevel.SetActive(false);
                oldLevel = currentLevel;

                currentLevel = Instantiate(levels[levelIndex], transform);
                currentLevel.transform.localScale = Vector3.zero;
                currentLevel.transform.DOScale(Vector3.one, 0.3f);
                Destroy(oldLevel);
                OnLevelChange?.Invoke();
            });
        }
        else
        {
            currentLevel = Instantiate(levels[levelIndex], transform);
            currentLevel.transform.localScale = Vector3.zero;
            currentLevel.transform.DOScale(Vector3.one, 0.3f);
            OnLevelChange?.Invoke();
        }
    }
}