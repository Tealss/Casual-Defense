using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public Tower[] towers;
    public ItemStats itemStats;
    public int gold;
    public int lifePoints = 1;
    public int totalLifePoints = 1;

    public int playerLevel = 1;
    public int playerExperience = 0;
    public int expScore = 0;
    public int experienceToNextLevel = 50;

    public int bestWave = 0;
    public int currentWave;
    public int playTime;


    private bool isGameOver = false;
    public bool isBlinking = true;
    public bool hasTransitioned = false;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            //if (transform.parent != null)
            //{
            //    transform.SetParent(null);
            //}
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadPlayerProgress();
        InitializeAllTowers();
        UpdateStartingGold();
        StartCoroutine(IncrementPlayTime());
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
        int lvBonus = playerLevel * 100;
        gold = 1500 + lvBonus;
    }

    public void AddExperience(int amount)
    {
        playerExperience += amount;
        Debug.Log($"Experience Gained: {amount}, Total: {playerExperience}/{experienceToNextLevel}");

        if (playerExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        SavePlayerProgress();
    }

    private void LevelUp()
    {
        playerExperience -= experienceToNextLevel;
        playerLevel++;
        experienceToNextLevel += 50;
        Debug.Log($"Level Up! New Level: {playerLevel}, Next Level Requires: {experienceToNextLevel} EXP");

        UpdateStartingGold();
        SavePlayerProgress();
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
                Time.timeScale = 0;
                GameUiManager.I.ShowGameOverPanel();
            }
        }
    }

    private void CalculateScoreAndGrantExperience()
    {
        //int expScore = 0;
        currentWave = WaveManager.I != null ? WaveManager.I.currentWave : 1;

        for (int i = 1; i <= currentWave; i++)
        {
            int multiplier = (i - 1) / 10 + 1;
            expScore += multiplier;
        }

        UpdateBestWave(currentWave);
        AddExperience(expScore);
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

    private IEnumerator IncrementPlayTime()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(1f); 
            playTime++; 
            //Debug.Log($"Play Time: {playTime} seconds");
        }
    }

    private void UpdateBestWave(int wave)
    {
        if (wave >= bestWave)
        {
            bestWave = wave;
            SavePlayerProgress();
            if (bestWave > WaveManager.I.currentWave)
            {
                GameUiManager.I.newRecord.SetActive(false);
            }
        }
    }

    private void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("PlayerExperience", playerExperience);
        PlayerPrefs.SetInt("ExperienceToNextLevel", experienceToNextLevel);
        PlayerPrefs.SetInt("BestWave", bestWave);
        PlayerPrefs.Save();
        Debug.Log("Player progress saved.");
    }

    private void LoadPlayerProgress()
    {
        if (PlayerPrefs.HasKey("PlayerLevel") && 
            PlayerPrefs.HasKey("PlayerExperience") &&
            PlayerPrefs.HasKey("ExperienceToNextLevel") && 
            PlayerPrefs.HasKey("BestWave"))
        {
            playerLevel = PlayerPrefs.GetInt("PlayerLevel");
            playerExperience = PlayerPrefs.GetInt("PlayerExperience");
            experienceToNextLevel = PlayerPrefs.GetInt("ExperienceToNextLevel");
            bestWave = PlayerPrefs.GetInt("BestWave");
            Debug.Log("Player progress loaded.");
        }
        else
        {
            Debug.Log("No saved player progress found. Starting fresh.");
        }
    }
}
