using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("풀 설정")]
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

    private int initialPoolSize = 5; // 초기 생성 개수 설정 (필요시 조정 가능)

    void Start()
    {
        // 일반 풀 초기화
        InitializePool(unitPrefab, unitPool, initialPoolSize);
        InitializePool(hpSliderPrefab, hpSliderPool, initialPoolSize);
        InitializePool(towerBuildButtonPrefab, towerBuildButtonPool, initialPoolSize);
        InitializePool(towerMergeButtonPrefab, towerMergeButtonPool, initialPoolSize);
        InitializePool(projectilePrefab, projectilePool, initialPoolSize); // 프로젝타일 풀 초기화

        // 타워 풀 초기화 (타워 종류마다 풀을 만듦)
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            towerPools[i] = new Queue<GameObject>();
            InitializePool(towerPrefabs[i], towerPools[i], initialPoolSize); // 각 타워 프리팹에 대해 풀 초기화
        }
    }

    // 풀 초기화 메서드
    private void InitializePool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 오브젝트 풀에서 가져오기
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

    // 오브젝트 풀로 반환
    public void ReturnToPool(GameObject obj, Queue<GameObject> pool)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    // 타워 풀에서 타워 가져오기
    public GameObject GetTowerFromPool(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("타워 인덱스가 유효하지 않습니다.");
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

    // 타워 풀로 반환
    public void ReturnTowerToPool(GameObject obj, int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("타워 인덱스가 유효하지 않습니다.");
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
