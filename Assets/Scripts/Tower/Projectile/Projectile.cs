using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 0;
    public float Damage { get; private set; }
    public float CriticalChance => criticalChance;
    public float CriticalDamage => criticalDamage;

    private Transform target;
    private bool isActive = false;

    private float criticalChance = 0f;
    private float criticalDamage = 0f;

    private Transform towerTransform;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private ObjectPool objectPool;
    private IProjectileBehavior projectileBehavior;

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
            Damage = towerStats.attackDamage;
            speed = towerStats.projectileSpeed;
            criticalChance = towerStats.criticalChance;
            criticalDamage = towerStats.criticalDamage;
        }
    }

    public void SetTowerTransform(Transform towerTransform, string towerType)
    {
        this.towerTransform = towerTransform;
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
            objectPool.ReturnProjectileToPool(gameObject);
        }
        gameObject.SetActive(false);
    }
}
