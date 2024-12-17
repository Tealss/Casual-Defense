using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager I;

    [Header("Mission Rewards")]
    public Dictionary<int, int> levelRewards = new Dictionary<int, int>
    {
        { 1, 1000 }, { 2, 2000 }, { 3, 3000 },{ 4, 4000 },{ 5, 5000 },{ 6, 6000 },{ 7, 7000 }
    };

    private HashSet<int> completedLevels = new HashSet<int>();

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    public void CheckMissionCompletion(List<Tower> towers)
    {
        var groupedByLevel = towers
            .GroupBy(t => t.level)
            .Where(group =>
                !completedLevels.Contains(group.Key) &&
                group.Select(t => t.towerType).Distinct().Count() >= 6
            );

        foreach (var group in groupedByLevel)
        {
            int level = group.Key;

            if (levelRewards.TryGetValue(level, out int reward))
            {
                //Debug.Log($"[Mission Complete] Level {level}: 7 different tower types found! Reward: {reward} gold.");
                SoundManager.I.PlaySoundEffect(14);
                GameUiManager.I.mission[level -1].SetActive(true);
                GameManager.I.AddGold(reward);
                completedLevels.Add(level);

            }
            else
            {
                //Debug.LogWarning($"[MissionManager] No reward defined for level {level}.");
            }
        }
    }

    public void ResetMissionStatus()
    {
        completedLevels.Clear();
        Debug.Log("[MissionManager] Mission progress has been reset.");
    }
}
