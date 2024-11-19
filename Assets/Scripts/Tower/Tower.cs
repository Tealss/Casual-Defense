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

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool�� ã�� �� ����");
            return;
        }

        attackTimer = 0f;

        //if (towerStats == null)
        //{
        //    Debug.LogError($"Ÿ�� ������ �Ҵ���� �ʾ��� {gameObject.name}");
        //    return;
        //}

        InitializeStats();
    }


    public void InitializeStats()
    {
        if (towerStats != null)
        {
            // Ÿ�� ���� �ʱ�ȭ
        }
        else
        {
            Debug.LogError($"Ÿ�� ���� �ʱ�ȭ ����: {gameObject.name}");
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
                Debug.LogError("Projectile Ǯ�� Ȯ�ο��");
                return;
            }

            projectile.transform.position = transform.position;
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                SoundManager.I.PlaySoundEffect(5);
                projectileScript.SetTarget(target.transform);  // Ÿ�� ����
                projectileScript.speed = towerStats.projectileSpeed;  // ������Ÿ�� �ӵ� ����
                projectileScript.SetTowerStats(towerStats);  // Ÿ���� ���� ����
            }
            else
            {
                Debug.LogError("Projectile ��ũ��Ʈ�� �Ҵ���� ����");
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
