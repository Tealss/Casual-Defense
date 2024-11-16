using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Stats", menuName = "Tower Defense/Tower Stats", order = 1)]
public class TowerStats : ScriptableObject
{
    public string towerName;         // 타워 이름
    public float attackDamage;       // 공격력
    public float attackSpeed;        // 공격 속도
    public float range;              // 공격 범위
    public int towercost;            // 타워 가격
    public GameObject towerPrefab;   // 타워 프리팹
}