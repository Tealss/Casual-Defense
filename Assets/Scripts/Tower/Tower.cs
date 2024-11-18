using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerStats towerStats; 
    public int level = 1;  
    public int towerIndex;  

    private float attackTimer;
    private ObjectPool objectPool;
    public string towerType;

    public List<ItemStats> itemStats = new List<ItemStats>(); 

    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float criticalChance;
    public float criticalDamage;
    public float enemySlowAmount;
    public float goldEarnRate;

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool을 찾을 수 없음");
            return;
        }

        attackTimer = 0f;

        if (towerStats == null)
        {
            Debug.LogError($"타워 스탯이 할당되지 않았음 {gameObject.name}");
            return;
        }

        InitializeStats();
    }

    public void InitializeStats()
    {
        if (towerStats != null)
        {
            //Debug.Log($"타워 {towerStats.towerName} 생성 - 공격력: {towerStats.attackDamage}, 범위: {towerStats.attackRange}, 공격속도: {towerStats.attackSpeed}");
        }
        else
        {
            Debug.LogError($"타워 스탯 초기화 실패: {gameObject.name}");
        }
    }

    private void Update()
    {
        if (towerStats == null || objectPool == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f / towerStats.attackSpeed)
        {
            attackTimer = 0f;
            Attack();
        }
    }

    private void Attack()
    {
        GameObject target = FindNearestMonster();
        if (target != null)
        {
            GameObject projectile = objectPool.GetProjectileFromPool(towerIndex);
            if (projectile == null)
            {
                Debug.LogError("Projectile 풀을 확인요망");
                return;
            }

            projectile.transform.position = transform.position;
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetTarget(target.transform);  // 타겟 설정
                projectileScript.speed = towerStats.projectileSpeed;  // 프로젝타일 속도 설정
                projectileScript.SetTowerStats(towerStats);  // 타워의 스탯 설정
            }
            else
            {
                Debug.LogError("Projectile 스크립트가 할당되지 않음");
            }
        }
    }

    private GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance && distance <= towerStats.attackRange)
            {
                minDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }
}
