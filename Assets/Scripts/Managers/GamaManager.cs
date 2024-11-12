using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public int Gold { get; private set; } = 100000;
    public int LifePoints { get; private set; } = 50; 

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
            Debug.Log("��尡 �����մϴ�!");
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
        GameUiManager.I.UpdateLifePointsText(LifePoints);

        if (LifePoints <= 0)
        {
            //GameOver();
        }
    }

    //private void GameOver()
    //{
    //    Debug.Log("Game Over!");
    //    GameUiManager.I.ShowGameOverPanel();
    //    Time.timeScale = 0; // ���� ����
    //}
}
