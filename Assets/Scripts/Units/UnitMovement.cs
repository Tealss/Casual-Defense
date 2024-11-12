using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private GameManager gameManager;

    public void Initialize(Transform[] waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        gameManager = FindObjectOfType<GameManager>();
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

            // 마지막 웨이포인트에 도달했을 때
            if (currentWaypointIndex >= waypoints.Length)
            {
                // GameManager의 라이프 포인트 감소
                gameManager.DecreaseLifePoints(1);

                // 유닛 삭제
                Destroy(gameObject);
            }
        }
    }
}
