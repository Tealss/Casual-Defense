using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float level;
    public float projectileSpeed = 0;
    public float damage = 0;
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float range = 0;
    public float goldEarn = 0;

    private Transform target;
    private bool isActive = false;

    private float criticalChance = 0f;
    private float criticalDamage = 0f;

    private int projectileTypeIndex;
    public Transform towerTransform;
    //private Transform firePoint;
    private Vector3 targetPosition;

    private ObjectPool objectPool;
    private IProjectileBehavior projectileBehavior;

    // Lighting Tower
    public int maxChainHits = 3;
    public HashSet<Monster> previousTargets = new HashSet<Monster>();

    // Ice Tower
    public float slowAmount = 0;
    public float slowDuration = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isActive = true;

        if (towerTransform != null && target != null)
        {

            //transform.position = firePoint.position;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = false; 
            }
        }
    }

    public void SetBehavior(IProjectileBehavior newBehavior)
    {
        projectileBehavior = newBehavior;
    }

    private void OnDisable()
    {
        isActive = false;
        target = null;
    }

    private void Update()
    {
        if (isActive && target != null)
        {
            MoveTowardsTarget();
        }
    }

    public void Initialize()
    {
        previousTargets.Clear();
        slowAmount = 0;
    }

    public void SetTarget(Transform targetTransform)
    {
        if (targetTransform != null && targetTransform.gameObject.activeInHierarchy)
        {
            if (target == null || !target.Equals(targetTransform))
            {
                target = targetTransform;
                targetPosition = target.position;
            }
        }
    }

    public void SetTowerStats(TowerStats towerStats, ItemStats itemStats)
    {
        if (towerStats != null)
        {
            level = towerStats.level;
            damage = towerStats.attackDamage + itemStats.itemAttackDamage;
            criticalChance = towerStats.criticalChance + itemStats.itemCriticalChance;
            criticalDamage = towerStats.criticalDamage + itemStats.itemCriticalDamage;
            slowAmount = towerStats.enemySlowAmount + itemStats.itemEnemySlowAmount;
            goldEarn = towerStats.goldEarnAmount + itemStats.itemGoldEarnAmount;
            range = towerStats.attackRange + itemStats.itemAttackRange;

            projectileSpeed = towerStats.projectileSpeed  + towerStats.attackSpeed + itemStats.itemAttackSpeed ;
        }
    }

    public void SetTowerTransform(Transform towerTransform, int projectileTypeIndex)
    {
        this.towerTransform = towerTransform;
        this.projectileTypeIndex = projectileTypeIndex;
        //firePoint = towerTransform.Find("FirePoint");
    }

    private void MoveTowardsTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 desiredVelocity = direction * projectileSpeed;

        if (rb != null)
        {
            rb.velocity = desiredVelocity;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= 0.5f)
        {
            DealDamageToTarget();
            ReturnToPool();
        }
    }

    private void DealDamageToTarget()
    {
        if (projectileBehavior != null && target != null && target.gameObject.activeInHierarchy)
        {
            projectileBehavior.Execute(this, target);
        }
    }

    private void ReturnToPool()
    {
        if (objectPool != null)
        {
            string poolName = $"Projectile_{projectileTypeIndex}";
            objectPool.ReturnToPool(poolName, gameObject);
        }
        gameObject.SetActive(false);
    }

}
