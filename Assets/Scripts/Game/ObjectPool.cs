using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Ǯ ����")]
    public GameObject unitPrefab;
    public GameObject hpSliderPrefab;
    public GameObject towerBuildButtonPrefab;
    public GameObject towerMergeButtonPrefab;
    public GameObject projectilePrefab; 
    public GameObject[] towerPrefabs = new GameObject[7];
    public GameObject[] mergeEft = new GameObject[7];

    public Queue<GameObject> unitPool = new Queue<GameObject>();
    public Queue<GameObject> hpSliderPool = new Queue<GameObject>();
    public Queue<GameObject> towerBuildButtonPool = new Queue<GameObject>();
    public Queue<GameObject> towerMergeButtonPool = new Queue<GameObject>();
    public Queue<GameObject> projectilePool = new Queue<GameObject>(); 
    public Queue<GameObject>[] towerPools = new Queue<GameObject>[7];
    public Queue<GameObject>[] mergeEftPools = new Queue<GameObject>[7];

    private int initialPoolSize = 5; // �ʱ� ���� ���� ���� (�ʿ�� ���� ����)

    void Start()
    {
        // �Ϲ� Ǯ �ʱ�ȭ
        InitializePool(unitPrefab, unitPool, initialPoolSize);
        InitializePool(hpSliderPrefab, hpSliderPool, initialPoolSize);
        InitializePool(towerBuildButtonPrefab, towerBuildButtonPool, initialPoolSize);
        InitializePool(towerMergeButtonPrefab, towerMergeButtonPool, initialPoolSize);
        InitializePool(projectilePrefab, projectilePool, initialPoolSize); // ������Ÿ�� Ǯ �ʱ�ȭ

        // Ÿ�� Ǯ �ʱ�ȭ (Ÿ�� �������� Ǯ�� ����)
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            towerPools[i] = new Queue<GameObject>();
            InitializePool(towerPrefabs[i], towerPools[i], initialPoolSize); // �� Ÿ�� �����տ� ���� Ǯ �ʱ�ȭ
        }
    }

    // Ǯ �ʱ�ȭ �޼���
    private void InitializePool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // ������Ʈ Ǯ���� ��������
    public GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(true);
            return newObj;
        }
    }

    // ������Ʈ Ǯ�� ��ȯ
    public void ReturnToPool(GameObject obj, Queue<GameObject> pool)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    // Ÿ�� Ǯ���� Ÿ�� ��������
    public GameObject GetTowerFromPool(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("Ÿ�� �ε����� ��ȿ���� �ʽ��ϴ�.");
            return null;
        }

        Queue<GameObject> pool = towerPools[towerIndex];

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(towerPrefabs[towerIndex]);
            newObj.SetActive(true);
            return newObj;
        }
    }

    // Ÿ�� Ǯ�� ��ȯ
    public void ReturnTowerToPool(GameObject obj, int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("Ÿ�� �ε����� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        obj.SetActive(false);
        towerPools[towerIndex].Enqueue(obj);
    }

    public GameObject GetProjectileFromPool()
    {
        return GetFromPool(projectilePool, projectilePrefab);
    }

    public void ReturnProjectileToPool(GameObject projectile)
    {
        ReturnToPool(projectile, projectilePool);
    }
}
