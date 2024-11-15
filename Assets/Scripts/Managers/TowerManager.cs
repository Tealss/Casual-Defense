using UnityEngine;

public class TowerManager : MonoBehaviour
{
    private GameObject currentBuildButton;
    private Canvas canvas;
    private ObjectPool objectPool;

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        objectPool = FindObjectOfType<ObjectPool>();  // ObjectPoolÀ» °¡Á®¿È
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
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Tile"))
        {
            Collider tileCollider = hit.collider;
            Vector3 tileCenter = tileCollider.bounds.center; 

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(tileCenter);

            if (currentBuildButton == null)
            {
                currentBuildButton = objectPool.GetFromPool(
                    objectPool.towerBuildButtonPool,
                    objectPool.towerBuildButtonPrefab
                );

                currentBuildButton.GetComponent<RectTransform>().SetParent(canvas.transform, false);
            }

            currentBuildButton.GetComponent<RectTransform>().position = screenPosition + new Vector3(0, 8f, 0);
        }
        else if (currentBuildButton != null)
        {
            objectPool.ReturnToPool(currentBuildButton, objectPool.towerBuildButtonPool);
            currentBuildButton = null;
        }
    }
}
