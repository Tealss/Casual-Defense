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

            // ������ ��������Ʈ�� �������� ��
            if (currentWaypointIndex >= waypoints.Length)
            {
                // GameManager�� ������ ����Ʈ ����
                gameManager.DecreaseLifePoints(1);

                // ���� ����
                Destroy(gameObject);
            }
        }
    }
}
