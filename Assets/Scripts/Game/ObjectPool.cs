using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject monsterPrefab;
    public GameObject healthBarPrefab;
    public GameObject towerBuildButtonPrefab;
    public GameObject towerMergeButtonPrefab;

    public GameObject[] towerPrefabs = new GameObject[7];
    public GameObject[] mergeEftPrefabs = new GameObject[7];
    public GameObject[] projectilePrefabs = new GameObject[7];
    public GameObject[] hitEftPrefabs = new GameObject[7];
    public TowerStats[] towerStatsArray = new TowerStats[7];

    // No Pool
    public GameObject[] bountyMonsterPrefabs;

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private int initialPoolSize = 1;

    void Start()
    {
        // Initialize fixed pools
        RegisterPool("Monster", monsterPrefab);
        RegisterPool("HealthBar", healthBarPrefab);
        RegisterPool("TowerBuildButton", towerBuildButtonPrefab);
        RegisterPool("TowerMergeButton", towerMergeButtonPrefab);
        // Initialize dynamic pools (arrays)
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            RegisterPool($"Bounty_{i}", bountyMonsterPrefabs[i]);
            RegisterPool($"Tower_{i}", towerPrefabs[i]);
            RegisterPool($"MergeEffect_{i}", mergeEftPrefabs[i]);
            RegisterPool($"Projectile_{i}", projectilePrefabs[i]);
            RegisterPool($"HitEffect_{i}", hitEftPrefabs[i]);
        }
    }
    private void RegisterPool(string poolName, GameObject prefab)
    {
        if (!pools.ContainsKey(poolName))
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            pools.Add(poolName, pool);
            InitializePool(prefab, pool, initialPoolSize);
        }
    }

    private void InitializePool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);

            if (Folder.folder != null)
            {
                obj.transform.SetParent(Folder.folder.transform, false);
            }
        }
    }

    public GameObject GetFromPool(string poolName, GameObject prefab)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool {poolName} does not exist. Creating a new pool.");
            RegisterPool(poolName, prefab);
        }

        Queue<GameObject> pool = pools[poolName];
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);

            if (Folder.folder != null)
            {
                obj.transform.SetParent(Folder.folder.transform, false);
            }

            return obj;


        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (pools.ContainsKey(poolName))
        {
            obj.SetActive(false);
            pools[poolName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Pool {poolName} does not exist. Object will be destroyed.");
            Destroy(obj);
        }
    }

    public GameObject GetTower(int index)
    {
        string poolName = $"Tower_{index}";
        GameObject tower = GetFromPool(poolName, towerPrefabs[index]);
        Tower towerComponent = tower.GetComponent<Tower>();

        if (towerComponent != null)
        {
            towerComponent.towerStats = towerStatsArray[index];
            towerComponent.InitializeStats();
        }

        return tower;
    }

    public void ReturnTower(int index, GameObject tower)
    {
        string poolName = $"Tower_{index}";
        ReturnToPool(poolName, tower);
    }

    public GameObject GetProjectile(int index)
    {
        return GetFromPool($"Projectile_{index}", projectilePrefabs[index]);
    }

    public void ReturnProjectile(int index, GameObject projectile)
    {
        ReturnToPool($"Projectile_{index}", projectile);
    }
}
