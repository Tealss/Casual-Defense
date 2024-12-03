using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float level;
    public float speed = 0;
    public float damage { get; private set; }
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
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private ObjectPool objectPool;
    private IProjectileBehavior projectileBehavior;

    //Lighting Tower
    public int maxChainHits = 3;
    public HashSet<Monster> previousTargets = new HashSet<Monster>();

    //Ice Tower
    public float slowAmount = 0;
    public float slowDuration = 3f;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    private void OnEnable()
    {
        isActive = true;

        if (towerTransform != null && target != null)
        {
            CalculateFirePoint(target);
            transform.position = startPosition;
        }
    }

    private void CalculateFirePoint(Transform targetTransform)
    {
        Collider targetCollider = targetTransform.GetComponent<Collider>();
        if (targetCollider != null)
        {
            Vector3 colliderCenter = targetCollider.bounds.center;

            Vector3 firePoint = colliderCenter + targetTransform.up * 1f + targetTransform.forward * 0.5f;
            startPosition = firePoint;
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
        if (isActive)
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

    public void SetTowerStats(TowerStats towerStats)
    {
        if (towerStats != null)
        {
            level = towerStats.level;
            damage = towerStats.attackDamage;
            speed = towerStats.projectileSpeed;
            criticalChance = towerStats.criticalChance;
            criticalDamage = towerStats.criticalDamage;
            slowAmount = towerStats.enemySlowAmount;
            goldEarn = towerStats.goldEarnAmount;
            range = towerStats.attackRange;
        }
    }

    public void SetTowerTransform(Transform towerTransform, int projectileTypeIndex)
    {
        this.towerTransform = towerTransform;
        this.projectileTypeIndex = projectileTypeIndex;

    }

    public void SetProjectileBehavior(IProjectileBehavior behavior)
    {
        projectileBehavior = behavior;
    }

    private void MoveTowardsTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, moveDistance))
        {
            Monster monster = hit.transform.GetComponent<Monster>();
            if (monster != null && hit.transform == target)
            {
                DealDamageToTarget();
                ReturnToPool();
                return;
            }
        }

        transform.Translate(direction * moveDistance);

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
