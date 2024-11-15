using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Ç® ¼³Á¤")]
    public GameObject unitPrefab;
    public GameObject hpSliderPrefab;
    public GameObject towerBuildButtonPrefab;
    [SerializeField] private int initialPoolSize = 10; 

    public Queue<GameObject> unitPool = new Queue<GameObject>();
    public Queue<GameObject> hpSliderPool = new Queue<GameObject>();
    public Queue<GameObject> towerBuildButtonPool = new Queue<GameObject>();

    void Start()
    {
        InitializePool(unitPrefab, unitPool, initialPoolSize);
        InitializePool(hpSliderPrefab, hpSliderPool, initialPoolSize);
        InitializePool(towerBuildButtonPrefab, towerBuildButtonPool, initialPoolSize);
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
}
