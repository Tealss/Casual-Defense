using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private GameManager gameManager;
    private Monster monster;  // Monster ������Ʈ�� ������ ����

    public void Initialize(Transform[] waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        gameManager = FindObjectOfType<GameManager>();

        // ���� ������Ʈ�� ������
        monster = GetComponent<Monster>();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // ���� ��ǥ ��������Ʈ
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        // ��������Ʈ�� ������ ���
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            // ������ ��������Ʈ�� �������� ��
            if (currentWaypointIndex >= waypoints.Length)
            {
                // Monster�� ü���� 0���� ����
                if (monster != null)
                {
                    monster.CurrentHealth = 0;  // ������ ü�� 0���� ����
                }

                // GameManager�� ������ ����Ʈ ����
                if (gameManager != null)
                {
                    gameManager.DecreaseLifePoints(1);  // GameManager�� ������ ����Ʈ ����
                }

                // ���� ����
                Destroy(gameObject);
            }
        }
    }
}
