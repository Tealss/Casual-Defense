using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Transform[] waypoints; // 웨이포인트 배열
    private int currentWaypointIndex = 0; // 현재 웨이포인트 인덱스
    private float speed = 5f; // 이동 속도


    public void Initialize(Transform[] waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return; 

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        StartCoroutine(MoveToNext(targetWaypoint));
    }

    private IEnumerator MoveToNext(Transform targetWaypoint)
    {
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
            yield return null;
        }

        currentWaypointIndex++;
        MoveToWaypoint();
    }
}