using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileRandom : IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        if (target != null && (target.CompareTag("Monster") || target.CompareTag("Bounty") || target.CompareTag("Boss")))
        {
            if (target == null || !target.gameObject.activeInHierarchy)
                return;

            EffectManager.I.SpawnAttackEffect(4, projectile.transform.position);

            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                float randomDamage = Random.Range(1f, projectile.damage * 2.3f);

                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;
                float finalDamage = isCriticalHit ? randomDamage * projectile.CriticalDamage : randomDamage  ;
                monster.TakeDamage(randomDamage);

                EffectManager.I.SpawnHitEffect(4, target.position);
                SoundManager.I.PlaySoundEffect(5);

                Vector3 spawnPosition = target.position + new Vector3(0.6f, 0.7f, 0);
                string damageText = isCriticalHit ? $"- {(int)finalDamage}!" : $"- {(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);
            }
        }
    }
}