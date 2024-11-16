using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Stats", menuName = "Tower Defense/Tower Stats", order = 1)]
public class TowerStats : ScriptableObject
{
    public string towerName;         // Ÿ�� �̸�
    public float attackDamage;       // ���ݷ�
    public float attackSpeed;        // ���� �ӵ�
    public float range;              // ���� ����
    public int towercost;            // Ÿ�� ����
    public GameObject towerPrefab;   // Ÿ�� ������
}