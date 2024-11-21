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

    private Dictionary<Tower, GameObject> activeEffects = new Dictionary<Tower, GameObject>();

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
        if (GameManager.I.Gold < 300) 
        {
            Debug.Log("No Money");
        }

        if (currentBuildButton == null)
        {
            currentBuildButton = objectPool.GetFromPool(
                objectPool.towerBuildButtonPool,
                objectPool.towerBuildButtonPrefab
            );
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
            objectPool.ReturnToPool(currentBuildButton, objectPool.towerBuildButtonPool);
            currentBuildButton = null;
        }
    }

    private void ShowMergeButton(Tower tower)
    {
        if (currentMergeButton == null)
        {
            currentMergeButton = objectPool.GetFromPool(
                objectPool.towerMergeButtonPool,
                objectPool.towerMergeButtonPrefab
            );
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
            objectPool.ReturnToPool(currentMergeButton, objectPool.towerMergeButtonPool);
            currentMergeButton = null;
        }
    }

    private void PlaceTower(Tiles clickedTile)
    {
        if (clickedTile == null || clickedTile.HasTower) return;

        if (GameManager.I.Gold < 300)
        {
            //fadeout 텍스트용 자리
        }

        GameManager.I.SpendGold(300);

        int randomTowerIndex = Random.Range(0, objectPool.towerPrefabs.Length);
        GameObject selectedTowerGO = objectPool.GetTowerFromPool(randomTowerIndex);

        if (selectedTowerGO != null)
        {
            Tower newTower = selectedTowerGO.GetComponent<Tower>();
            if (newTower == null)
            {
                newTower = selectedTowerGO.AddComponent<Tower>();
            }
            newTower.level = 1;
            newTower.towerType = selectedTowerGO.name;

            Vector3 towerPosition = clickedTile.transform.position;
            Collider tileCollider = clickedTile.GetComponent<Collider>();
            if (tileCollider != null)
            {
                Collider towerCollider = selectedTowerGO.GetComponent<Collider>();
                if (towerCollider != null)
                {
                    towerPosition.y = tileCollider.bounds.max.y + towerCollider.bounds.extents.y;
                }
                else
                {
                    towerPosition.y = tileCollider.bounds.max.y;
                }
            }

            SoundManager.I.PlaySoundEffect(6);
            selectedTowerGO.transform.position = towerPosition;
            selectedTowerGO.transform.SetParent(clickedTile.transform);
            clickedTile.HasTower = true;
            HideBuildButton();
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
            bool stillValid = groupedTowers.Any(g => g.Contains(tower));
            if (!stillValid)
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
                    int effectIndex = tower.level - 1;
                    GameObject effect = ShowMergeEffect(effectIndex, tower.transform.position);
                    activeEffects[tower] = effect;
                }
            }
        }
    }

    private GameObject ShowMergeEffect(int effectIndex, Vector3 position)
    {

        GameObject mergeEffectFolder = GameObject.Find("ObjectPool");
        if (mergeEffectFolder == null)
        {
            mergeEffectFolder = new GameObject("ObjectPool");
        }

        GameObject effect = objectPool.GetMergeEffectFromPool(effectIndex);
        effect.transform.SetParent(mergeEffectFolder.transform, false);
        effect.transform.position = position;


        return effect;
    }
    private void RemoveMergeEffect(Tower tower)
    {
        if (activeEffects.ContainsKey(tower))
        {
            objectPool.ReturnMergeEffectToPool(activeEffects[tower], tower.level - 1);
            activeEffects.Remove(tower);
        }
    }

    private void MergeTower(Tower tower)
    {
        var sameTypeAndLevelTowers = FindObjectsOfType<Tower>()
            .Where(t => t.towerType == tower.towerType && t.level == tower.level && t != tower).ToList();

        if (sameTypeAndLevelTowers.Count > 0)
        {
            Tower mergingTower = sameTypeAndLevelTowers[0];
            RemoveMergeEffect(mergingTower);

            tower.level++;

            Tiles tileOfOtherTower = mergingTower.GetComponentInParent<Tiles>();
            if (tileOfOtherTower != null)
            {
                tileOfOtherTower.HasTower = false;
            }

            Destroy(mergingTower.gameObject);

            if (tower.transform.parent.TryGetComponent<Tiles>(out Tiles currentTile))
            {
                currentTile.HasTower = true;
            }

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

    private bool CanMergeByTypeAndLevel(Tower tower)
    {
        return FindObjectsOfType<Tower>().Count(t => t.towerType == tower.towerType && t.level == tower.level) > 1;
    }

}
