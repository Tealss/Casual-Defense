using UnityEngine;
using System.Collections.Generic;

public class ProjectileBoss : IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        Transform prioritizedTarget = GetPrioritizedTarget(projectile, target);

        if (prioritizedTarget != null)
        {
            if (target == null || !target.gameObject.activeInHierarchy)
                return;
            Monster monster = prioritizedTarget.GetComponent<Monster>();
            if (monster != null)
            {
                float multiplier = 0.5f;

                if (prioritizedTarget.CompareTag("Boss"))
                {
                    multiplier = 3f;
                }
                else if (prioritizedTarget.CompareTag("Bounty"))
                {
                    multiplier = 2f;
                }

                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;

                float finalDamage = isCriticalHit ? projectile.damage * multiplier * projectile.CriticalDamage : projectile.damage * multiplier;
                monster.TakeDamage(finalDamage);

                EffectManager.I.SpawnHitEffect(6, prioritizedTarget.position);

                Vector3 spawnPosition = prioritizedTarget.position + new Vector3(0.6f, 0.7f, 0);
                string damageText = isCriticalHit ? $"- {(int)finalDamage}!" : $"- {(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);
            }
        }
    }

    private Transform GetPrioritizedTarget(Projectile projectile, Transform originalTarget)
    {
        Collider[] colliders = Physics.OverlapSphere(projectile.transform.position, projectile.range);
        List<Transform> potentialTargets = new List<Transform>();

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Boss") || collider.CompareTag("Bounty") || collider.CompareTag("Monster"))
            {
                potentialTargets.Add(collider.transform);
            }
        }

        potentialTargets.Sort((a, b) =>
        {
            int aPriority = GetPriority(a);
            int bPriority = GetPriority(b);

            return aPriority.CompareTo(bPriority);
        });

        return potentialTargets.Count > 0 ? potentialTargets[0] : originalTarget;
    }

    private int GetPriority(Transform target)
    {
        if (target.CompareTag("Boss")) return 0;
        if (target.CompareTag("Bounty")) return 1;
        if (target.CompareTag("Monster")) return 2;
        return int.MaxValue;
    }
}
