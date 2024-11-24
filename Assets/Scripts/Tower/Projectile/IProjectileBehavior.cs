using UnityEngine;

public interface IProjectileBehavior
{
    void Execute(Projectile projectile, Transform target);

}
