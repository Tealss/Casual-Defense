using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 0f;
    public float damage = 0f;
    private Transform target;
    private bool isActive = false;

    private float criticalChance = 0f;
    private float criticalDamage = 2f;

    private Transform towerTransform;
    private Vector3 startPosition;
    private Vector3 targetPosition;  // 목표 위치 추가

    private void OnEnable()
    {
        isActive = true;

        if (towerTransform != null)
        {
            startPosition = towerTransform.position + new Vector3(0, 2f, 0);
            transform.position = startPosition;
            //Debug.Log($"[OnEnable] 발사체 시작 위치: {transform.position}");
        }

        //Debug.Log($"[OnEnable] 발사체 활성화: 기본 속도: {speed}, 기본 데미지: {damage}");
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
            targetPosition = target.position;  // 타겟 위치 저장
            //Debug.Log($"[SetTarget] 타겟 설정됨: {target.name}");
        }
        else
        {
            //Debug.LogWarning("[SetTarget] 유효하지 않은 타겟이 설정되었습니다.");
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
            //Debug.Log($"[SetTowerStats] 적용 후 스피드: {speed}, 데미지: {damage}, 크리티컬 확률: {criticalChance}, 크리티컬 데미지: {criticalDamage}");
        }
        else
        {
            Debug.LogWarning("[SetTowerStats] 타워 스탯이 null입니다.");
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null && target.gameObject.activeInHierarchy)
        {
            targetPosition = target.position;
        }

        if (target == null || !target.gameObject.activeInHierarchy)
        {

            if (target == null)
            {
                targetPosition = transform.position;
            }
        }

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
        float randomChance = Random.Range(0f, 1f);
        bool isCriticalHit = randomChance <= criticalChance;

        if (isCriticalHit)
        {
            // 0.1 = 10%
            //Debug.Log($"[DealDamageToTarget] 크리티컬 히트 발생! (확률: {criticalChance * 100}%)");
            damage *= criticalDamage;
        }

        if (target != null && target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                Debug.Log($"[DealDamageToTarget] {target.name}에게 {damage} 데미지 적용");
            }
        }
    }

    private void ReturnToPool()
    {
        ObjectPool objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool != null)
        {
            objectPool.ReturnProjectileToPool(gameObject);
            //Debug.Log("[ReturnToPool] 발사체를 풀로 반환");
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            //Debug.Log($"[OnTriggerEnter] 몬스터와 충돌: {other.name}");
            DealDamageToTarget();
            ReturnToPool();
        }
    }

    public void SetTowerTransform(Transform towerTransform)
    {
        this.towerTransform = towerTransform;
        //Debug.Log($"[SetTowerTransform] 타워 위치 설정: {towerTransform.position}");
    }
}
