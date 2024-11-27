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
            // FirePoint를 계산하여 발사 시작 위치를 설정합니다.
            CalculateFirePoint(target);
            transform.position = startPosition;  // 새로 계산된 startPosition을 적용
        }
    }

    private void CalculateFirePoint(Transform targetTransform)
    {
        // 몬스터의 콜라이더를 찾습니다.
        Collider targetCollider = targetTransform.GetComponent<Collider>();
        if (targetCollider != null)
        {
            // 콜라이더의 중앙 위치
            Vector3 colliderCenter = targetCollider.bounds.center;

            // 몬스터의 "머리 앞위치" 계산
            // 여기서 targetTransform.up은 몬스터의 상단 방향, targetTransform.forward는 몬스터의 앞 방향입니다.
            Vector3 firePoint = colliderCenter + targetTransform.up * 1f + targetTransform.forward * 0.5f;

            // 파이어포인트 위치 설정 (필요에 따라 오프셋 값 조정)
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

        // 물선의 방향은 여전히 원래 방식대로
        Vector3 direction = (target.position - transform.position).normalized;
        float moveDistance = speed * Time.deltaTime;

        // 물선 발사와 관련된 로직은 그대로 유지됩니다.
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
