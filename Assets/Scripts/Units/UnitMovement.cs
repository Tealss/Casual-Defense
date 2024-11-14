using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private GameManager gameManager;
    private Monster monster;
    private ObjectPool objectPool;
    private GameObject hpSlider;

    public void Initialize(Transform[] waypoints, float speed, ObjectPool objectPool)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        this.objectPool = objectPool;
        gameManager = FindObjectOfType<GameManager>();
        monster = GetComponent<Monster>();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                if (monster != null)
                    monster.CurrentHealth = 0;

                if (gameManager != null)
                    gameManager.DecreaseLifePoints(1);

                if (hpSlider != null)
                    objectPool.ReturnToPool(hpSlider, objectPool.hpSliderPool);

                objectPool.ReturnToPool(gameObject, objectPool.unitPool);
            }
        }
    }

    public void SetHpSlider(GameObject slider)
    {
        hpSlider = slider;
    }
}
