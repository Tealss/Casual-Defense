using UnityEngine;

public class ProjectileIce : IProjectileBehavior
{

    public void Execute(Projectile projectile, Transform target)
    {
        if (target != null && (target.CompareTag("Monster") || target.CompareTag("Bounty") || target.CompareTag("Boss")))
        {
            if (target == null || !target.gameObject.activeInHierarchy)
                return;
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;
                float finalDamage = isCriticalHit ? projectile.damage * projectile.CriticalDamage : projectile.damage;

                monster.TakeDamage(finalDamage);
                monster.ApplySlow(projectile.slowAmount, projectile.slowDuration);
                //Debug.Log($"�ӵ�: {projectile.slowAmount}");
                EffectManager.I.SpawnHitEffect(3, target.position);

                Vector3 spawnPosition = target.position + new Vector3(0.6f, 0.7f, 0);
                string damageText = isCriticalHit ? $"- {(int)finalDamage}!" : $"- {(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);

                //Debug.Log(projectile.slowAmount);
            }
        }
    }
}