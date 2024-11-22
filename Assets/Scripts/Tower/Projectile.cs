using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 0f;
    public float damage = 0f;
    private Transform target;
    private bool isActive = false;

    private float criticalChance = 0f;
    private float criticalDamage = 0f;

    private Transform towerTransform;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private int towerTypeIndex;

    private ObjectPool objectPool;
    private EffectManager effectManager;

    private void Awake()
    {
        // ObjectPool을 미리 한 번만 찾아서 저장해두기
        objectPool = FindObjectOfType<ObjectPool>();
        effectManager = FindObjectOfType<EffectManager>(); // Initialize effectManager
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

    private void OnDisable()
    {
        isActive = false;
        target = null;
    }

    private int GetTowerTypeIndexFromString(string towerType)
    {
        switch (towerType)
        {
            case "T1(Clone)": return 0;
            case "T2(Clone)": return 1;
            case "T3(Clone)": return 2;
            case "T4(Clone)": return 3;
            case "T5(Clone)": return 4;
            case "T6(Clone)": return 5;
            case "T7(Clone)": return 6;
            default: return -1;
        }
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
            damage = towerStats.attackDamage;
            speed = towerStats.projectileSpeed;
            criticalChance = towerStats.criticalChance;
            criticalDamage = towerStats.criticalDamage;
        }
    }

    public void SetTowerTransform(Transform towerTransform, string towerType)
    {
        this.towerTransform = towerTransform;
        this.towerTypeIndex = GetTowerTypeIndexFromString(towerType);  // towerType을 인덱스로 변환
    }

    private void MoveTowardsTarget()
    {
        // 타겟이 비활성화되면 바로 풀로 반환
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        // 타겟의 위치 갱신
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
        float randomChance = Random.Range(0f, 100f);
        bool isCriticalHit = randomChance <= criticalChance;

        float finalDamage = damage; // 기본 데미지
        if (isCriticalHit)
        {
            finalDamage *= criticalDamage;
        }

        if (target != null && target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(finalDamage);

                if (effectManager != null)
                {
                    effectManager.SpawnHitEffect(towerTypeIndex, target.position); // Use the correct towerTypeIndex
                }

                Vector3 spawnPosition = target.position + new Vector3(0.5f, 1f, 0);
                string damageText = isCriticalHit ? $"-{(int)finalDamage}!" : $"-{(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
                if (fadeOutTextSpawner != null)
                {
                    fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, damageText, textColor);
                }
            }
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
