using UnityEngine;

public class TowerManager : MonoBehaviour
{
    private GameObject currentBuildButton;
    private Canvas canvas;
    private ObjectPool objectPool;

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        objectPool = FindObjectOfType<ObjectPool>();  // ObjectPool을 가져옴
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
            // 타일의 중심 위치와 화면상의 위치 계산
            Vector3 tileCenter = hit.collider.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(tileCenter) + new Vector3(0, 8f, 0);

            if (currentBuildButton == null)
            {
                // 버튼이 없으면 풀에서 가져옴
                currentBuildButton = objectPool.GetFromPool(objectPool.towerBuildButtonPool);
                currentBuildButton.GetComponent<RectTransform>().SetParent(canvas.transform, false);
            }

            // 버튼의 위치를 항상 최신 화면 위치로 업데이트
            currentBuildButton.GetComponent<RectTransform>().position = screenPosition;
        }
        else if (currentBuildButton != null)
        {
            // 타일 외 클릭 시, 버튼을 풀로 반환
            objectPool.ReturnToPool(currentBuildButton, objectPool.towerBuildButtonPool);
            currentBuildButton = null;
        }
    }
}
