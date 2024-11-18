using UnityEngine;

[CreateAssetMenu(fileName = "New Item Stats", menuName = "Tower Defense/Item Stats", order = 2)]
public class ItemStats : ScriptableObject
{
    public string itemName;
    public string itemDescription;  
    public Sprite itemIcon;

    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float criticalChance;
    public float criticalDamage;
    public float enemySlowAmount;
    public float goldEarnRate;
}