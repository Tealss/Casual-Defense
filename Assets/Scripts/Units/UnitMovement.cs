using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private GameManager gameManager;
    private Monster monster;  // Monster 컴포넌트를 참조할 변수

    public void Initialize(Transform[] waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        gameManager = FindObjectOfType<GameManager>();

        // 몬스터 컴포넌트를 가져옴
        monster = GetComponent<Monster>();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 현재 목표 웨이포인트
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        // 웨이포인트에 도달한 경우
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            // 마지막 웨이포인트에 도달했을 때
            if (currentWaypointIndex >= waypoints.Length)
            {
                // Monster의 체력을 0으로 설정
                if (monster != null)
                {
                    monster.CurrentHealth = 0;  // 몬스터의 체력 0으로 설정
                }

                // GameManager의 라이프 포인트 감소
                if (gameManager != null)
                {
                    gameManager.DecreaseLifePoints(1);  // GameManager의 라이프 포인트 감소
                }

                // 유닛 삭제
                Destroy(gameObject);
            }
        }
    }
}
