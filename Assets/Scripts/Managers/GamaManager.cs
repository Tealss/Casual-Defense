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

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject); // GameManager가 씬 전환 시에도 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadPlayerProgress(); // 플레이어 진행 데이터 로드
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
        gold = 2000 * playerLevel + 100;
    }

    public void AddExperience(int amount)
    {
        playerExperience += amount;
        Debug.Log($"Experience Gained: {amount}, Total: {playerExperience}/{experienceToNextLevel}");

        if (playerExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        SavePlayerProgress(); // 경험치 변경 시 진행 데이터 저장
    }

    private void LevelUp()
    {
        playerExperience -= experienceToNextLevel;
        playerLevel++;
        experienceToNextLevel += 50;
        Debug.Log($"Level Up! New Level: {playerLevel}, Next Level Requires: {experienceToNextLevel} EXP");

        UpdateStartingGold();
        SavePlayerProgress(); // 레벨 변경 시 진행 데이터 저장
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
        Debug.Log("작동");
        if (wave >= bestWave)
        {
            bestWave = wave;
            SavePlayerProgress();
            Debug.Log("작동");
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
        PlayerPrefs.SetInt("BestWave", bestWave);
        PlayerPrefs.Save();
        Debug.Log("Player progress saved.");
    }

    private void LoadPlayerProgress()
    {
        if (PlayerPrefs.HasKey("PlayerLevel") && PlayerPrefs.HasKey("PlayerExperience") && PlayerPrefs.HasKey("BestWave"))
        {
            playerLevel = PlayerPrefs.GetInt("PlayerLevel");
            playerExperience = PlayerPrefs.GetInt("PlayerExperience");
            bestWave = PlayerPrefs.GetInt("BestWave");
            Debug.Log("Player progress loaded.");
        }
        else
        {
            Debug.Log("No saved player progress found. Starting fresh.");
        }
    }
}
