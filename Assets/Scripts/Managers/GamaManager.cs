using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Tower[] towers;
    //public TowerStats towerStats;
    public static GameManager I { get; private set; }
    public int gold { get; private set; } = 2000;
    public int lifePoints { get; private set; } = 50;
    public int totalLifePoints { get; private set; } = 50;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeAllTowers();
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

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        else
        {
            Debug.Log("골드가 부족합니다!");
            return false;
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public void DecreaseLifePoints(int amount)
    {
        lifePoints -= amount;
        if (lifePoints < 0) lifePoints = 0;

        if (GameUiManager.I != null)
        {
            GameUiManager.I.UpdateLifePointsText(lifePoints, totalLifePoints);
        }

        if (lifePoints <= 0)
        {
            if (GameUiManager.I != null)
            {
                GameUiManager.I.ShowGameOverPanel();
            }
            //Debug.Log("게임 오버!");
        }
    }

}
