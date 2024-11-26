using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 0;
    public float damage { get; private set; }
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;
    public float range = 0;

    private Transform target;
    private bool isActive = false;

    private float criticalChance = 0f;
    private float criticalDamage = 0f;

    //private Tower tower;
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
    public float slowAmount = 0f; 
    public float slowDuration = 3f;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    private void OnEnable()
    {
        isActive = true;

        if (towerTransform != null)
        {
            startPosition = towerTransform.position + new Vector3(0, 2f, 0);
            transform.position = startPosition;
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
    }

    public void SetTarget(Transform targetTransform)
    {
        if (targetTransform != null && targetTransform.gameObject.activeInHierarchy)
        {
            target = targetTransform;
            targetPosition = target.position;
        }
    }

    public void SetTowerStats(TowerStats towerStats)
    {
        if (towerStats != null)
        {
            damage = towerStats.attackDamage;
            speed = towerStats.projectileSpeed;
            criticalChance = towerStats.criticalChance;
            criticalDamage = towerStats.criticalDamage;
            slowAmount = towerStats.enemySlowAmount;
            range = towerStats.attackRange;
            //slowAmount = slowAmount + tower.level;


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

        targetPosition = target.position;

        transform.LookAt(targetPosition);
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget <= 0.5f)
        {
            DealDamageToTarget();
            ReturnToPool();
        }
    }

    private void DealDamageToTarget()
    {
        if (projectileBehavior != null)
        {
            projectileBehavior.Execute(this, target);
        }
    }

    private void ReturnToPool()
    {
        if (objectPool != null)
        {
            string poolName = $"Projectile_{projectileTypeIndex}"; // projectileTypeIndex는 해당 투사체의 타입 인덱스
            objectPool.ReturnToPool(poolName, gameObject);
        }
        gameObject.SetActive(false);
    }
}
