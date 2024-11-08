using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Gold { get; private set; } = 1000; // �ʱ� ��� ����

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ��� ���� �޼���
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

    // ��� ���� �޼��� (����)
    public void AddGold(int amount)
    {
        Gold += amount;
    }
}
