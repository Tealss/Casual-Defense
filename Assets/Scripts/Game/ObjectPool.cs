using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject healthBarPrefab;
    public GameObject towerBuildButtonPrefab;
    public GameObject towerMergeButtonPrefab;
    public GameObject sellButtonPrefab;

    public GameObject[] monsterPrefabs;
    public GameObject[] towerPrefabs = new GameObject[7];
    public GameObject[] mergeEftPrefabs = new GameObject[7];
    public GameObject[] projectilePrefabs = new GameObject[7];
    public GameObject[] hitEftPrefabs = new GameObject[7];
    public GameObject[] attackEftPrefabs = new GameObject[7];
    public TowerStats[] towerStatsArray = new TowerStats[7];

    public GameObject[] bossMonsterPrefabs;
    public GameObject[] bountyMonsterPrefabs;

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, float> lastUsedTime = new Dictionary<string, float>();
    private int initialPoolSize = 1;

    void Start()
    {
        // 기존 초기화 코드

        RegisterPool("HealthBar", healthBarPrefab);
        RegisterPool("TowerBuildButton", towerBuildButtonPrefab);
        RegisterPool("TowerMergeButton", towerMergeButtonPrefab);
        RegisterPool("SellButton", sellButtonPrefab);

        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            RegisterPool($"Monster_{i}", monsterPrefabs[i]);
        }
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            RegisterPool($"Bounty_{i}", bountyMonsterPrefabs[i]);
            RegisterPool($"Tower_{i}", towerPrefabs[i]);
            RegisterPool($"MergeEffect_{i}", mergeEftPrefabs[i]);
            RegisterPool($"Projectile_{i}", projectilePrefabs[i]);
            RegisterPool($"HitEffect_{i}", hitEftPrefabs[i]);
            RegisterPool($"attackEffect_{i}", attackEftPrefabs[i]);
        }
        for (int i = 0; i < bossMonsterPrefabs.Length; i++)
        {
            RegisterPool($"Boss_{i}", bossMonsterPrefabs[i]);
        }

        StartCoroutine(ClearUnusedPoolsRoutine());
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

        lastUsedTime[poolName] = Time.time;

        Queue<GameObject> pool = pools[poolName];
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
            //ReturnToPool(poolName, newObj);
            return newObj;
        }
    }

    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (pools.ContainsKey(poolName))
        {
            ResetObjectState(obj);
            obj.SetActive(false);
            pools[poolName].Enqueue(obj);

            lastUsedTime[poolName] = Time.time;
        }
        else
        {
            Debug.LogWarning($"Pool {poolName} does not exist. Object will be destroyed.");
            Destroy(obj);
        }
    }
    private void ResetObjectState(GameObject obj)
    {
        //obj.transform.position = Vector3.zero;
        //obj.transform.rotation = Quaternion.identity;

        //var rigidbody = obj.GetComponent<Rigidbody>();
        //if (rigidbody != null)
        //{
        //    rigidbody.velocity = Vector3.zero;
        //    rigidbody.angularVelocity = Vector3.zero;
        //}

        var monster = obj.GetComponent<Monster>();
        if (monster != null)
        {
            monster.ResetState();
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

    private IEnumerator ClearUnusedPoolsRoutine()
    {
        float checkInterval = 60f;
        float unusedDuration = 600f;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            float currentTime = Time.time;
            List<string> poolsToClear = new List<string>();

            foreach (var pool in lastUsedTime)
            {
                if (currentTime - pool.Value > unusedDuration)
                {
                    poolsToClear.Add(pool.Key);
                }
            }

            foreach (string poolName in poolsToClear)
            {
                Debug.Log($"Clearing unused pool: {poolName}");
                ClearPool(poolName);
                lastUsedTime.Remove(poolName);
            }
        }
    }

    public void ClearPool(string poolName)
    {
        if (pools.ContainsKey(poolName))
        {
            Queue<GameObject> pool = pools[poolName];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }
    }
}
