using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public int Gold { get; private set; } = 100000;
    public int LifePoints { get; private set; } = 15;
    public int TotalLifePoints { get; private set; } = 50;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
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
        Gold += amount;
    }

    public void DecreaseLifePoints(int amount)
    {
        LifePoints -= amount;
        if (LifePoints < 0) LifePoints = 0;

        if (GameUiManager.I != null)
        {
            GameUiManager.I.UpdateLifePointsText(LifePoints, TotalLifePoints);
        }

        if (LifePoints <= 0)
        {
            if (GameUiManager.I != null)
            {
                GameUiManager.I.ShowGameOverPanel();
            }
            Debug.Log("게임 오버!");
        }
    }
}
