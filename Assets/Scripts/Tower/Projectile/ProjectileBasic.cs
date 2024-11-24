using UnityEngine;

public class ProjectileBasic : IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        if (target != null && target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;
                float finalDamage = isCriticalHit ? projectile.Damage * projectile.CriticalDamage : projectile.Damage;

                monster.TakeDamage(finalDamage);

                EffectManager.I.SpawnHitEffect(0, target.position);

                Vector3 spawnPosition = target.position + new Vector3(0.5f, 1f, 0);
                string damageText = isCriticalHit ? $"-{(int)finalDamage}!" : $"-{(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);
            }
        }
    }
}
