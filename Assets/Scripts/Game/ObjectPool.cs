using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Ç® ¼³Á¤")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject hpSliderPrefab;
    [SerializeField] private int poolSize = 100;

    public Queue<GameObject> unitPool = new Queue<GameObject>();
    public Queue<GameObject> hpSliderPool = new Queue<GameObject>();

    void Start()
    {
        InitializePool(unitPrefab, unitPool);
        InitializePool(hpSliderPrefab, hpSliderPool);
    }

    private void InitializePool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public GameObject GetFromPool(Queue<GameObject> pool)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return null; 
        }
    }
    public void ReturnToPool(GameObject obj, Queue<GameObject> pool)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}