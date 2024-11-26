using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class TowerManager : MonoBehaviour
{
    private GameObject currentBuildButton;
    private GameObject currentMergeButton;
    private Canvas canvas;
    private ObjectPool objectPool;
    private Tiles selectedTile;
    private Tower selectedTower;
    public int[] towerTypes = new int[7];

    public static TowerManager I { get; private set; }
    private Dictionary<Tower, GameObject> activeEffects = new Dictionary<Tower, GameObject>();

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(PeriodicEffectUpdate());
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        objectPool = FindObjectOfType<ObjectPool>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }

    private void HandleTileClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Tile"))
            {
                selectedTile = hit.collider.GetComponent<Tiles>();
                Vector3 tileCenter = hit.collider.bounds.center;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(tileCenter);

                if (selectedTile != null)
                {
                    if (!selectedTile.HasTower)
                    {
                        HideMergeButton();
                        ShowBuildButton(screenPosition);
                    }
                    else
                    {
                        HideBuildButton();
                        selectedTower = selectedTile.GetComponentInChildren<Tower>();
                        if (selectedTower != null && CanMergeByTypeAndLevel(selectedTower))
                        {
                            ShowMergeButton(selectedTower);
                        }
                    }
                }
            }
            else if (hit.collider.CompareTag("Tower"))
            {
                selectedTower = hit.collider.GetComponent<Tower>();
                if (selectedTower != null)
                {
                    if (CanMergeByTypeAndLevel(selectedTower))
                    {
                        HideBuildButton();
                        ShowMergeButton(selectedTower);
                    }
                }
            }
            else
            {
                HideBuildButton();
                HideMergeButton();
            }
        }
        else
        {
            HideBuildButton();
            HideMergeButton();
        }
    }

    private void ShowBuildButton(Vector3 screenPosition)
    {
        if (GameManager.I.gold < 300)
        {
            Debug.Log("No Money");
            return;
        }

        if (currentBuildButton == null)
        {
            currentBuildButton = objectPool.GetFromPool("TowerBuildButton", objectPool.towerBuildButtonPrefab);
            currentBuildButton.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        }

        currentBuildButton.GetComponent<RectTransform>().position = screenPosition + new Vector3(0, 8f, 0);
        currentBuildButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        currentBuildButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => PlaceTower(selectedTile));
    }

    private void HideBuildButton()
    {
        if (currentBuildButton != null)
        {
            objectPool.ReturnToPool("TowerBuildButton", currentBuildButton);
            currentBuildButton = null;
        }
    }

    private void ShowMergeButton(Tower tower)
    {
        if (currentMergeButton == null)
        {
            currentMergeButton = objectPool.GetFromPool("TowerMergeButton", objectPool.towerMergeButtonPrefab);
            currentMergeButton.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        }

        Vector3 towerScreenPosition = Camera.main.WorldToScreenPoint(tower.transform.position);
        currentMergeButton.GetComponent<RectTransform>().position = towerScreenPosition + new Vector3(0, 8f, 0);
        currentMergeButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        currentMergeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => MergeTower(tower));
    }

    private void HideMergeButton()
    {
        if (currentMergeButton != null)
        {
            objectPool.ReturnToPool("TowerMergeButton", currentMergeButton);
            currentMergeButton = null;
        }
    }

    private void PlaceTower(Tiles clickedTile)
    {
        if (clickedTile == null || clickedTile.HasTower) return;

        if (GameManager.I.gold < 300)
        {
            ShowNotEnoughGoldMessage(clickedTile.transform.position);
            HideBuildButton();
            return;
        }

        GameManager.I.SpendGold(300);

        int randomTowerIndex = Random.Range(0, objectPool.towerPrefabs.Length);
        GameObject towerGO = objectPool.GetFromPool($"Tower_{randomTowerIndex}", objectPool.towerPrefabs[randomTowerIndex]);

        if (towerGO != null)
        {
            SetupTowerOnTile(towerGO, clickedTile, randomTowerIndex);
            ShowGoldSpentMessage(clickedTile.transform.position, 300);
        }

        HideBuildButton();
    }

    private void SetupTowerOnTile(GameObject towerGO, Tiles tile, int towerIndex)
    {
        float tileHeight = tile.GetComponent<Collider>().bounds.size.y;
        Vector3 towerPosition = tile.transform.position + new Vector3(0, tileHeight / 2 + 0.5f, 0); 

        Tower newTower = towerGO.GetComponent<Tower>() ?? towerGO.AddComponent<Tower>();
        newTower.level = 1;
        newTower.towerType = $"Tower_{towerIndex}";

        towerGO.transform.position = towerPosition;
        towerGO.transform.SetParent(tile.transform); 
        tile.HasTower = true;
    }

    private void ShowNotEnoughGoldMessage(Vector3 position)
    {
        ShowFadeOutMessage(position, "Not Enough Gold!", Color.red);
    }

    private void ShowGoldSpentMessage(Vector3 position, int amount)
    {
        ShowFadeOutMessage(position, $"-{amount}", Color.red);
    }

    private void ShowFadeOutMessage(Vector3 position, string message, Color color)
    {
        FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
        fadeOutTextSpawner?.SpawnFadeOutText(position + new Vector3(0.2f, 0.2f, 0), message, color);
    }

    private void MergeTower(Tower tower)
    {
        var sameTowers = FindObjectsOfType<Tower>()
            .Where(t => t.towerType == tower.towerType && t.level == tower.level && t != tower)
            .ToList();

        if (sameTowers.Count > 0)
        {
            Tower mergingTower = sameTowers[0];
            RemoveMergeEffect(mergingTower);

            tower.level++;
            tower.ApplyMergeBonus();

            Tiles otherTile = mergingTower.GetComponentInParent<Tiles>();
            if (otherTile != null)
            {
                otherTile.HasTower = false;
            }
            objectPool.ReturnToPool($"Tower_{tower.level - 1}", mergingTower.gameObject);

            RemoveMergeEffect(tower);
        }

        HideMergeButton();
    }

    private IEnumerator PeriodicEffectUpdate()
    {
        while (true)
        {
            UpdateMergeEffects();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void UpdateMergeEffects()
    {
        var groupedTowers = FindObjectsOfType<Tower>()
            .GroupBy(t => new { t.towerType, t.level })
            .Where(g => g.Count() > 1);

        var activeEffectTowers = activeEffects.Keys.ToList();

        foreach (var tower in activeEffectTowers)
        {
            if (!groupedTowers.Any(g => g.Contains(tower)))
            {
                RemoveMergeEffect(tower);
            }
        }

        foreach (var group in groupedTowers)
        {
            foreach (var tower in group)
            {
                if (!activeEffects.ContainsKey(tower))
                {
                    int effectIndex = Mathf.Max(0, tower.level - 1);
                    GameObject effect = ShowMergeEffect(effectIndex, tower.transform.position);
                    activeEffects[tower] = effect;
                }
            }
        }
    }

    private GameObject ShowMergeEffect(int effectIndex, Vector3 position)
    {
        effectIndex = Mathf.Clamp(effectIndex, 0, objectPool.mergeEftPrefabs.Length - 1);

        // Log the index and prefab being requested
        Debug.Log($"index: {effectIndex}");

        GameObject effect = objectPool.GetFromPool($"MergeEffect_{effectIndex}", objectPool.mergeEftPrefabs[effectIndex]);


        effect.transform.position = position;
        return effect;
    }

    private void RemoveMergeEffect(Tower tower)
    {
        if (activeEffects.TryGetValue(tower, out GameObject effect))
        {
            objectPool.ReturnToPool($"MergeEffect_{tower.level - 1}", effect);
            activeEffects.Remove(tower);
        }
    }

    private bool CanMergeByTypeAndLevel(Tower tower)
    {
        return FindObjectsOfType<Tower>().Count(t => t.towerType == tower.towerType && t.level == tower.level) > 1;
    }
}
