using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private ObjectPool objectPool;
    public static EffectManager I; // 싱글톤으로 변경

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);

        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool not found!");
        }
    }

    // 타워 타입 인덱스와 위치를 받아 히트 이펙트를 생성하는 메서드
    public void SpawnHitEffect(int towerTypeIndex, Vector3 position)
    {
        if (objectPool != null)
        {
            GameObject hitEffect = objectPool.GetHitEffectFromPool(towerTypeIndex);
            if (hitEffect != null)
            {
                hitEffect.transform.SetParent(Folder.folder.transform, false);
                hitEffect.transform.position = position;
                hitEffect.SetActive(true);

                // 일정 시간 후에 이펙트를 풀로 반환
                StartCoroutine(ReturnEffectToPoolAfterDelay(hitEffect, towerTypeIndex, 0.5f));
            }
        }
    }

    private IEnumerator ReturnEffectToPoolAfterDelay(GameObject effect, int towerTypeIndex, float delay)
    {
        if (effect == null || !effect.activeInHierarchy)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        effect.SetActive(false);

        if (objectPool != null)
        {
            objectPool.ReturnHitEffectToPool(effect, towerTypeIndex);
        }
    }
}
