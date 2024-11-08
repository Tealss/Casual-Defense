using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Gold { get; private set; } = 1000; // 초기 골드 예시

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 골드 감소 메서드
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

    // 골드 증가 메서드 (예시)
    public void AddGold(int amount)
    {
        Gold += amount;
    }
}
