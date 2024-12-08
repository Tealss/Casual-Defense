using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tower[] towers;
    public static GameManager I;

    public int gold { get; private set; }
    public int lifePoints { get; private set; } = 50;
    public int totalLifePoints { get; private set; } = 50;

    public int playerLevel { get; private set; } = 1;
    public int playerExperience { get; private set; } = 0;
    public int experienceToNextLevel { get; private set; } = 100;

    private bool isGameOver = false;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeAllTowers();
        UpdateStartingGold();
    }

    private void InitializeAllTowers()
    {
        foreach (Tower tower in towers)
        {
            if (tower != null)
            {
                tower.InitializeStats();
            }
        }
    }

    private void UpdateStartingGold()
    {
        gold = 20000 * playerLevel + 100;
    }

    public void AddExperience(int amount)
    {
        playerExperience += amount;
        Debug.Log($"Experience Gained: {amount}, Total: {playerExperience}/{experienceToNextLevel}");

        if (playerExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerExperience -= experienceToNextLevel;
        playerLevel++;
        experienceToNextLevel += 50;
        Debug.Log($"Level Up! New Level: {playerLevel}, Next Level Requires: {experienceToNextLevel} EXP");

        UpdateStartingGold();
    }

    public void DecreaseLifePoints(int amount)
    {
        lifePoints -= amount;
        if (lifePoints < 0) lifePoints = 0;

        if (GameUiManager.I != null)
        {
            GameUiManager.I.UpdateLifePointsText(lifePoints, totalLifePoints);
        }

        if (lifePoints <= 0 && !isGameOver)
        {
            isGameOver = true;
            CalculateScoreAndGrantExperience();

            if (GameUiManager.I != null)
            {
                GameUiManager.I.ShowGameOverPanel();
                Time.timeScale = 0;
            }
        }
    }

    private void CalculateScoreAndGrantExperience()
    {
        int totalScore = 0;
        int currentWave = WaveManager.I != null ? WaveManager.I.currentWave : 1;

        for (int i = 1; i <= currentWave; i++)
        {
            int multiplier = (i - 1) / 10 + 1;
            totalScore += multiplier;
        }

        //Debug.Log($"Game Over! Total Score: {totalScore}");

        AddExperience(totalScore);
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }
}
