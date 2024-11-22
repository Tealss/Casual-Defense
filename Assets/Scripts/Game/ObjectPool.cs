using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField]
    public GameObject unitPrefab;
    public GameObject hpSliderPrefab;
    public GameObject towerBuildButtonPrefab;
    public GameObject towerMergeButtonPrefab;

    public GameObject[] towerPrefabs = new GameObject[7];
    public GameObject[] mergeEftPrefabs = new GameObject[7];
    public GameObject[] projectilePrefabs = new GameObject[7];
    public GameObject[] hitEftPrefabs = new GameObject[7]; 
    public TowerStats[] towerStatsArray = new TowerStats[7];

    public Queue<GameObject> unitPool = new Queue<GameObject>();
    public Queue<GameObject> hpSliderPool = new Queue<GameObject>();
    public Queue<GameObject> towerBuildButtonPool = new Queue<GameObject>();
    public Queue<GameObject> towerMergeButtonPool = new Queue<GameObject>();
    public Queue<GameObject> projectilePool = new Queue<GameObject>();

    public Queue<GameObject>[] towerPools = new Queue<GameObject>[7];
    public Queue<GameObject>[] mergeEftPools = new Queue<GameObject>[7];
    public Queue<GameObject>[] hitEftPools = new Queue<GameObject>[7]; 

    private int initialPoolSize = 0;

    void Start()
    {
        InitializePool(unitPrefab, unitPool, initialPoolSize);
        InitializePool(hpSliderPrefab, hpSliderPool, initialPoolSize);
        InitializePool(towerBuildButtonPrefab, towerBuildButtonPool, initialPoolSize);
        InitializePool(towerMergeButtonPrefab, towerMergeButtonPool, initialPoolSize);
        InitializePool(projectilePrefabs[0], projectilePool, initialPoolSize);

        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            towerPools[i] = new Queue<GameObject>();
            InitializePool(towerPrefabs[i], towerPools[i], initialPoolSize);

            mergeEftPools[i] = new Queue<GameObject>();
            InitializePool(mergeEftPrefabs[i], mergeEftPools[i], initialPoolSize);

            hitEftPools[i] = new Queue<GameObject>(); // Initialize hit effect pools
            InitializePool(hitEftPrefabs[i], hitEftPools[i], initialPoolSize);
        }
    }

    private void InitializePool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

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

    public void ReturnToPool(GameObject obj, Queue<GameObject> pool)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject GetTowerFromPool(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("Tower Index Error");
            return null;
        }

        GameObject tower = GetFromPool(towerPools[towerIndex], towerPrefabs[towerIndex]);
        Tower towerComponent = tower.GetComponent<Tower>();

        if (towerComponent != null)
        {
            towerComponent.towerStats = towerStatsArray[towerIndex];
            towerComponent.InitializeStats();
        }

        return tower;
    }

    public void ReturnTowerToPool(GameObject obj, int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPools.Length)
        {
            Debug.LogError("Tower Index Error");
            return;
        }

        ReturnToPool(obj, towerPools[towerIndex]);
    }

    public GameObject GetMergeEffectFromPool(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= mergeEftPools.Length)
        {
            Debug.LogError("EFT Index Error");
            return null;
        }

        return GetFromPool(mergeEftPools[effectIndex], mergeEftPrefabs[effectIndex]);
    }

    public void ReturnMergeEffectToPool(GameObject obj, int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= mergeEftPools.Length)
        {
            Debug.LogError("EFT Index Error");
            return;
        }

        ReturnToPool(obj, mergeEftPools[effectIndex]);
    }

    public GameObject GetProjectileFromPool(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= projectilePrefabs.Length)
        {
            Debug.LogError("Projectile Index Error");
            return null;
        }

        return GetFromPool(projectilePool, projectilePrefabs[towerIndex]);
    }

    public void ReturnProjectileToPool(GameObject projectile)
    {
        ReturnToPool(projectile, projectilePool);
    }

    public GameObject GetHitEffectFromPool(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= hitEftPools.Length)
        {
            Debug.LogError("Hit Effect Index Error");
            return null;
        }

        return GetFromPool(hitEftPools[effectIndex], hitEftPrefabs[effectIndex]);
    }

    public void ReturnHitEffectToPool(GameObject hitEffect, int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= hitEftPools.Length)
        {
            Debug.LogError("Hit Effect Index Error");
            return;
        }

        ReturnToPool(hitEffect, hitEftPools[effectIndex]);
    }

    public void ReturnUnitToPool(GameObject unit)
    {
        ReturnToPool(unit, unitPool);
    }

    public void ReturnHpSliderToPool(GameObject slider)
    {
        ReturnToPool(slider, hpSliderPool);
    }

}
