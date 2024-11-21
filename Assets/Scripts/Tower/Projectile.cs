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
    private Vector3 targetPosition;  // ��ǥ ��ġ �߰�

    private void OnEnable()
    {
        isActive = true;

        if (towerTransform != null)
        {
            startPosition = towerTransform.position + new Vector3(0, 2f, 0);
            transform.position = startPosition;
            //Debug.Log($"[OnEnable] �߻�ü ���� ��ġ: {transform.position}");
        }

        //Debug.Log($"[OnEnable] �߻�ü Ȱ��ȭ: �⺻ �ӵ�: {speed}, �⺻ ������: {damage}");
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
            targetPosition = target.position;  // Ÿ�� ��ġ ����
            //Debug.Log($"[SetTarget] Ÿ�� ������: {target.name}");
        }
        else
        {
            //Debug.LogWarning("[SetTarget] ��ȿ���� ���� Ÿ���� �����Ǿ����ϴ�.");
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
            //Debug.Log($"[SetTowerStats] ���� �� ���ǵ�: {speed}, ������: {damage}, ũ��Ƽ�� Ȯ��: {criticalChance}, ũ��Ƽ�� ������: {criticalDamage}");
        }
        else
        {
            Debug.LogWarning("[SetTowerStats] Ÿ�� ������ null�Դϴ�.");
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
        float randomChance = Random.Range(0f, 100f);
        bool isCriticalHit = randomChance <= criticalChance;

        float finalDamage = damage; // ���� ������
        if (isCriticalHit)
        {
            finalDamage *= criticalDamage;
            // Debug.Log($"[DealDamageToTarget] ũ��Ƽ�� ��Ʈ �߻�! (Ȯ��: {criticalChance * 100}%)");
        }

        if (target != null && target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(finalDamage);

                Vector3 spawnPosition = target.transform.position + new Vector3(0.5f, 1f, 0);
                string damageText = isCriticalHit ? $"-{(int)finalDamage}!" : $"-{(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
                if (fadeOutTextSpawner != null)
                {
                    fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, damageText, textColor);
                }
                else
                {
                    Debug.LogWarning("FadeOutTextSpawner�� ã�� �� �����ϴ�!");
                }

                Debug.Log($"[DealDamageToTarget] {target.name}���� {finalDamage} ������ ����");
            }
        }
    }


    private void ReturnToPool()
    {
        ObjectPool objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool != null)
        {
            objectPool.ReturnProjectileToPool(gameObject);
            //Debug.Log("[ReturnToPool] �߻�ü�� Ǯ�� ��ȯ");
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            //Debug.Log($"[OnTriggerEnter] ���Ϳ� �浹: {other.name}");
            DealDamageToTarget();
            ReturnToPool();
        }
    }

    public void SetTowerTransform(Transform towerTransform)
    {
        this.towerTransform = towerTransform;
        //Debug.Log($"[SetTowerTransform] Ÿ�� ��ġ ����: {towerTransform.position}");
    }
}
